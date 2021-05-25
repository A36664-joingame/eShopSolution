using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public interface IUserService
    {
        // thằng ApiResult ta tạo ra để check và thông báo một message hoặc trả về một object theo kiểu mà cnos chuyền vào

        Task<ApiResult<string>> Authencate(LoginRequest request);

        Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResult<bool>> Register(RegisterRequest request);

        // phương thức lấy tất cả User ra
        Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(GetUserPagingRequest request); // phườn thức này lấy ra một user và trả về một model phân trang

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<bool>> Delete(Guid Id);

        Task<ApiResult<bool>> RoleAssign(Guid Id, RoleAssignRequest request);
    }
}