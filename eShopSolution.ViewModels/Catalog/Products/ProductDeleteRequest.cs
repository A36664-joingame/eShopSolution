using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductDeleteRequest
    {
        [Display(Name = "Mã Sản Phẩm")]
        public int Id { get; set; }
    }
}