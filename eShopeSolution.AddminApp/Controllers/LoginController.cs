﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using eShopeSolution.AddminApp.Services;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace eShopeSolution.AddminApp.Controllers
{
    public class LoginController : Controller
    {
        // tiếm tục tiêm Di
        private readonly IUserApiClient _userApiClient;

        private readonly IConfiguration _configuration;

        public LoginController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        // đưa phương thức login lên cho view

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // vào trang login là ta logout những section(phần) cũ đi
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request) // add refrence project model vào nhe
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            var result = await _userApiClient.Authenticate(request);  // đó lấy ra được token ở request rồi

            // giải mã token ra
            var userPrincipal = this.ValidateToken(result.ResultObj); // chuyền token sang UserPrincipal
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            var authProperties = new AuthenticationProperties // lấy tập Properties của cookies
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false  // là khi đăng nhập rồi mà tắt chương trình nó vẫn đăng nhập khi chạy lại
            };

            HttpContext.Session.SetString("Token", result.ResultObj);  //phải add thêm modul token vào trong startup của project AdminApp

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal,
                    authProperties);

            return RedirectToAction("Index", "Home");  // đi đến Index trong thư mục Home
        }

        // hàm giải mã token
        private ClaimsPrincipal ValidateToken(string jwToken)
        {
            // vào đây mà đọc
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;
            // nhớ lấy cái phần Token cảu appSetting Api coppy sang AdminApp nhe
            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwToken, validationParameters, out validatedToken);
            return principal;
        }
    }
}