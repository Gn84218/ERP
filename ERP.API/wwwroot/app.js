const state = {
  token: localStorage.getItem("erpToken") || "",
  lastInventoryAction: "stock-in",
  products: [],
  customers: [],
  suppliers: [],
  warehouses: [],
  purchaseOrders: [],
  receipts: [],
  salesOrders: [],
  transfers: []
};

const routes = {
  products: "/api/products/%E6%9F%A5%E8%A9%A2%E5%88%86%E9%A0%81?page=1&pageSize=100",
  createProduct: "/api/products/%E6%96%B0%E5%A2%9E%E5%95%86%E5%93%81",
  customers: "/api/customers/%E7%8D%B2%E5%8F%96%E5%AE%A2%E6%88%B6%E8%B3%87%E6%96%99",
  createCustomer: "/api/customers/%E6%96%B0%E5%A2%9E%E5%AE%A2%E6%88%B6",
  suppliers: "/api/suppliers/%E7%8D%B2%E5%8F%96%E4%BE%9B%E6%87%89%E5%95%86%E5%90%8D%E5%96%AE",
  createSupplier: "/api/suppliers/%E6%96%B0%E5%A2%9E%E4%BE%9B%E6%87%89%E5%95%86",
  warehouses: "/api/warehouses/%E7%8D%B2%E5%8F%96%E5%80%89%E5%BA%AB%E5%90%8D%E5%96%AE",
  createWarehouse: "/api/warehouses/%E6%96%B0%E5%A2%9E%E5%80%89%E5%BA%AB",
  ledger: "/api/inventory/ledger/%E6%9F%A5%E5%BA%AB%E5%AD%98%E5%8F%B0%E5%B8%B3",
  stockIn: "/api/inventory/stock-in/%E5%85%A5%E5%BA%AB",
  stockOut: "/api/inventory/stock-out/%E5%87%BA%E5%BA%AB",
  purchases: "/api/purchase-orders/%E6%9F%A5%E8%A9%A2",
  createPurchase: "/api/purchase-orders/%E6%96%B0%E5%A2%9E%E6%8E%A1%E8%B3%BC%E5%96%AE",
  purchaseDetail: (id) => `/api/purchase-orders/${id}/%E7%8D%B2%E5%8F%96%E6%8E%A1%E8%B3%BC%E5%96%AE%E6%98%8E%E7%B4%B0`,
  approvePurchase: (id) => `/api/purchase-orders/${id}/approve/%E6%A0%B8%E5%87%86`,
  receipts: "/api/goods-receipts/%E6%9F%A5%E8%A9%A2",
  createReceipt: "/api/goods-receipts/%E5%BB%BA%E7%AB%8B%E6%94%B6%E6%93%9A%E5%96%AE(%E8%A1%A8+%E6%98%8E%E7%B4%B0)",
  receiptDetail: (id) => `/api/goods-receipts/${id}/%E7%8D%B2%E5%8F%96%E6%98%8E%E7%B4%B0`,
  postReceipt: (id) => `/api/goods-receipts/${id}/post/%E9%81%8E%E5%B8%B3(%E8%87%AA%E5%8B%95%E5%85%A5%E5%BA%AB)`,
  sales: "/api/sales-orders/%E6%9F%A5%E8%A9%A2",
  createSales: "/api/sales-orders/%E6%96%B0%E5%A2%9E%E9%8A%B7%E5%94%AE%E5%96%AE",
  salesDetail: (id) => `/api/sales-orders/${id}/%E9%8A%B7%E5%94%AE%E5%96%AE%E6%98%8E%E7%B4%B0`,
  approveSales: (id) => `/api/sales-orders/${id}/approve/%E6%A0%B8%E5%87%86`,
  transfers: "/api/transfers/%E6%9F%A5%E8%A9%A2",
  createTransfer: "/api/transfers/%E6%96%B0%E5%A2%9E%E8%AA%BF%E6%92%A5%E5%96%AE",
  transferDetail: (id) => `/api/transfers/${id}/%E7%8D%B2%E5%8F%96%E8%AA%BF%E6%92%A5%E5%96%AE%E6%98%8E%E7%B4%B0`,
  postTransfer: (id) => `/api/transfers/${id}/post/%E8%AA%BF%E6%92%A5%E5%96%AE%E6%A0%B8%E5%87%86`
};

