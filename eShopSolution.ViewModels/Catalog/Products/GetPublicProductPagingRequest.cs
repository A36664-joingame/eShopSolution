using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class GetPublicProductPagingRequest: PagingRequestBase
    {
        
        public int? CategoryId { get; set; }  // chỉ cần lấy ra sản phẩm theo CtegoryId này thôi
    }
}
