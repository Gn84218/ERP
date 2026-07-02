# ERP 進銷存系統 API

本專案是一套使用 **ASP.NET Core 8 Web API** 開發的 ERP 進銷存後端系統，主要用來展示我對企業內部系統、後端 API、分層架構、資料一致性與庫存流程設計的理解。

專案重點不是畫面，而是後端架構與商業流程實作。系統包含商品、倉庫、供應商、客戶、採購、收貨、銷售、出貨、調撥、庫存台帳、庫存結餘、JWT 驗證與 Redis 快取。

## 我想展示的能力

- 使用 ASP.NET Core Web API 建立企業系統後端
- 依照 Clean Architecture 概念拆分 API、Application、Domain、Infrastructure
- 使用 Entity Framework Core 操作 SQL Server
- 設計進銷存常見的庫存台帳與庫存結餘模型
- 使用交易處理確保庫存異動資料一致
- 實作防負庫存等商業規則
- 使用 JWT Bearer Authentication 處理登入驗證
- 使用 Redis 示範快取整合
- 使用 Docker Compose 建立可重現的本機開發環境

## 技術摘要

| 類別 | 使用技術 |
| --- | --- |
| 後端框架 | ASP.NET Core 8 Web API |
| 語言 | C# |
| ORM | Entity Framework Core |
| 資料庫 | SQL Server 2022 |
| 快取 | Redis |
| 驗證 | JWT Bearer Authentication |
| 權限 | Role-based Access Control |
| API 文件 | Swagger / OpenAPI |
| 容器化 | Docker Compose |
| 測試 | xUnit 測試專案 |

## 系統分層

本專案使用接近 Clean Architecture 的分層方式，讓 Controller、商業流程、核心模型與資料存取責任分開。

```text
ERP.API
  對外 API 入口
  Controller
  Middleware
  Swagger 設定
  JWT 驗證設定
  Dependency Injection 設定

ERP.Application
  DTO
  Service Interface
  應用層合約
  定義外部如何呼叫商業流程

ERP.Domain
  Entity
  Enum
  核心商業模型
  不依賴資料庫與 Web Framework

ERP.Infrastructure
  EF Core DbContext
  Service 實作
  SQL Server 資料存取
  Redis 快取
  JWT Token 產生器

ERP.Tests
  單元測試專案
```

### 分層目的

- Controller 只負責接收 HTTP Request 與回傳 Response
- Application 定義 DTO 與服務介面，避免 API 直接依賴資料庫實作
- Domain 保留核心商業資料模型
- Infrastructure 負責 EF Core、資料庫、Redis、JWT 等外部技術細節
- 未來若更換資料庫、調整快取或增加前端，不會大幅影響核心模型

## 業務範圍

此系統模擬企業 ERP / MIS 常見的進銷存流程。

- 商品主檔
- 倉庫主檔
- 供應商主檔
- 客戶主檔
- 採購單
- 收貨單
- 銷售單
- 出貨單
- 倉庫調撥
- 庫存台帳
- 庫存結餘
- 使用者登入與角色

## 核心流程

### 採購入庫

```text
建立採購單
  -> 建立收貨單
  -> 寫入庫存台帳，數量為正數
  -> 更新庫存結餘
  -> Transaction Commit
```

這個流程主要展示：

- 採購與收貨的業務流程拆分
- 入庫時同時保留歷史紀錄與目前庫存
- 使用交易確保台帳與結餘同步成功

### 銷售出貨

```text
建立銷售單
  -> 檢查庫存是否足夠
  -> 建立出貨單
  -> 寫入庫存台帳，數量為負數
  -> 更新庫存結餘
  -> Transaction Commit
```

這個流程主要展示：

- 出貨前檢查庫存
- 防止庫存扣成負數
- 出貨紀錄與庫存扣減同時完成

### 倉庫調撥

```text
檢查來源倉庫庫存
  -> 扣減來源倉庫庫存
  -> 增加目標倉庫庫存
  -> 寫入庫存台帳
  -> Transaction Commit
```

這個流程主要展示：

- 同一商品在不同倉庫間移動
- 避免只扣來源倉庫、卻沒有增加目標倉庫
- 使用交易避免調撥流程只完成一半

## 庫存設計

本專案將庫存拆成兩種資料模型：

```text
Stock Ledger       庫存台帳
Inventory Balance  庫存結餘
```

### Stock Ledger：庫存台帳

庫存台帳記錄每一筆庫存異動。

例如：

- 採購入庫：正數
- 銷售出貨：負數
- 倉庫調撥：來源倉庫扣減、目標倉庫增加

台帳的用途是保留歷史紀錄，方便追蹤每一筆庫存為什麼增加或減少。

### Inventory Balance：庫存結餘

庫存結餘記錄目前每個商品在每個倉庫的現有數量。

它的用途是快速查詢目前庫存，並在出貨或調撥前檢查數量是否足夠。

### 為什麼要同時設計台帳與結餘

如果只使用台帳，每次查詢目前庫存都需要加總大量歷史資料。  
如果只使用結餘，雖然查詢很快，但無法追蹤庫存變動原因。

因此本專案同時使用：

- 台帳：負責追蹤歷史
- 結餘：負責快速查詢目前數量

