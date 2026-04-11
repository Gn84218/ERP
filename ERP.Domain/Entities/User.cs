using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }
}
