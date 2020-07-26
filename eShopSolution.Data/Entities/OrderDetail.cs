using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class OrderDetail
    {

        public int OrderId { set; get; }
        public int ProductId { set; get; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }

        public Order Order { get; set; } // để cấu hình 1 nhiều với Order

        public Product Product { get; set; } // để cấu hình 1 nhiều với Product

    }
}
