using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.ProductImages
{
    public class ProductImageCreateRequest
    {
        [Display(Name = "Chú thích ảnh")]
        public string Caption { get; set; }

        [Display(Name = "Chọn ảnh mặc định")]
        public bool Isdefault { get; set; }  // ảnh có phải là ảnh mặc định hay không

        [Display(Name = "Thứ tự ảnh")]
        public int SortOrder { get; set; } // thứ tự của nó

        public int ProductId { get; set; }

        [Display(Name = "Chọn ảnh từ máy")]
        public IFormFile ImageFile { get; set; }

        
       
    }
}