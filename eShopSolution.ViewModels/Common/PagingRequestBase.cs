using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PagingRequestBase   // dùng cho GetManaggeProductPagingRequest,GetPublicProductPagingRequest,GetUserPagingRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}