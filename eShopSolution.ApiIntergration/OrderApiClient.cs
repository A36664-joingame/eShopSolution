using eShopeSolution.Utilities.Constants;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public class OrderApiClient :IOrderApiClient
    {

        private IHttpContextAccessor _httpContextAccessor;
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;

        public OrderApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
         
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<int> CreateOrder(CheckoutRequest request)
        {

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var sessions = _httpContextAccessor
              .HttpContext
              .Session // Session được cài đạt trong startup
              .GetString(SystemConstants.AppSettings.Token);
            // tạo ra một client
            var client = _httpClientFactory.CreateClient();
            // có địa chỉ giống địa chỉ cấu hình local host bên appsetting
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            //ủy quyền token giống đăng nhập Swagger để mở khóa
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            // phải chuyển thằng thumbImage sang binary
         

            // đẩy requestContent lên api  (backend)
            var response = await client.PostAsync($"/api/orders", httpContent); // nó gửi lên nhe

            if (response.IsSuccessStatusCode)
            {
                // lấy token ra để đăng nhập và sử dụng token như bên Swargger để đăng nhập
                return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
        }
    }
}