const titles = {
  dashboard: ["總覽", "查看 ERP 基本資料與作業狀態"],
  products: ["商品", "建立與查詢商品主檔"],
  purchase: ["採購單", "建立、查看明細與核准採購單"],
  receipts: ["收貨單", "建立、查看明細與過帳收貨單"],
  sales: ["銷售單", "建立、查看明細與核准銷售單"],
  transfers: ["調撥單", "建立、查看明細與過帳調撥單"],
  inventory: ["庫存", "庫存異動與台帳查詢"],
  customers: ["客戶", "客戶主檔資料"],
  suppliers: ["供應商", "供應商主檔資料"],
  warehouses: ["倉庫", "倉庫主檔資料"]
};

const $ = (selector) => document.querySelector(selector);

function showMessage(text, isError = false) {
  const el = $("#message");
  if (!el) return;
  el.textContent = text || "完成。";
  el.classList.toggle("error", isError);
  el.classList.remove("hidden");
  clearTimeout(showMessage.timer);
  showMessage.timer = setTimeout(() => el.classList.add("hidden"), 5000);
}

async function api(path, options = {}) {
  const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
  if (state.token) headers.Authorization = `Bearer ${state.token}`;
  const response = await fetch(path, { ...options, headers });
  const text = await response.text();
  let data = null;
  try {
    data = text ? JSON.parse(text) : null;
  } catch {
    data = { message: text };
  }
  if (!response.ok) {
    throw new Error(data?.message || data?.title || data?.error || `HTTP ${response.status}`);
  }
  return data;
}

function formToObject(form) {
  return Object.fromEntries(new FormData(form).entries());
}

function setLoginState() {
  const loggedIn = Boolean(state.token);
  document.body.classList.toggle("logged-in", loggedIn);
  $("#loginState").textContent = loggedIn ? "已登入" : "尚未登入";
  $("#loginState").classList.toggle("ok", loggedIn);
  $("#logoutBtn").classList.toggle("hidden", !loggedIn);
}

function statusText(value) {
  return value ? "啟用" : "停用";
}

function renderRows(target, rows, mapper, columns) {
  const body = $(target);
  if (!body) return;
  if (!Array.isArray(rows) || rows.length === 0) {
    body.innerHTML = `<tr><td class="empty" colspan="${columns}">目前沒有資料</td></tr>`;
    return;
  }
  body.innerHTML = rows.map(mapper).join("");
}

function fillSelects(source, rows, labeler) {
  document.querySelectorAll(`select[data-source="${source}"]`).forEach((select) => {
    const current = select.value;
    const empty = select.dataset.emptyLabel ? `<option value="">${select.dataset.emptyLabel}</option>` : "";
    select.innerHTML = empty + rows.map((item) => `<option value="${item.id}">${labeler(item)}</option>`).join("");
    if (rows.some((item) => item.id === current)) select.value = current;
  });
}

function nameById(rows, id) {
  const item = rows.find((row) => row.id === id);
  return item ? (item.name || item.sku || item.code || id) : id;
}

function productName(line) {
  return line.productName || nameById(state.products, line.productId);
}

function formatDate(value) {
  if (!value) return "";
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString();
}

async function loadProducts() {
  const rows = await api(routes.products);
  state.products = rows;
  fillSelects("products", rows, (item) => `${item.name} (${item.sku})`);
  renderRows("#productsTable", rows, (item) => `
    <tr><td>${item.sku || ""}</td><td>${item.name || ""}</td><td>${item.cost ?? ""}</td><td>${item.price ?? ""}</td><td>${statusText(item.isActive)}</td></tr>
  `, 5);
  $("#productCount").textContent = rows.length;
  return rows;
}

