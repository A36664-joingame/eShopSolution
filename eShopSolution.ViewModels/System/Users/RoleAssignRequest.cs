using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RoleAssignRequest
    {
        public Guid Id { get; set; }

        // ở đây ta sẽ có 1 list SelectItem
        public List<SelectItem> Roles { get; set; } = new List<SelectItem>(); // để cho nó không bị null
    }
}