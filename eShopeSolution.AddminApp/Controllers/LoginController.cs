using eShopeSolution.Utilities.Constants;
using eShopSolution.ApiIntergration;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            {
                ModelState.AddModelError("", "Tài Khoản Và Mật Khẩu Không Chính Xác");
                return View();
            }

            var result = await _userApiClient.Authenticate(request);  // đó lấy ra được token ở request rồi

            //check lỗi khi đăng nhập sai mà khồn lấy dược token thì nó sẽ bị null nên phải check nó ở đây
            if (result.ResultObj == null)
            {
                ModelState.AddModelError("", result.Message); // riêng ModelState var khong thể chứa được nhe nhớ đấy
                return View();
            }

            // giải mã token ra
            var userPrincipal = this.ValidateToken(result.ResultObj); // chuyền token sang UserPrincipal

            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            var authProperties = new AuthenticationProperties //thuộc tính xác thực , lấy tập Properties của cookies
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false  // là khi đăng nhập rồi mà tắt chương trình nó vẫn đăng nhập khi chạy lại
            };

            HttpContext.Session.SetString(SystemConstants.AppSettings.DefaultLanguageId, _configuration[SystemConstants.AppSettings.DefaultLanguageId]); // nhớ phải thêm DefaultLanguageId vào trong appsetting của adminApp

            HttpContext.Session.SetString(SystemConstants.AppSettings.Token, result.ResultObj);  //phải add thêm modul token vào trong startup của project AdminApp

            // sign-in
            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, //scheme(kế hoạch)
                    userPrincipal, //principal (hiệu trưởng)
                    authProperties); //properties(tính chất)

            return RedirectToAction("Index", "Home");  // đi đến Index trong thư mục Home
        }

        // hàm giải mã token
        private ClaimsPrincipal ValidateToken(string jwToken)
        {
            // vào đây mà đọc
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            IdentityModelEventSource.ShowPII = true;   // show log lôiz khi chạy identity

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