庫存異動時兩者必須在同一個交易中一起更新，避免資料不一致。

## 交易一致性

進銷存系統最重要的是資料一致性。  
本專案在入庫、出庫、調撥等會改變庫存的流程中，使用 EF Core Transaction 保護資料。

要避免的錯誤狀態：

- 台帳新增成功，但庫存結餘沒有更新
- 庫存結餘更新成功，但沒有留下台帳紀錄
- 出貨單建立成功，但庫存沒有正確扣除
- 調撥時來源倉庫扣庫存成功，但目標倉庫沒有增加

在服務層中，若外層流程已經開啟交易，庫存服務可以沿用既有交易；若沒有交易，則由庫存服務自己建立交易。

## 驗證與權限

本專案包含 JWT 登入驗證流程。

已實作項目：

- 使用者註冊
- 使用者登入
- JWT Token 產生
- ASP.NET Core JWT Bearer 驗證
- 登入回傳角色資訊
- Swagger 支援 Bearer Token 測試

設計目的：

- 將公開 API 與需要登入的 API 分開
- 讓後續功能可以依照角色限制操作權限
- 模擬企業系統常見的帳號權限控管

## Redis 快取

本專案整合 Redis 作為分散式快取。

設計方向是 Cache Aside Pattern：

```text
先查快取
  -> 快取不存在時查資料庫
  -> 將資料寫回快取
```

在正式系統中，庫存異動、商品資料異動、供應商或客戶資料更新時，需要搭配明確的快取失效策略。

## Docker Compose

專案提供 Docker Compose，方便快速建立本機開發環境。

包含服務：

- ERP API
- SQL Server 2022
- Redis

啟動方式：

```bash
docker compose up -d
```

Swagger：

```text
http://localhost:8080/swagger
```

注意：目前 `docker-compose.yml` 是開發環境設定。正式環境應將資料庫密碼、JWT Key 等敏感資訊移到 `.env`、User Secrets 或部署平台環境變數。

## 技術面試官建議閱讀順序

如果想快速理解專案架構，建議依照以下順序看：

1. `ERP.API/Program.cs`
   - DI 註冊
   - JWT 設定
   - Swagger 設定
   - Middleware
   - EF Core Migration 啟動流程

2. `ERP.API/Controllers`
   - API 路由設計
   - Controller 是否只處理 HTTP 層
   - Request / Response DTO 使用方式

3. `ERP.Application`
   - DTO 定義
   - Service Interface
   - Application 與 Infrastructure 的邊界

4. `ERP.Domain/Entities`
   - 商品、倉庫、單據、庫存相關 Entity
   - 業務資料模型關係

5. `ERP.Infrastructure/Services`
   - 採購流程
   - 收貨流程
   - 銷售流程
   - 出貨流程
   - 庫存入庫與出庫邏輯
   - 調撥邏輯

6. `ERP.Infrastructure/Persistence/AppDbContext.cs`
   - EF Core DbContext
   - Entity 關係設定
   - 資料庫模型

## 重點檔案

```text
ERP.API/Program.cs
ERP.Infrastructure/Persistence/AppDbContext.cs
ERP.Infrastructure/Services/InventoryService.cs
ERP.Infrastructure/Services/AuthService.cs
ERP.Infrastructure/Services/PurchaseOrderService.cs
ERP.Infrastructure/Services/GoodsReceiptService.cs
ERP.Infrastructure/Services/SalesOrderService.cs
ERP.Infrastructure/Services/ShipmentService.cs
ERP.Infrastructure/Services/TransferService.cs
ERP.Domain/Entities
ERP.Application/DTOs
```

## 可討論的技術重點

面試時我可以針對以下部分進一步說明：

- 為什麼庫存要分成台帳與結餘
- 出貨時如何避免負庫存
- 入庫、出庫、調撥如何透過 Transaction 保持一致
- 為什麼 Controller 不直接操作 DbContext
- DTO 與 Entity 分離的原因
- JWT 驗證流程如何接進 ASP.NET Core
- Redis 快取適合放在哪些查詢
- 若要上正式環境，哪些地方需要強化
- 如果往 MES 延伸，可以如何加入工單、BOM 與生產入庫

## 已知限制與後續改進

這是作品專案，仍有一些地方可以再接近正式系統。

- 將 `docker-compose.yml` 中的敏感資訊移到環境變數
- 密碼雜湊改用 salted hash、PBKDF2、bcrypt 或 ASP.NET Core Identity
- 增加 Refresh Token
- 增加 Request Validation
- 增加統一錯誤回應格式
- 增加更多庫存流程整合測試
- 增加高併發庫存異動的 RowVersion 或樂觀鎖設計
- 增加分頁、排序與查詢條件標準化
- 增加 CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 等稽核欄位
- 增加 structured logging
- 增加 CI/CD pipeline

## MES / ERP 延伸方向

此專案目前以進銷存與庫存為核心，若往 MES 或製造業資訊系統延伸，可以加入：

- 工單管理
- BOM 表
- 生產領料
- 成品入庫
- 在製品庫存
- 產線狀態追蹤
- 不良品與返工紀錄
- 設備狀態整合
- 條碼掃描與物料移動紀錄

這些延伸方向可以建立在目前的商品、倉庫、庫存台帳與庫存結餘基礎上。
