using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductUpdateRequest
    {
        public int Id { set; get; }

        [Display(Name = "Tên sản phẩm")]
        public string Name { set; get; }

        [Display(Name = "Miêu tả sản phẩm")]
        public string Description { set; get; }

        [Display(Name = "Chi tiết sản phẩm")]
        public string Details { set; get; }

        [Display(Name = "Mô tả seo")]
        public string SeoDescription { set; get; }

        [Display(Name = "Tiêu đề seo")]
        public string SeoTitle { set; get; }

        [Display(Name = "Tên seo")]
        public string SeoAlias { get; set; }

        [Display(Name = "Ngôn ngữ")]
        public string LanguageId { set; get; } // chỉ để query chứ ko update thằng này

        public bool IsFeatured { get; set; }

        [Display(Name = "Ảnh")]
        // phải install thằng IFormFile nó có chứ năng mã hóa nhị phân để ta sử dụng cho mã hóa nhị phân ảnh
        public IFormFile ThumbnailImage { get; set; }
    }
}