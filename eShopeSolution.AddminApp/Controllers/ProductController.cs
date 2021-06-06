using eShopeSolution.Utilities.Constants;
using eShopSolution.ApiIntergration;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eShopeSolution.AddminApp.Controllers
{
    public class ProductController : Controller
    {
        // tiếm tục tiêm Di
        private readonly IProductApiClient _productApiClient;

        private readonly ICategoryApiClient _categoryApiClient;

        private readonly IConfiguration _configuration;

        public ProductController(IProductApiClient productApiClient,
            IConfiguration configuration, ICategoryApiClient categoryApiClient)
        {
            _configuration = configuration;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
        }

        // biding từ query nếu để class thì thêm [formQuery]
        public async Task<IActionResult> Index(int? categoryId, string keyword, int pageIndex = 1, int pageSize = 6)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var request = new GetManageProductPagingRequest()
            {
                KeyWord = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId,
                CategoryId = categoryId
            };
            var data = await _productApiClient.GetProductPagings(request);
            ViewBag.Keyword = keyword;

            var categories = await _categoryApiClient.GetAll(languageId); //chuyền lên cho ViewBag.Categories
            ViewBag.Categories = categories.ResultObj.Select(x => new SelectListItem() // phải SelctListItem thì thẻ select nó mới hiểu chứ chuyền thẳng nó ko hiểu đâu
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                // để nó dữ lại giá trị khi đã select
                Selected = categoryId.HasValue && categoryId.Value == x.Id // vì categoryId là object nên ta phải hasvalue vì nó có cả null
            });
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int Id, string LanguageId) // lấy ra Id nhớ Index cũng phải có Id nhe
        {
            // add View với View Name= Details , Template=Details , Model class=UserVm , và layout dùng chung

            var result = await _productApiClient.GetById(Id, LanguageId); // trả về một ProductVm
            return View(result); // nên ta sẽ trả luân về ProductVm
        }

        [HttpGet]  // lấy lên cho người dùng xem
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // người dùng đưa giữ liệu suống
        /*[Consumes("multipart/form-data")]*/  // phải có nó nó mới nhận được  cái data file từ FromFrom tương ứng với thằng backEnd
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            // dữ liệu đi từ view vào đây rooid vào CreateProduct
            if (!ModelState.IsValid)
                return View();
            var result = await _productApiClient.CreateProduct(request);  // cái này trả ra true false sang mà xem

            if (result)
            {
                TempData["result"] = "Thêm mới sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm sản phẩm thất bại");
            return View(request);
        }

        [HttpGet]  // lấy lên cho người dùng xem
        public IActionResult CreateImage(int productId)
        {
            var PrImageRequset = new ProductImageCreateRequest()
            {
                ProductId = productId
                
            };
            return View(PrImageRequset);
        }

        [HttpPost] // người dùng đưa giữ liệu suống
        [Consumes("multipart/form-data")]  // phải có nó nó mới nhận được  cái data file từ FromFrom tương ứng với thằng backEnd
        public async Task<IActionResult> CreateImage([FromForm] ProductImageCreateRequest request)
        {
            // dữ liệu đi từ view vào đây rooid vào CreateProduct
            if (!ModelState.IsValid)
                return View();
            var result = await _productApiClient.CreateImage(request);  // cái này trả ra true false sang mà xem
            if (result)
            {
                TempData["result"] = "Thêm mới ảnh thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm ảnh thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string LanguageId)
        {
            //lấy sản phẩm ra để cho người dùng xem

            var result = await _productApiClient.GetById(id, LanguageId);

            if (result != null)
            {
                var ProductRequest = new ProductUpdateRequest()
                {
                    Name = result.Name,
                    Description = result.Description,
                    Details = result.Details,
                    SeoAlias = result.SeoAlias,
                    SeoDescription = result.SeoDescription,
                    SeoTitle = result.SeoTitle,
                    Id = id,
                    LanguageId = LanguageId,
                    IsFeatured=result.IsFeatured==null?false:result.IsFeatured.Value
                };

                return View(ProductRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.UpdateProduct(request);
            if (result > 0)
            {
                // khi thành công ta có thể tạo một TempData  đây là đầu đi và sẽ có đầu nhận dữ liệu này nhe bên View Của nó
                TempData["result"] = "Sửa Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> EditPrice(int id, string LanguageId)
        {
            //lấy price của sản phẩm tương ứng lên cho người dùng xem

            var result = await _productApiClient.GetById(id, LanguageId);

            if (result != null)
            {
                var prVm = new ProductVm()
                {
                    Price = result.Price,
                    Id = result.Id,
                };

                return View(prVm);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditPrice(int id, decimal Price)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.UpdatePrice(id, Price);
            if (result)
            {
                TempData["result"] = "Sửa Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditImage(int imageId)
        {
            //lấy price của sản phẩm tương ứng lên cho người dùng xem

            var result = await _productApiClient.GetImageById(imageId);

            if (result != null)
            {
                var prImageVm = new ProductImageUpdateRequest()
                {
                    Caption = result.Caption,
                    Id = result.Id,
                    Isdefault = result.Isdefault,
                    SortOrder = result.SortOrder
                };

                return View(prImageVm);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.UpDateImage(request);
            if (result > 0)
            {
                TempData["result"] = "Sửa Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditStock(int id, string LanguageId)
        {
            //lấy price của sản phẩm tương ứng lên cho người dùng xem

            var result = await _productApiClient.GetById(id, LanguageId);

            if (result != null)
            {
                var prVm = new ProductVm()
                {
                    Stock = result.Stock,
                    Id = result.Id,
                };

                return View(prVm);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditStock(int id, int Stock)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.UpdateStock(id, Stock);
            if (result)
            {
                TempData["result"] = "Sửa Thành Công"; //có key là result
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Delete(int id)  // lấy id của đối tựng cần xóa về
        {
            return View(new ProductDeleteRequest()
            {
                Id = id,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteRequest request)  // lấy request từ thằng backeEnd thông qua thằng UserApiClient
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _productApiClient.Delete(request.Id);// với id lấy về trên kia trong request và thực hiện xóa
            if (result)
            {
                // khi thành công ta có thể tạo một TempData  đây là đầu đi và sẽ có đầu nhận dữ liệu này nhe bên View Của nó
                TempData["result"] = "Xóa Thành Công"; //có key là result
                return RedirectToAction("Index");  // nếu thành công thì chuyển tới phân trang là Index
            }

            return View(request);//nếu ko thành công ta chả về request để xem request
        }

        [HttpGet]
        public async Task<IActionResult> CategoryAssign(int productId) // chuyền id từ index về đây nhờ phương thức HttpGet
        {
            // add view có VIew name là RoleAssign Template là Edit và Model clas RoleAssignRequest à layout dùng chung

            var roleAssignRequest = await GetCategoryAssignRequest(productId);

            return View(roleAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryAssign(CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.CategoryAssign(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhập danh mục thành công"; //có key là result
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);  // đây là lỗi của Model này

            var roleAssignRequest = await GetCategoryAssignRequest(request.Id); // nếu trong trường hợp bị false nó vẫn lấy được về
            return View(roleAssignRequest);
        }

        private async Task<CategoryAssignRequest> GetCategoryAssignRequest(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var productObj = await _productApiClient.GetById(id, languageId);

            // chúng ta phải lấy ra được danh sách role
            var categories = await _categoryApiClient.GetAll(languageId);  // trả về một danh sách ApiResult<List<RoleVm>> ,ta đưa vào list<RoleVm> nên thằng ResultObj sẽ là list<RoleVm>

            var categoryAssignRequest = new CategoryAssignRequest();
            foreach (var category in categories.ResultObj)  // ResultObj nằm trong ApiResult
            {
                categoryAssignRequest.Categories.Add(new SelectItem()
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Selected = productObj.Categories.Contains(category.Name) // Contains xem nó có chứa Name của role không,nếu có nó sẽ là true không là false
                });
            }

            categoryAssignRequest.Id = id;

            return categoryAssignRequest;
        }
    }
}