async function loadCustomers() {
  const rows = await api(routes.customers);
  state.customers = rows;
  fillSelects("customers", rows, (item) => `${item.name} (${item.code})`);
  renderRows("#customersTable", rows, (item) => `<tr><td>${item.code || ""}</td><td>${item.name || ""}</td><td>${statusText(item.isActive)}</td></tr>`, 3);
  $("#customerCount").textContent = rows.length;
  return rows;
}

async function loadSuppliers() {
  const rows = await api(routes.suppliers);
  state.suppliers = rows;
  fillSelects("suppliers", rows, (item) => `${item.name} (${item.code})`);
  renderRows("#suppliersTable", rows, (item) => `<tr><td>${item.code || ""}</td><td>${item.name || ""}</td><td>${statusText(item.isActive)}</td></tr>`, 3);
  $("#supplierCount").textContent = rows.length;
  return rows;
}

async function loadWarehouses() {
  const rows = await api(routes.warehouses);
  state.warehouses = rows;
  fillSelects("warehouses", rows, (item) => `${item.name} (${item.code})`);
  renderRows("#warehousesTable", rows, (item) => `<tr><td>${item.code || ""}</td><td>${item.name || ""}</td><td>${statusText(item.isActive)}</td></tr>`, 3);
  $("#warehouseCount").textContent = rows.length;
  return rows;
}

function orderStatus(status, doneText) {
  return Number(status) === 2 || String(status) === doneText ? doneText : "草稿";
}

function linesTable(lines = [], qtyKey, priceKey, priceTitle) {
  if (!Array.isArray(lines) || lines.length === 0) return `<span class="muted">尚未載入明細</span>`;
  return `
    <div class="table-wrap">
      <table>
        <thead><tr><th>商品</th><th>數量</th><th>${priceTitle}</th></tr></thead>
        <tbody>${lines.map((line) => `<tr><td>${productName(line)}</td><td>${line[qtyKey] ?? ""}</td><td>${line[priceKey] ?? ""}</td></tr>`).join("")}</tbody>
      </table>
    </div>
  `;
}

function transferLinesTable(lines = []) {
  if (!Array.isArray(lines) || lines.length === 0) return `<span class="muted">尚未載入明細</span>`;
  return `<div class="table-wrap"><table><thead><tr><th>商品</th><th>數量</th></tr></thead><tbody>${lines.map((line) => `<tr><td>${productName(line)}</td><td>${line.qty ?? ""}</td></tr>`).join("")}</tbody></table></div>`;
}

function renderOrderCards(target, rows, config) {
  const list = $(target);
  if (!list) return;
  if (!rows.length) {
    list.innerHTML = `<div class="record-card"><span class="muted">目前沒有資料</span></div>`;
    return;
  }
  list.innerHTML = rows.map((item) => {
    const status = config.status(item.status);
    const done = status === config.doneText;
    return `
      <article class="record-card">
        <div class="record-head">
          <div><strong>${item.no || config.title}</strong><small>ID：${item.id}</small>${config.meta(item)}</div>
          <span class="status-tag ${done ? "done" : ""}">${status}</span>
        </div>
        <div class="record-actions">
          <button class="ghost-button" data-detail="${item.id}" type="button">明細</button>
          <button data-action="${item.id}" type="button" ${done ? "disabled" : ""}>${config.actionText}</button>
        </div>
      </article>
    `;
  }).join("");
}

function ensureDetailModalRoot() {
  const modal = $("#detailModal");
  if (modal && modal.parentElement !== document.body) {
    document.body.appendChild(modal);
  }
}

function openDetailModal(title, subtitle, content) {
  ensureDetailModalRoot();
  $("#detailModalTitle").textContent = title;
  $("#detailModalSubtitle").textContent = subtitle || "";
  $("#detailModalBody").innerHTML = content || `<span class="muted">尚未載入明細</span>`;
  $("#detailModal").classList.remove("hidden");
  document.body.classList.add("modal-open");
}

function closeDetailModal() {
  $("#detailModal").classList.add("hidden");
  document.body.classList.remove("modal-open");
}

async function loadPurchaseOrders() {
  state.purchaseOrders = await api(routes.purchases);
  renderPurchaseOrders();
}

