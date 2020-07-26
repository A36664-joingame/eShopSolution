using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    // bắt đầu cấu hình bảng nhiều nhiều
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            // chỉ ra 2 khóa chính đê liên kết đến hai bảng
            builder.HasKey(t => new { t.CategoryId, t.ProductId }); // tạo ra một hàm nặc danh

            builder.ToTable("ProductInCategories");

            // bắt đầu liên kết khóa phụ
            builder.HasOne(t => t.Product).WithMany(pc => pc.ProductInCategories)
                .HasForeignKey(pc=>pc.ProductId);

            builder.HasOne(t => t.Category).WithMany(pc => pc.ProductInCategories)
                 .HasForeignKey(pc => pc.CategoryId);


        }
    }
}
