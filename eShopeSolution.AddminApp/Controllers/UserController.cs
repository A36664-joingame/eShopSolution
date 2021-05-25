using eShopSolution.AdminApp.Controllers;
using eShopSolution.ApiIntergration;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eShopeSolution.AddminApp.Controllers
{
    public class UserController : BaseController  // đương nhiên nó sẽ chạy vào BaseController trước check rồi mới suống dưới thực hiện vì kế thừa song mới suống dưới
    {
        // tiếm tục tiêm Di
        private readonly IUserApiClient _userApiClient;

        private readonly IConfiguration _configuration;

        private readonly IRoleApiClient _roleApiClient;

        public UserController(IUserApiClient userApiClient, IConfiguration configuration, IRoleApiClient roleApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _roleApiClient = roleApiClient;
        }

        //ĐÂY LÀ TÌM KIẾM VƠI KEYWORD BÊN Default đã thự hiện cộng key word nếu thấy để gữ lại đối tượng khi tìm thấy mà nó vẫn sử dụng được phân trang ,chỉ khi reset nó mới ko cộng keyword sang Default mà xem nhe
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 6) // gán mặc định nếu không có giá trị nào ,pageSize được tính rồi nhe tính cho phân trang
        {
            //phải add 1 view là viewName Index ,Template List ,Model class UserVm , use Layout chúng có sẵn tải về

            //từ Token này ta phải ra một cái Request
            var request = new GetUserPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                KeyWord = keyword
            };
            var data = await _userApiClient.GetUsersPagings(request);
            // để giữ lại giá trị hiển thị tren ô tìm kiếm ta dùng viewBag chuyền về cho view
            if (keyword != null)
                ViewBag.keyword = keyword;// tự động view sẽ nhận được

            if (TempData["result"] != null)
            {
                //bên này đã nhận được TempData đấy  //  ta sẽ show nó bên view
                ViewBag.SuccessMsg = TempData["result"];
            }

            return View(data.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid Id) // lấy ra Id nhớ Index cũng phải có Id nhe
        {
            // add View với View Name= Details , Template=Details , Model class=UserVm , và layout dùng chung

            var result = await _userApiClient.GetById(Id);
            return View(result.ResultObj);
        }

        // đăng kí tài khoản biding từ
        [HttpGet]
        public IActionResult Create()
        {
            // tạo View với View Name : Create , Template Create ,Model là RegisterRequest và layout dùng chung
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.RegisterUser(request);
            if (result.IsSuccessed)
            {
                // khi thành công ta có thể tạo một TempData  đây là đầu đi và sẽ có đầu nhận dữ liệu này nhe bên View Của nó
                TempData["result"] = "Thêm Thành Công"; //có key là result
                return RedirectToAction("Index");  // nếu thành công thì chuyển tới phân trang là Index
            }

            ModelState.AddModelError("", result.Message);// Message trên Api nó chuyền suống được
            return View(request);//nếu ko thành công ta chả về request để xem request
        }

        [HttpGet] // lấy cho người dùng xem
        public async Task<IActionResult> Edit(Guid id)// lấy ra Id nhớ Index cũng phải có Id nhe
        {
            //add view : ViewName=Edit Template=Edit ModelClass= UserUpdate  chọn layout chung

            var result = await _userApiClient.GetById(id); //nhớ viết thêm getById bên thằng UserController của Api
            if (result.IsSuccessed)
            {
                var user = result.ResultObj;
                var updateRequest = new UserUpdateRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Id = id
                };
                return View(updateRequest); // nếu thành công thì chả về đối tượng User được update luân
            }
            return RedirectToAction("Error", "Home"); // nếu không trả về trang Error của Home
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.UpdateUser(request.Id, request);
            if (result.IsSuccessed)
            {
                // khi thành công ta có thể tạo một TempData  đây là đầu đi và sẽ có đầu nhận dữ liệu này nhe bên View Của nó
                TempData["result"] = "Sửa Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);  // đây là lỗi của Model này
            return View(request);
        }

        // nhớ cài đặt bên layout thằng action này thì mới logout được nhe
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);// thực hiện Logout
            //phải xóa thằng token đi để nó ko lưu thằng cũ khi đăng nhập lại mà chúng ta sẽ sử dụng token mới khi đăng nhập cũ xóa đi
            HttpContext.Session.Remove("Token"); // 30' Token chưa hết thì Sesstion đã hết

            return RedirectToAction("Index", "Login"); // logout song đi đén Action là Index và Controller là Login
        }

        // delete user
        [HttpGet]
        public IActionResult Delete(Guid id)  // lấy id của đối tựng cần xóa về
        {
            return View(new UserDeleteRequest()
            {
                Id = id,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDeleteRequest request)  // lấy request từ thằng backeEnd thông qua thằng UserApiClient
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.Delete(request.Id);// với id lấy về trên kia trong request và thực hiện xóa
            if (result.IsSuccessed)
            {
                // khi thành công ta có thể tạo một TempData  đây là đầu đi và sẽ có đầu nhận dữ liệu này nhe bên View Của nó
                TempData["result"] = "Xóa Thành Công"; //có key là result
                return RedirectToAction("Index");  // nếu thành công thì chuyển tới phân trang là Index
            }

            ModelState.AddModelError("", result.Message);// Message trên Api nó chuyền suống được
            return View(request);//nếu ko thành công ta chả về request để xem request
        }

        [HttpGet]
        public async Task<IActionResult> RoleAssign(Guid id) // chuyền id từ index về đây nhờ phương thức HttpGet
        {
            // add view có VIew name là RoleAssign Template là Edit và Model clas RoleAssignRequest à layout dùng chung

            var roleAssignRequest = await GetRoleAssignRequest(id);

            return View(roleAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.RoleAssign(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Thêm Quyền Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);  // đây là lỗi của Model này

            var roleAssignRequest = await GetRoleAssignRequest(request.Id); // nếu trong trường hợp bị false nó vẫn lấy được về
            return View(roleAssignRequest);
        }

        private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid id)
        {
            var userObj = await _userApiClient.GetById(id);

            // chúng ta phải lấy ra được danh sách role
            var roleObj = await _roleApiClient.GetAll();  // trả về một danh sách ApiResult<List<RoleVm>> ,ta đưa vào list<RoleVm> nên thằng ResultObj sẽ là list<RoleVm>

            var roleAssignRequest = new RoleAssignRequest();
            foreach (var role in roleObj.ResultObj)  // ResultObj nằm trong ApiResult
            {
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Selected = userObj.ResultObj.Roles.Contains(role.Name) // Contains xem nó có chứa Name của role không,nếu có nó sẽ là true không là false
                });
            }

            return roleAssignRequest;
        }
    }
}