async function loadReceipts() {
  state.receipts = await api(routes.receipts);
  renderReceipts();
}

async function loadSalesOrders() {
  state.salesOrders = await api(routes.sales);
  renderSalesOrders();
}

async function loadTransfers() {
  state.transfers = await api(routes.transfers);
  renderTransfers();
}

async function loadLedger() {
  const params = new URLSearchParams({ page: "1", pageSize: "50" });
  const productId = $("#ledgerProductId")?.value;
  const warehouseId = $("#ledgerWarehouseId")?.value;
  const refNo = $("#ledgerRefNo")?.value.trim();
  if (productId) params.set("productId", productId);
  if (warehouseId) params.set("warehouseId", warehouseId);
  if (refNo) params.set("refNo", refNo);
  const data = await api(`${routes.ledger}?${params.toString()}`);
  const rows = Array.isArray(data) ? data : data?.items || [];
  renderRows("#ledgerTable", rows, (item) => `<tr><td>${formatDate(item.txnAtUtc)}</td><td>${item.productName || nameById(state.products, item.productId)}</td><td>${item.warehouseName || nameById(state.warehouses, item.warehouseId)}</td><td>${item.txnType ?? ""}</td><td>${item.qty ?? ""}</td><td>${item.refNo || ""}</td></tr>`, 6);
}

function makeOrderView(target, formId, title, fieldsHtml, lineHtml, cardsId, refreshId, lookupId, lookupBtn) {
  $(target).innerHTML = `
    <div class="operation-grid">
      <form id="${formId}" class="panel"><h2>${title}</h2>${fieldsHtml}<div class="line-editor"><h3>明細</h3>${lineHtml}</div><button type="submit">${title}</button></form>
      <div class="panel table-panel">
        <div class="panel-title"><h2>單據列表</h2><div class="lookup-row"><input id="${lookupId}" placeholder="輸入單據 ID"><button id="${lookupBtn}" class="ghost-button" type="button">查詢</button></div></div>
        <button id="${refreshId}" class="ghost-button full-button" type="button">重新整理</button>
        <div id="${cardsId}" class="record-list"></div>
      </div>
    </div>
  `;
}

function makeMasterView(target, formId, title, tableBodyId, refreshId) {
  $(target).innerHTML = `
    <form id="${formId}" class="panel compact-form"><h2>新增${title}</h2><label>代碼<input name="code" required></label><label>名稱<input name="name" required></label><button type="submit">新增</button></form>
    <div class="panel table-panel"><div class="panel-title"><h2>${title}</h2><button id="${refreshId}" class="ghost-button" type="button">重新整理</button></div><div class="table-wrap"><table><thead><tr><th>代碼</th><th>名稱</th><th>狀態</th></tr></thead><tbody id="${tableBodyId}"></tbody></table></div></div>
  `;
}

function buildViews() {
  makeOrderView("#purchaseView", "purchaseForm", "新增採購單", `<label>供應商<select name="supplierId" data-source="suppliers" required></select></label>`, `<label>商品<select name="productId" data-source="products" required></select></label><label>數量<input name="qty" type="number" min="1" step="0.01" required></label><label>單價<input name="unitCost" type="number" min="0" step="0.01" required></label>`, "purchaseCards", "refreshPurchases", "purchaseLookupId", "lookupPurchase");
  makeOrderView("#receiptsView", "receiptForm", "新增收貨單", `<label>採購單 ID<input name="purchaseOrderId" required></label><label>倉庫<select name="warehouseId" data-source="warehouses" required></select></label>`, `<label>商品<select name="productId" data-source="products" required></select></label><label>實收數量<input name="receivedQty" type="number" min="1" step="0.01" required></label><label>單價<input name="unitCost" type="number" min="0" step="0.01" required></label>`, "receiptCards", "refreshReceipts", "receiptLookupId", "lookupReceipt");
  makeOrderView("#salesView", "salesForm", "新增銷售單", `<label>客戶<select name="customerId" data-source="customers" required></select></label>`, `<label>商品<select name="productId" data-source="products" required></select></label><label>數量<input name="qty" type="number" min="1" step="0.01" required></label><label>售價<input name="unitPrice" type="number" min="0" step="0.01" required></label>`, "salesCards", "refreshSales", "salesLookupId", "lookupSales");
  makeOrderView("#transfersView", "transferForm", "新增調撥單", `<label>來源倉庫<select name="fromWarehouseId" data-source="warehouses" required></select></label><label>目標倉庫<select name="toWarehouseId" data-source="warehouses" required></select></label>`, `<label>商品<select name="productId" data-source="products" required></select></label><label>數量<input name="qty" type="number" min="1" step="0.01" required></label>`, "transferCards", "refreshTransfers", "transferLookupId", "lookupTransfer");
  makeMasterView("#customersView", "customerForm", "客戶", "customersTable", "refreshCustomers");
  makeMasterView("#suppliersView", "supplierForm", "供應商", "suppliersTable", "refreshSuppliers");
  makeMasterView("#warehousesView", "warehouseForm", "倉庫", "warehousesTable", "refreshWarehouses");
}

