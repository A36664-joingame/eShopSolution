using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    // cái interface này chỉ chứa các phương thức cho develop
    public interface IProductService
    {
        // dùng Task để sử dụng Async để dùng được nhiều Request cũng một luc và có thể dúng thread(đa luông)

        Task<int> Create(ProductCreateRequest request); // sử dụng task để trả về một Async

        Task<int> Update(ProductUpdateRequest request); // update dữ liệu

        Task<bool> UpdatePrice(int ProductId, decimal newPrice);

        Task<bool> UpdateStock(int ProductId, int addedQuantity);

        Task AddViewCount(int ProductId);//nếu ko viết kiểu mặc định là void

        Task<int> Delete(int ProductId);// chỉ cần chuyền vào ProductId

        Task<ProductVm> GetById(int productId, string languageId);

        Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(GetManageProductPagingRequest request);  // phân trang

        // phương thức thêm ảnh vào database
        Task<int> AddImage(ProductImageCreateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<int> UpDateImage(int imageId, ProductImageUpdateRequest request);

        Task<ProductImageVm> GetImageById(int imageId);

        Task<List<ProductImageVm>> GetListImages(int productId);

        Task<PagedResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request); // hiển thị danh sách sản phẩm theo categoryId (thể loại )

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);

        Task<List<ProductVm>> GetFeaturedProducts(string langugeId, int take);

        Task<List<ProductVm>> GetLatestProducts(string langugeId, int take);

        Task<List<ProductVm>> GetRelatedProducts(int productId, string langugeId, int take);

        Task<List<string>> GetImagePaths(int productId);
    }
}