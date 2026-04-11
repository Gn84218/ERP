using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities;
/*
 * 客戶主檔
 */
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;

    public bool IsActive { get; set; } = true;
}