async function refreshDashboard() {
  await Promise.all([loadProducts(), loadCustomers(), loadSuppliers(), loadWarehouses()]);
}

async function switchView(view) {
  document.querySelectorAll(".nav-item").forEach((btn) => btn.classList.toggle("active", btn.dataset.view === view));
  document.querySelectorAll(".view").forEach((panel) => panel.classList.remove("active"));
  $(`#${view}View`)?.classList.add("active");
  $("#pageTitle").textContent = titles[view][0];
  $("#pageSubtitle").textContent = titles[view][1];
  try {
    if (view === "dashboard") await refreshDashboard();
    if (view === "products") await loadProducts();
    if (view === "purchase") await Promise.all([loadProducts(), loadSuppliers()]).then(loadPurchaseOrders);
    if (view === "receipts") await Promise.all([loadProducts(), loadWarehouses()]).then(loadReceipts);
    if (view === "sales") await Promise.all([loadProducts(), loadCustomers()]).then(loadSalesOrders);
    if (view === "transfers") await Promise.all([loadProducts(), loadWarehouses()]).then(loadTransfers);
    if (view === "inventory") await Promise.all([loadProducts(), loadWarehouses()]).then(loadLedger);
    if (view === "customers") await loadCustomers();
    if (view === "suppliers") await loadSuppliers();
    if (view === "warehouses") await loadWarehouses();
  } catch (error) {
    showMessage(error.message, true);
  }
}

function bindCreateForm(selector, path, mapValues, afterSave) {
  $(selector).addEventListener("submit", async (event) => {
    event.preventDefault();
    try {
      await api(path, { method: "POST", body: JSON.stringify(mapValues(formToObject(event.currentTarget))) });
      event.currentTarget.reset();
      showMessage("已儲存。");
      await afterSave();
    } catch (error) {
      showMessage(error.message, true);
    }
  });
}

function upsert(list, record) {
  const index = list.findIndex((item) => item.id === record.id);
  if (index >= 0) list[index] = record;
  else list.unshift(record);
}

function bindLookup(buttonSelector, inputSelector, detailEndpoint, stateKey, render, loadAll) {
  $(buttonSelector).addEventListener("click", async () => {
    const id = $(inputSelector).value.trim();
    if (!id) {
      await loadAll();
      return;
    }
    try {
      const record = await api(detailEndpoint(id));
      upsert(state[stateKey], record);
      render();
      showMessage("已載入。");
    } catch (error) {
      showMessage(error.message, true);
    }
  });
}

function bindCardActions(targetSelector, detailEndpoint, actionEndpoint, stateKey, render, successText) {
  $(targetSelector).addEventListener("click", async (event) => {
    const detailId = event.target.dataset.detail;
    const actionId = event.target.dataset.action;
    try {
      if (detailId) {
        const record = await api(detailEndpoint(detailId));
        upsert(state[stateKey], record);
        render();
        openDetailModal(
          record.no || "單據明細",
          `ID：${record.id}`,
          getDetailContent(stateKey, record)
        );
      }
      if (actionId) {
        const record = await api(actionEndpoint(actionId), { method: "POST" });
        upsert(state[stateKey], record);
        render();
        showMessage(successText);
      }
    } catch (error) {
      showMessage(error.message, true);
    }
  });
}

