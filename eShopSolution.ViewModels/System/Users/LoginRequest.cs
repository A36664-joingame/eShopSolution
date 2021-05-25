using System;
using System.Collections.Generic;
using System.Text;

// khi login các Request từ database sẽ được kéo nên
namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequest
    {
        // cách 1: Validation  nhưng nó lại gắn chặt với thuộc tính nên ko trực qun phải sử dụng cách thứ 2
        // [Required(ErrorMessage = "User Name RequiredS")]

        // cách 2
        // cài đặt nuget:FluentValidation.AspNetCore
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}