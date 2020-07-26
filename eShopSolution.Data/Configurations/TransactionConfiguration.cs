using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace eShopSolution.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<TranSaction>
    {
        public void Configure(EntityTypeBuilder<TranSaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();

            builder.HasOne(x => x.AppUser).WithMany(x => x.TranSactions).HasForeignKey(x => x.UserId);
        }
    }
}
