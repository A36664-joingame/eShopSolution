using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.ProductImages
{
    public class ProductImageUpdateRequest
    {
        public int Id { get; set; }



        public string Caption { get; set; }

        public bool Isdefault { get; set; }  // ảnh có phải là ảnh mặc định hay không



        public int SortOrder { get; set; } // thứ tự của nó

        public IFormFile ImageFile { get; set; }
    }
}
