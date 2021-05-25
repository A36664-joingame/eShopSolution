using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.ApiIntergration;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public ProductController(IProductApiClient productApiClient, ICategoryApiClient categoryApiClient)
        {
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Details(int id, string culture)
        {
            var product = await _productApiClient.GetProductDetails(id, culture);
            var productCategories = await _productApiClient.GetRelatedProduct(id, culture, 6);
            var detailsViewModel = new DetalisViewModel()
            {
                productDetalis = product,
                productCategories = productCategories
            };
            return View(detailsViewModel);
        }

        public async Task<IActionResult> Category(int id, string keyword, int pageIndex = 1, int pageSize = 6)
        {
            var languageId = CultureInfo.CurrentCulture.Name;

            var request = new GetManageProductPagingRequest()
            {
                KeyWord = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId,
                CategoryId = id
            };
            var data = await _productApiClient.GetProductPagings(request);

            return View(data.ResultObj);
        }
    }
}