using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductCreateRequest
    {
        // đây là phần của bảng Product cần tạo

        [Display(Name = "Giá Bán Sản Phẩm")]
        public decimal Price { set; get; }

        [Display(Name = "Giá Gốc Sản Phẩm")]
        public decimal OriginalPrice { set; get; }

        [Display(Name = "Số lượng Tồn Kho")]
        public int Stock { set; get; }

        // đây là phần cảu ProductTranslation cần tạo
        [Required(ErrorMessage = "phải nhập tên sản phẩm")]
        [Display(Name = "Tên Sản Phẩm")]
        public string Name { set; get; }

        [Display(Name = "Miêu Tả Sản Phẩm")]
        public string Description { set; get; }

        [Display(Name = "Chi Tiết Sản Phẩm")]
        public string Details { set; get; }

        [Display(Name = "Mô Tả Seo Sản Phẩm")]
        public string SeoDescription { set; get; }

        [Display(Name = "Tiêu Đề Seo")]
        public string SeoTitle { set; get; }

        [Display(Name = "Tên Seo")]
        public string SeoAlias { get; set; }

        public bool? IsFeatured { get; set; }
        public string LanguageId { set; get; }

        // phải install thằng IFormFile nó có chứ năng mã hóa nhị phân để ta sử dụng cho mã hóa nhị phân ảnh
        [Display(Name = "Ảnh Sản Phẩm")]
        public IFormFile ThumbnailImage { get; set; }
    }
}