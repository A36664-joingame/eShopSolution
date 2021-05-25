using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public class UserApiClient : BaseApiClient, IUserApiClient
    {
        public UserApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            // khi thằng UserController của Api nó post token lên ở đây ta sẽ lấy nó về theo đường dẫn đấy
            var data = await PostAsync<ApiResult<string>>("/api/users/authenticate", request); // đương dẫn này bên userController Của Api

            return data;
        }

        // Delete
        public async Task<ApiResult<bool>> Delete(Guid Id)
        {
            var data = await DeleteAsync<ApiResult<bool>>($"/api/users/{Id}");

            return data;
        }

        // lấy Id
        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var data = await GetAsync<ApiResult<UserVm>>($"/api/users/{id}");
            return data;
        }

        // lấy ra UserPaging và appsettings.Development của AdminApp sửa
        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request)   // dùng thằng này cho UserController của   GetAllPaging
        {
            var data = await GetAsync<ApiResult<PagedResult<UserVm>>>($"/api/users/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.KeyWord}"); // đường dẫ lấy bên UserController của Api nó sẽ theo request trả về
            return data;
        }

        // đăng kí người dùng
        public async Task<ApiResult<bool>> RegisterUser(RegisterRequest request)
        {
            var data = await PostAsync<ApiResult<bool>>("/api/users", request);

            return data;
        }

        // phương thức role cho frontend
        public async Task<ApiResult<bool>> RoleAssign(Guid Id, RoleAssignRequest request)
        {
            var data = await PutAsync<ApiResult<bool>>($"/api/users/{Id}/roles", request);
            return data;
        }

        // UpdateUser
        public async Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            var data = await PutAsync<ApiResult<bool>>($"/api/users/{id}", request);
            return data;
        }
    }
}