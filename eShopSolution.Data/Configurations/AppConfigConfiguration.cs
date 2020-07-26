using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class AppConfigConfiguration : IEntityTypeConfiguration<AppConfig>
    {
        public void Configure(EntityTypeBuilder<AppConfig> builder)
        {

            // thường là bên đối tượng class sẽ đặt là [Table("name thay thế")]
            builder.ToTable("AppConfigs");// tên bảng khi hiển thị ra


            // cấu hình cho các properties (thuộc tính)
            // mấy cái này là thuộc tính của AppConfig
            builder.HasKey(x => x.Key);
            builder.Property(x => x.Value).IsRequired(true);// IsRequired(true) là bắt phải nhập ,như kiểu thêm hạn chế
                                        // thường là trước viết thẳng chỗ thuộc tính như này [Required] ko được null


            // nếu liên kết với trang khác ta còn phải cấu hình khóa ngoại nữa

        }
    }
}
