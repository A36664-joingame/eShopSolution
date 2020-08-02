﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// tải nuget :Swashbuckle.AspNetCore

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // bắt buộc thằng này phải đăng nhập có token thì mới vào    Nó hiển thị trong giao diện cảu Swagger nhe chỗ ổ khóa ý   bên user ta không để vì phải đăng nhập mới có token
    public class ProductsController : ControllerBase
    {
        // bắt đầu Di

        private readonly IProductService _ProductService;

        public ProductsController(IProductService productService)
        {
            _ProductService = productService;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request)
        {
            var products = await _ProductService.GetAllPaging(request);
            return Ok(products);
        }

        // CÁC PHƯƠNG THỨC CỦA MANAGE

        // SỬ DỤNG CÁC PHƯƠNG THỨC CẢU PRODUCT

        // http://localhost:port/product/1
        [HttpGet("{productId}/{languageId}")] // thằng này sẽ chuyền vào id cảu sản phẩm  trên ta lấy vd productId=1
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var Product = await _ProductService.GetById(productId, languageId);
            if (Product == null)
                return BadRequest("Cannot find Product");
            return Ok(Product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request) // tất cả các thuộc tính phải using form data
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productId = await _ProductService.Create(request); // thằng này chả về id nhe
            if (productId == 0)
                return BadRequest();// đây là nỗi 400

            // để có product chuyền về thì ta phải
            var product = await _ProductService.GetById(productId, request.LanguageId);
            // cách 1:
            //return Created(nameof(GetById), product);
            // cách 2:
            return CreatedAtAction(nameof(GetById), new { id = productId }, product); // Ok trả ra 200 còn created là 201  khi học jquery đã được học cái này
        }

        [HttpPut] // update tất cả phần
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request) // tất cả các thuộc tính phải using form data
        {
            var affectedResult = await _ProductService.Update(request); // thằng này chả về id nhe
            if (affectedResult == 0)
                return BadRequest("Cannot find Id");// đây là nỗi 400

            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId) //
        {
            var affectedResult = await _ProductService.Delete(productId); // thằng này chả về id nhe
            if (affectedResult == 0)
                return BadRequest();// đây là nỗi 400

            return Ok();
        }

        //  [HttpPut("price/{id}/{newPrice}")] //đây là update cả phần
        [HttpPatch("{productId}/{newPrice}")] // đây là update 1 phần
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice) // không cần fromquery nữa nếu để như thế này là bắt buộc
        {
            var isSuccessful = await _ProductService.UpdatePrice(productId, newPrice); // thằng này chả về id nhe
            if (isSuccessful == false)
                return BadRequest();// đây là nỗi 400
            return Ok();
        }

        // SỬ DỤNG CÁC PHƯƠNG THỨC CỦA IMAGE

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request) // tất cả các thuộc tính phải using form data
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageId = await _ProductService.AddImage(productId, request); // thằng này chả về id nhe
            if (imageId == 0)
                return BadRequest();// đây là nỗi 400

            // để có product chuyền về thì ta phải
            var image = await _ProductService.GetImageById(imageId);

            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image); // Ok trả ra 200 còn created là 201  khi học jquery đã được học cái này
        }

        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request) // tất cả các thuộc tính phải using form data
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ProductService.UpDateImage(imageId, request); // thằng này chả về id nhe
            if (result == 0)
                return BadRequest();// đây là nỗi 400

            return Ok(); // Ok trả ra 200 còn created là 201  khi học jquery đã được học cái này
        }

        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId) // tất cả các thuộc tính phải using form data
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ProductService.RemoveImage(imageId); // thằng này chả về id nhe
            if (result == 0)
                return BadRequest();// đây là nỗi 400

            return Ok(); // Ok trả ra 200 còn created là 201  khi học jquery đã được học cái này
        }

        [HttpGet("{productId}/images/{imageId}")] // nhớ phải đi qua images rồi mới đến imageId (có nghĩa là image của sản phẩm nay)
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _ProductService.GetImageById(imageId);
            if (image == null)
                return BadRequest("Cannot dìn Product");
            return Ok(image);
        }
    }
}