function getDetailContent(stateKey, record) {
  if (stateKey === "transfers") return transferLinesTable(record.lines);
  if (stateKey === "receipts") return linesTable(record.lines, "receivedQty", "unitCost", "單價");
  if (stateKey === "salesOrders") return linesTable(record.lines, "qty", "unitPrice", "售價");
  return linesTable(record.lines, "qty", "unitCost", "單價");
}

function renderPurchaseOrders() {
  renderOrderCards("#purchaseCards", state.purchaseOrders, { title: "採購單", doneText: "Approved", status: (s) => orderStatus(s, "Approved"), actionText: "核准", meta: (x) => `<small>供應商：${nameById(state.suppliers, x.supplierId)}</small>`, lines: (x) => linesTable(x.lines, "qty", "unitCost", "單價") });
}

function renderReceipts() {
  renderOrderCards("#receiptCards", state.receipts, { title: "收貨單", doneText: "Posted", status: (s) => orderStatus(s, "Posted"), actionText: "過帳", meta: (x) => `<small>採購單：${x.purchaseOrderId}</small><small>倉庫：${nameById(state.warehouses, x.warehouseId)}</small>`, lines: (x) => linesTable(x.lines, "receivedQty", "unitCost", "單價") });
}

function renderSalesOrders() {
  renderOrderCards("#salesCards", state.salesOrders, { title: "銷售單", doneText: "Approved", status: (s) => orderStatus(s, "Approved"), actionText: "核准", meta: (x) => `<small>客戶：${nameById(state.customers, x.customerId)}</small>`, lines: (x) => linesTable(x.lines, "qty", "unitPrice", "售價") });
}

function renderTransfers() {
  renderOrderCards("#transferCards", state.transfers, { title: "調撥單", doneText: "Posted", status: (s) => orderStatus(s, "Posted"), actionText: "過帳", meta: (x) => `<small>來源倉：${nameById(state.warehouses, x.fromWarehouseId)}</small><small>目標倉：${nameById(state.warehouses, x.toWarehouseId)}</small>`, lines: (x) => transferLinesTable(x.lines) });
}

