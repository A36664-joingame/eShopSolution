using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
    public interface IProductApiClient
    {
        Task<ApiResult<PagedResult<ProductVm>>> GetProductPagings(GetManageProductPagingRequest request);// để đọc cái Api láy paging của backEnd

        Task<bool> CreateProduct(ProductCreateRequest request);

        Task<int> UpdateProduct(ProductUpdateRequest request); // update dữ liệu

        Task<ProductVm> GetById(int productId, string languageId);

        Task<bool> Delete(int Id);

        Task<bool> UpdatePrice(int ProductId, decimal newPrice);

        Task<bool> UpdateStock(int ProductId, int newStock);

        Task<bool> CreateImage(ProductImageCreateRequest request);

        Task<int> UpDateImage(ProductImageUpdateRequest request);

        Task<ProductImageVm> GetImageById(int imageId);

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);

        Task<List<ProductVm>> GetFeaturedProducts(int take, string languageId);

        Task<List<ProductVm>> GetLatestProducts(int take, string languageId);

        Task<ProductDetails> GetProductDetails(int productId, string languageId);

        Task<List<ProductVm>> GetRelatedProduct(int productId, string languageId, int take);
    }
}