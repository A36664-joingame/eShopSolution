using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PageResultBase    //dùng cho PagedResult
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int TotalRecords { set; get; }

        public int PageCount
        {
            get
            {
                var pageCount = (double)TotalRecords / PageSize;
                return (int)Math.Ceiling(pageCount); // lấy trần trong toán logic đó
            }
        }
    }
}