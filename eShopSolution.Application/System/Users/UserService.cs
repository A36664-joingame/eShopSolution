﻿using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public class UserService : IUserService
    {
        //UserManager và SignInManager nằm trong thư viện using Microsoft.AspNetCore.Identity;
        // phải config bên Startup của project Api nha như kiểu Di ý

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<ApiResult<string>> Authencate(LoginRequest request) // xác thực đăng nhập
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return null;

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true); // bằng true là cùa lockoutOnfilure là khi login nhiều quá ta sẽ khóa tài khoản lại
            if (!result.Succeeded)// NẾU đăng nhập thành công
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user); // lấy một list role của user
            var claims = new[] //claims yêu cầu trả lại
            {
                new Claim(ClaimTypes.GivenName,user.FirstName),
                 new Claim(ClaimTypes.Email,user.Email),
                 new Claim(ClaimTypes.Role,string.Join(";",roles)), // string.Join là nối tất cả các dối dượng lại thành một chuối cách nhau bới giấu ;
                 new Claim(ClaimTypes.Name,request.UserName)
            };

            // bắt đầu mã hóa Claim bằng Symmetric vào appseting của Api cài đặt

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.DOB,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        // phương thức lấy ra danh sách user     (nó sẽ lấy ra user và trả về một model phân trang)
        public async Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.KeyWord))// chỉ string mới được sử dụng phương thức này nhe
            {
                // tìm giống hệt product bằng keyWord
                query = query.Where(x => x.UserName.Contains(request.KeyWord)
                || x.PhoneNumber.Contains(request.KeyWord));
            }
            int totalRow = await query.CountAsync(); // lấy ra tông số số dòng để phân trang
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)// nếu PageIndex=2 và PageSize=20 thì bỏ qua 10 chỉ lấy 10 bẩn ghi hiện lên ko lấy tất để phù hợp với PageSize
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        // đăng kí user
        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            user = new AppUser()
            {
                DOB = request.DOB,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };

            var rusult = await _userManager.CreateAsync(user, request.Password);
            if (rusult.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đăng kí không thành công");
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            // Users lấy ra tât vả user
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.DOB = request.Dob;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }
    }
}