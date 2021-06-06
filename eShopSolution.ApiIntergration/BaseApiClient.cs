using eShopeSolution.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)  // Tresponse là kiểu mà chuyền vào là gì thì trả ra là đấy cũng như Generic
        {
            // lấy ra chuỗi token
            var sessions = _httpContextAccessor
                .HttpContext
                .Session // Session được cài đạt trong startup
                .GetString(SystemConstants.AppSettings.Token);

            // tạo ra một client
            var client = _httpClientFactory.CreateClient();
            // có địa chỉ giống địa chỉ cấu hình local host bên appsetting
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            //ủy quyền token giống đăng nhập Swagger để mở khóa
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);  // điền token vào đây để xác thực (giông mật khâu)
            // với client lấy response trên url
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync(); // đọc nội dung chuỗi lấy từ response về
            if (response.IsSuccessStatusCode)
            {
                //response sẽ là một chuỗi json trả về chúng ta cần giải nó
                //  Và phương thức tôi đang sử dụng để giải tuần tự hóa một phản hồi JSON thành một TResponse đối tượng
                TResponse myDeserializedObjList = (TResponse)JsonConvert.DeserializeObject(body, typeof(TResponse));// muốn trả về kiểu TResponse nên ép sang  nó

                return myDeserializedObjList;
            }
            return JsonConvert.DeserializeObject<TResponse>(body); // cái này cũng giống trên chỉ là khồn chỉ ra typeof nhưng lại chuyền thẳng vòa
        }

        protected async Task<TResponse> PostAsync<TResponse>(string url, object request)
        {
            // chúng ta phải convert thằng request để chuyền lên nếu ko convert nó sẽ không post lên được
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            // phải AddHttpClient vào startup của AdminAPp nhe
            var client = _httpClientFactory.CreateClient();
            // nhớ phải set lại cổng cho Api là 5001 và UserApp là 5002 để nó không bị trùng cổng nhe     //CẨN THẬN NHẦM CỔNG ĐÉO LẤY ĐƯỢC TOKEN ĐÂU
            // lấy cấu hình  Appsetting devlopment trong AdminApp bằng _configuration
            client.BaseAddress = new Uri(_configuration["BaseAddress"]); // đó nó đã lấy được cấu hình của Appsetting devlopment trong AdminApp

            var response = await client.PostAsync(url, httpContent);// cái này dùng để đi đến tầng Backend theo đúng đường dẫn chuyền vào

            if (response.IsSuccessStatusCode)
            {
                // lấy token ra để đăng nhập và sử dụng token như bên Swargger để đăng nhập
                return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        protected async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);   // Phải add
            var response = await client.DeleteAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResponse>(body);

            return JsonConvert.DeserializeObject<TResponse>(body);
        }

        protected async Task<TResponse> PutAsync<TResponse>(string url, object request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // đây chính là đường dẫn kết nối với role của tầng Backend và lấy về response mang về đây sử lý tiếp
            var response = await client.PutAsync(url, httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResponse>(result);

            return JsonConvert.DeserializeObject<TResponse>(result);
        }
    }
}