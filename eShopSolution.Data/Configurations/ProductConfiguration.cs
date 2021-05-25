using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // tăng hạn chế cho mấy thuộc tính muốn

            builder.ToTable("Products");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn(); // cột id tự tăng

            builder.Property(x => x.Price).IsRequired(true);//  mặc định là true,ko ghi tru cũng được
            builder.Property(x => x.Stock).IsRequired(true).HasDefaultValue(0);// có thế chấm tiếp để tăng hạn chế nhe,như kiểu ngôn ngữ sql bình thường nhưng kiểu viết khác
            builder.Property(x => x.ViewCount).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.OriginalPrice).IsRequired();
        }
    }
}