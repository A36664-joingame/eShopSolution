using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.System.Users;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        // bắt đâu tiêm Di nhớ đăng kí bên startup của APi nhe

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // BẮT ĐẦU THỰC HIỆN CÁC PHƯƠNG THỨC CỦA USER

        [HttpPost("authenticate")]
        [AllowAnonymous] // chưa đăng nhập vẫn gọi được cái này
        public async Task<IActionResult> Authencate([FromBody] LoginRequest request)  // phải dùng FromBody cho Template
        {  //Authencate(xác thực )
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // phương thức _userService có Authencate và Authencate khai báo kiểu dữ liệu ApiResult trong ApiResult có ResultObj nên Authencate có thê trả về ResultObj
            var resultToken = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(resultToken.ResultObj))  //ResultObj nằm trong Authencate rồi trong ApiResult
            {
                return BadRequest(resultToken);
            }

            return Ok(resultToken); // TRẢ ra resultToken nó sẽ post lên  [HttpPost("authenticate")]  đường đúng là http://localhost/api/users/authenticate
        }

        [HttpPost]
        [AllowAnonymous] // chưa đăng nhập vẫn gọi được cái này
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/users/id/roles
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // http://localhost/api/users/paging?pageIndex=1&pageSize=10&keyWord=   ví dụ đường dẫn
        [HttpGet("paging")] // đặt alias(bí danh) cho nó để không trùng với thằng ở trên chỉ dành cho HttpGet
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request) // tất cả tham số đều lấy từ query ra khi ta để FromQuery
        {                                            // ĐẺ FROM QUERY ĐỂ BÊN UserApiClient ta biding từ phương thức GetUsersPagings vào
            var Users = await _userService.GetUserPaging(request); // thằng request này có languageId rồi nhe
            return Ok(Users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        // Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.Delete(id);

            return Ok(result);  // trả về kết quả sử lý true hoặc một messge cho thằng frontEnd bắt request này
        }
    }
}