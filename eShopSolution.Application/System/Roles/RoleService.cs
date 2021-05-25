using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Roles
{
    public class RoleService : IRoleService
    {
        // bắt đầu tiêm Di nhớ là các thứ cảu Entities ta đã thêm 1 danh sách tất cả bên startup
        private RoleManager<AppRole> _RoleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _RoleManager = roleManager;
        }

        public async Task<List<RoleVm>> GetAll()
        {
            var roles = await _RoleManager.Roles.Select(x => new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).ToListAsync();

            return roles;
        }
    }
}