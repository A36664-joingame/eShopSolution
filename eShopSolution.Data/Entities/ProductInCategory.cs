using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
    // quan hệ nhiều nhiều Many-to-many
    public class ProductInCategory
    {
        // vì là quan hệ nhiêu nhiều lên ta tạo ra bảng mới là bảng này


        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
