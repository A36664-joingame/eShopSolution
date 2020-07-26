using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            // khóa chính là hai thằng
            builder.HasKey(x => new { x.OrderId, x.ProductId });

            // lấy ra thăng order trong orderDetail ,trong order có list OrderDetails tham chiếu tới OrderId của orderDetail 
            builder.HasOne(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.Product).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductId);
        
        }
    }
}
