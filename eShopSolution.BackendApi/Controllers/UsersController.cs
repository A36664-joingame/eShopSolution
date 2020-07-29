﻿using System;
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
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resultToken = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("Username or password is incorrect");
            }

            return Ok(resultToken);
        }

        [HttpPost]
        [AllowAnonymous] // chưa đăng nhập vẫn gọi được cái này
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result)
            {
                return BadRequest("Regisster is unsuccessful");
            }

            return Ok();
        }

        // http://localhost/api/users/paging?pageIndex=1&pageSize=10&keyWord=   ví dụ đường dẫn
        [HttpGet("paging")] // đặt alias(bí danh) cho nó để không trùng với thằng ở trên chỉ dành cho HttpGet
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request) // tất cả tham số đều lấy từ query ra khi ta để FromQuery
        {                                            // ĐẺ FROM QUERY ĐỂ BÊN UserApiClient ta biding từ phương thức GetUsersPagings vào
            var Users = await _userService.GetUserPaging(request); // thằng request này có languageId rồi nhe
            return Ok(Users);
        }
    }
}