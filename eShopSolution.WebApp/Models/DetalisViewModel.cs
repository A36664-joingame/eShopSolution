using eShopSolution.ViewModels.Catalog.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.WebApp.Models
{
    public class DetalisViewModel
    {
        public ProductDetails productDetalis { get; set; }
        public List<ProductVm> productCategories { get; set; }
    }
}