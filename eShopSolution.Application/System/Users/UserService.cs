using eShopSolution.Data.Entities;
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
        private readonly SignInManager<AppUser> _signInManager; // thằng này phải cài nuget mới có này
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
                return new ApiErrorResult<string>("Tài Khoản Không Tồn Tại");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true); // bằng true là cùa lockoutOnfilure là khi login nhiều quá ta sẽ khóa tài khoản lại
            if (!result.Succeeded)// NẾUn ko đăng nhập thành công
            {
                return new ApiErrorResult<string>("Mật Khẩu Tài Khoản Không Đúng"); // trả về một object error
            }
            var roles = await _userManager.GetRolesAsync(user); // lấy một list role của user
            var claims = new[] //claims yêu cầu trả lại,quyền đòi hỏi
            {
                new Claim(ClaimTypes.GivenName,user.FirstName),
                 new Claim(ClaimTypes.Email,user.Email),
                 new Claim(ClaimTypes.Role,string.Join(";",roles)), // string.Join là nối tất cả các dối dượng lại thành một chuối cách nhau bới giấu ;
                 new Claim(ClaimTypes.Name,request.UserName)
            }; 

            // bắt đầu mã hóa Claim bằng Symmetric vào appseting của Api cài đặt
            //bắt đầu mã hóa token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])); // lấy từ cấu hình appseting theo Tokens và "Key": "0123456789ABCDEF",
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Tokens:Issuer"],  // "Issuer": "https://thanglong.edu.vn/"
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3), // token hết hạn sau 3 tiếng
                signingCredentials: creds); // ký thông tin xác thực

            // vậy là sau khi đăng nhập ta có 1 token được mã hóa , nội dụng token có : thông tin ( FirstName,Email,UserName,role) và key,Issuer

            //ApiSuccessResult là con của ApiResult nên có thể trả về con nó
            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));  // rồi chả về một chuỗi token
        }

        //  DELETE user
        public async Task<ApiResult<bool>> Delete(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();

            return new ApiErrorResult<bool>("Xóa Không Thành Công");
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("Tài Khoản không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user); // lấy ra role của user
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.DOB,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName,
                Roles = roles
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        // phương thức lấy ra danh sách user     (nó sẽ lấy ra user và trả về một model phân trang)
        public async Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(GetUserPagingRequest request)  // có nhận keyword từ request nhe vào trong nó mà xem
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.KeyWord))// chỉ string mới được sử dụng phương thức này nhe
            {
                // Tìm kiếm bằng keyword hiện tại ta đang cho shearch bằng user name và phoneNumgber ta có thẻ cho thêm ở đây nhe
                query = query.Where(x => x.UserName.Contains(request.KeyWord)
                || x.PhoneNumber.Contains(request.KeyWord)
                || x.Email.Contains(request.KeyWord)
                || x.LastName.Contains(request.KeyWord)
                || x.FirstName.Contains(request.KeyWord));
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
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>(result.Errors.ToList());
        }

        // phương thức Role
        public async Task<ApiResult<bool>> RoleAssign(Guid Id, RoleAssignRequest request)
        {
            // lấy user
            var user = await _userManager.FindByIdAsync(Id.ToString()); // tìm thoe id mà gặp phát lấy ra luân rồi không tìm tiếp  // co nghĩa là chỉ 1
            if (user == null) // nếu user không tồn tại
            {
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            }

            // selected bằng false là đã tích hay chưa // có nghĩa là khi ta nhán checkbox không còn tích thì nó sẽ có Selected bằng false và xóa quyền ,kể cả chưa tích cũng xóa
            var removedRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList(); //vào danh sách roles lấy ra thằng nào không Selected có nghĩa là bằng false và lấy cái name của thằng đấy ra
                                                                                                           // nhớ là Roles  ,chuyền vào user và danh sách các role

            foreach (var roleName in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)  // user có có roleName
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName); // RemoveFromRolesAsync nó tính theo Name lên phải lấy Name ra
                }
            }

            // trong trường hợp thằng nào đã có tồn tại , có nghĩa là khi ta tích vào check box nó sẽ cho Selected bằng true
            var addedRoles = request.Roles.Where(x => x.Selected == true).Select(x => x.Name).ToList();
            foreach (var roleName in addedRoles)
            {
                // bắt buộc chưa nằm trong cái role mới add vào role
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName); // ở đay là add từng role nên không có s nhe
                }
            }

            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            // Users lấy ra tât vả user
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))  // xem bất kì thằng nào thảo mã thằng này không
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