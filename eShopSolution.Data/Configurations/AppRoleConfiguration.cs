using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            // thằng này đẵ có key là Guild rồi nhe không cần chỉ ra key nữa

            builder.ToTable("AppRoles");

            builder.Property(x => x.Description).HasMaxLength(200).IsRequired(true);
        }
    }
}