function bindEvents() {
  document.querySelectorAll(".nav-item").forEach((btn) => btn.addEventListener("click", () => switchView(btn.dataset.view)));
  document.querySelectorAll("[data-close-modal]").forEach((el) => el.addEventListener("click", closeDetailModal));
  $("#logoutBtn").addEventListener("click", () => {
    state.token = "";
    localStorage.removeItem("erpToken");
    setLoginState();
    showMessage("已登出。");
  });
  $("#loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    try {
      const data = await api("/api/auth/login", { method: "POST", body: JSON.stringify(formToObject(event.currentTarget)) });
      state.token = data.token || data.accessToken || "";
      localStorage.setItem("erpToken", state.token);
      setLoginState();
      showMessage("登入成功。");
      await switchView("dashboard");
    } catch (error) {
      showMessage(error.message, true);
    }
  });
  $("#registerForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    try {
      await api("/api/auth/register", { method: "POST", body: JSON.stringify(formToObject(event.currentTarget)) });
      event.currentTarget.reset();
      showMessage("註冊成功。");
    } catch (error) {
      showMessage(error.message, true);
    }
  });

  bindCreateForm("#productForm", routes.createProduct, (v) => ({ sku: v.sku, name: v.name, cost: Number(v.cost), price: Number(v.price) }), loadProducts);
  bindCreateForm("#customerForm", routes.createCustomer, (v) => v, loadCustomers);
  bindCreateForm("#supplierForm", routes.createSupplier, (v) => v, loadSuppliers);
  bindCreateForm("#warehouseForm", routes.createWarehouse, (v) => v, loadWarehouses);
  bindCreateForm("#purchaseForm", routes.createPurchase, (v) => ({ supplierId: v.supplierId, lines: [{ productId: v.productId, qty: Number(v.qty), unitCost: Number(v.unitCost) }] }), loadPurchaseOrders);
  bindCreateForm("#receiptForm", routes.createReceipt, (v) => ({ purchaseOrderId: v.purchaseOrderId, warehouseId: v.warehouseId, lines: [{ productId: v.productId, receivedQty: Number(v.receivedQty), unitCost: Number(v.unitCost) }] }), loadReceipts);
  bindCreateForm("#salesForm", routes.createSales, (v) => ({ customerId: v.customerId, lines: [{ productId: v.productId, qty: Number(v.qty), unitPrice: Number(v.unitPrice) }] }), loadSalesOrders);
  bindCreateForm("#transferForm", routes.createTransfer, (v) => ({ fromWarehouseId: v.fromWarehouseId, toWarehouseId: v.toWarehouseId, lines: [{ productId: v.productId, qty: Number(v.qty) }] }), loadTransfers);

  bindLookup("#lookupPurchase", "#purchaseLookupId", routes.purchaseDetail, "purchaseOrders", renderPurchaseOrders, loadPurchaseOrders);
  bindLookup("#lookupReceipt", "#receiptLookupId", routes.receiptDetail, "receipts", renderReceipts, loadReceipts);
  bindLookup("#lookupSales", "#salesLookupId", routes.salesDetail, "salesOrders", renderSalesOrders, loadSalesOrders);
  bindLookup("#lookupTransfer", "#transferLookupId", routes.transferDetail, "transfers", renderTransfers, loadTransfers);
  bindCardActions("#purchaseCards", routes.purchaseDetail, routes.approvePurchase, "purchaseOrders", renderPurchaseOrders, "已核准。");
  bindCardActions("#receiptCards", routes.receiptDetail, routes.postReceipt, "receipts", renderReceipts, "已過帳。");
  bindCardActions("#salesCards", routes.salesDetail, routes.approveSales, "salesOrders", renderSalesOrders, "已核准。");
  bindCardActions("#transferCards", routes.transferDetail, routes.postTransfer, "transfers", renderTransfers, "已過帳。");

  $("#inventoryForm").addEventListener("click", (event) => {
    const button = event.target.closest("button[data-action]");
    if (button) state.lastInventoryAction = button.dataset.action;
  });
  $("#inventoryForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const values = formToObject(event.currentTarget);
    const path = state.lastInventoryAction === "stock-out" ? routes.stockOut : routes.stockIn;
    try {
      await api(path, { method: "POST", body: JSON.stringify({ productId: values.productId, warehouseId: values.warehouseId, qty: Number(values.qty), refType: values.refType, refNo: values.refNo }) });
      event.currentTarget.reset();
      showMessage("已儲存。");
      await loadLedger();
    } catch (error) {
      showMessage(error.message, true);
    }
  });

  $("#refreshProducts").addEventListener("click", () => loadProducts().catch((e) => showMessage(e.message, true)));
  $("#refreshPurchases").addEventListener("click", () => loadPurchaseOrders().catch((e) => showMessage(e.message, true)));
  $("#refreshReceipts").addEventListener("click", () => loadReceipts().catch((e) => showMessage(e.message, true)));
  $("#refreshSales").addEventListener("click", () => loadSalesOrders().catch((e) => showMessage(e.message, true)));
  $("#refreshTransfers").addEventListener("click", () => loadTransfers().catch((e) => showMessage(e.message, true)));
  $("#refreshLedger").addEventListener("click", () => loadLedger().catch((e) => showMessage(e.message, true)));
  $("#refreshCustomers").addEventListener("click", () => loadCustomers().catch((e) => showMessage(e.message, true)));
  $("#refreshSuppliers").addEventListener("click", () => loadSuppliers().catch((e) => showMessage(e.message, true)));
  $("#refreshWarehouses").addEventListener("click", () => loadWarehouses().catch((e) => showMessage(e.message, true)));
}

buildViews();
setLoginState();
ensureDetailModalRoot();
bindEvents();
if (state.token) refreshDashboard().catch((error) => showMessage(error.message, true));
