using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);// để đọc cái Api xác thực của backEnd

        Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request);// để đọc cái Api láy paging của backEnd

        Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request);// để đọc cái Api Update của backEnd

        Task<ApiResult<bool>> RegisterUser(RegisterRequest request);// để đọc cái Api đăng kí của backEnd

        Task<ApiResult<UserVm>> GetById(Guid id);// để đọc cái Api lấy Id của backEnd

        Task<ApiResult<bool>> Delete(Guid Id);  // để đọc cái Api Delete của backEnd

        Task<ApiResult<bool>> RoleAssign(Guid Id, RoleAssignRequest request);
    }
}