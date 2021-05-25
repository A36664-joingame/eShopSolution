using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.ProductImages
{
    public class ProductImageVm
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImagePath { get; set; }

        public string Caption { get; set; }

        public bool Isdefault { get; set; }  // ảnh có phải là ảnh mặc định hay không

        public DateTime DateCreated { get; set; } // ngày tạo

        public int SortOrder { get; set; } // thứ tự của nó

        public long FileSize { get; set; }
    }
}