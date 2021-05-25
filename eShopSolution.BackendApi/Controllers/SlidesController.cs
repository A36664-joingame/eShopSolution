using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.System.Roles;
using eShopSolution.Application.Utilities.Slides;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // cũng bắt buộc phải đăng nhập
    public class SlidesController : ControllerBase
    {
        // bắt đầu tiêm Di
        // nhớ đăng kí nhe bên startup của Api nhe vì nó là service
        private readonly ISlideService _slideService;

        public SlidesController(ISlideService slideService)
        {
            _slideService = slideService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _slideService.GetAll();
            return Ok(roles);
        }
    }
}