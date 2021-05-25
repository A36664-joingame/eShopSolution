using eShopeSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;

        public ProductApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ApiResult<PagedResult<ProductVm>>> GetProductPagings(GetManageProductPagingRequest request)
        {
            // chuyền vào đường dẫn url để lấy kết quả trên request và vào trong BaseApiClient thực hiện phương thức GetAsync để giải json và lấy ra PagedResult<ProductVm>
            var data = await GetAsync<ApiResult<PagedResult<ProductVm>>>( // nó sẽ đọc theo kiểu ApiResult nhe chứ không có nó thì đéo đọc được đâu
                $"/api/products/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&keyword={request.KeyWord}&languageId={request.LanguageId}&CategoryId={request.CategoryId}");

            return data;
        }

        public async Task<bool> CreateProduct(ProductCreateRequest request)
        {
            // dữ liệu từ Controller vào đây
            // lấy ra chuỗi token
            var sessions = _httpContextAccessor
                .HttpContext
                .Session // Session được cài đạt trong startup
                .GetString(SystemConstants.Appsettings.Token);
            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.Appsettings.DefaultLanguageId);

            // tạo ra một client
            var client = _httpClientFactory.CreateClient();
            // có địa chỉ giống địa chỉ cấu hình local host bên appsetting
            client.BaseAddress = new Uri(_configuration[SystemConstants.Appsettings.BaseAddress]);
            //ủy quyền token giống đăng nhập Swagger để mở khóa
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            // phải chuyển thằng thumbImage sang binary
            if (request.ThumbnailImage != null)    // chuyền Ảnh lấy từ Controoller sang byte
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "ThumbnailImage", request.ThumbnailImage.FileName); // phải add đúng tên bên ProductService
            }
            // song add nó vào requestContent
            requestContent.Add(new StringContent(request.Price.ToString()), "price");
            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "OriginalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(request.Name.ToString()), "Name");
            requestContent.Add(new StringContent(request.Description.ToString()), "Description");
            requestContent.Add(new StringContent(request.Details.ToString()), "Details");
            requestContent.Add(new StringContent(request.SeoDescription.ToString()), "SeoDescription");
            requestContent.Add(new StringContent(request.SeoTitle.ToString()), "SeoTitle");
            requestContent.Add(new StringContent(request.SeoAlias.ToString()), "SeoAlias");
            requestContent.Add(new StringContent(languageId.ToString()), "languageId");

            // đẩy requestContent lên api  (backend)
            var response = await client.PostAsync($"/api/products", requestContent); // nó gửi lên nhe
            return response.IsSuccessStatusCode;
        }

        public async Task<int> UpdateProduct(ProductUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.Appsettings.BaseAddress]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            //  var json = JsonConvert.SerializeObject(request);

            var json = JsonConvert.SerializeObject(request);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // đây chính là đường dẫn kết nối với role của tầng Backend và lấy về response mang về đây sử lý tiếp
            var response = await client.PutAsync($"/api/products", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<int>(result);

            return JsonConvert.DeserializeObject<int>(result);
        }

        public async Task<ProductVm> GetById(int id, string LanguageId)
        {
            var sessions = _httpContextAccessor
                  .HttpContext
                  .Session
                  .GetString(SystemConstants.Appsettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.Appsettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/products/{id}/{LanguageId}"); // đúng đương dẫn đấy
            var body = await response.Content.ReadAsStringAsync(); // đọc nội dung chuỗi lấy từ response về
            if (response.IsSuccessStatusCode)
            {
                //response sẽ là một chuỗi json trả về chúng ta cần giải nó
                //  Và phương thức tôi đang sử dụng để giải tuần tự hóa một phản hồi JSON thành một TResponse đối tượng
                ProductVm myDeserializedObjList = (ProductVm)JsonConvert.DeserializeObject(body, typeof(ProductVm));// muốn trả về kiểu TResponse nên ép sang  nó

                return myDeserializedObjList;
            }
            return JsonConvert.DeserializeObject<ProductVm>(body); // trả về một sản phẩm
        }

        public async Task<bool> Delete(int Id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);   // Phải add
            var response = await client.DeleteAsync($"/api/products/{Id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<bool>(body);

            return JsonConvert.DeserializeObject<bool>(body);
        }

        public async Task<bool> UpdatePrice(int ProductId, decimal newPrice)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.PatchAsync($"/api/products/{ProductId}/{newPrice}", null);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<bool>(result);
            return JsonConvert.DeserializeObject<bool>(result);
        }

        public async Task<bool> UpdateStock(int ProductId, int newStock)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.PatchAsync($"/api/products/{ProductId}/Stock/{newStock}", null);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<bool>(result);
            return JsonConvert.DeserializeObject<bool>(result);
        }

        public async Task<bool> CreateImage(ProductImageCreateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var requestContent = new MultipartFormDataContent();

            // phải chuyển thằng ImageFile sang binary
            if (request.ImageFile != null)    // chuyền Ảnh lấy từ Controoller sang byte
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "ImageFile", request.ImageFile.FileName); // phải add đúng tên bên ProductService
            }

            if(request.Caption!=null)
            requestContent.Add(new StringContent(request.Caption.ToString()), "Caption");
            requestContent.Add(new StringContent(request.SortOrder.ToString()), "SortOrder");
            requestContent.Add(new StringContent(request.Isdefault.ToString()), "Isdefault");
            requestContent.Add(new StringContent(request.ProductId.ToString()), "ProductId");
           
            var response = await client.PostAsync($"/api/products/images", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> UpDateImage(ProductImageUpdateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.PutAsync($"/api/products/images/{request.Id}", null);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<int>(result);
            return JsonConvert.DeserializeObject<int>(result);
        }

        public async Task<ProductImageVm> GetImageById(int imageId)
        {
            var sessions = _httpContextAccessor
                  .HttpContext
                  .Session
                  .GetString(SystemConstants.Appsettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.Appsettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/products/images/{imageId}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                ProductImageVm myDeserializedObjList = (ProductImageVm)JsonConvert.DeserializeObject(body, typeof(ProductImageVm));

                return myDeserializedObjList;
            }
            return JsonConvert.DeserializeObject<ProductImageVm>(body);
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // đây chính là đường dẫn kết nối với role của tầng Backend và lấy về response mang về đây sử lý tiếp
            var response = await client.PutAsync($"/api/products/{id}/categories", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<List<ProductVm>> GetFeaturedProducts(int take, string languageId)
        {
            var data = await GetAsync<List<ProductVm>>($"/api/products/featured/{languageId}/{take}");

            return data;
        }

        public async Task<List<ProductVm>> GetLatestProducts(int take, string languageId)
        {
            var data = await GetAsync<List<ProductVm>>($"/api/products/latest/{languageId}/{take}");

            return data;
        }

        public async Task<ProductDetails> GetProductDetails(int productId, string languageId)
        {
            var data = await GetAsync<ProductDetails>($"/api/products/detalis/{productId}/{languageId}");
            return data;
        }

        public async Task<List<ProductVm>> GetRelatedProduct(int productId, string languageId, int take)
        {
            var data = await GetAsync<List<ProductVm>>($"/api/products/related/{productId}/{take}/{languageId}");
            return data;
        }
    }
}