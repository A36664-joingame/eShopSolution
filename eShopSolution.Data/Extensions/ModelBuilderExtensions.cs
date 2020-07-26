using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Extensions
{
    // tạo extension này để data seeding dễ dàng hơn (nhớ nó là extension lên là static  nhe)
    // lấy trên https://www.learnentityframeworkcore.com/migrations/seeding
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // bắt đầu seeding từng thằng ở đây
            modelBuilder.Entity<AppConfig>().HasData(
               new AppConfig() { Key = "HomeTitle", Value = "this is a home page eShopeSlution" },
                new AppConfig() { Key = "HomeKeyWord", Value = "this is KeyWord of eShopeSlution" },
                 new AppConfig() { Key = "HomeDescription", Value = "this is Description of eShopeSlution" }
                  );

            modelBuilder.Entity<Language>().HasData(
                new Language() { Id = "vi-VN", Name = "Tiếng Việt", IsDefault = true },
                new Language() { Id = "en-US", Name = "Tiếng Anh", IsDefault = false }


                );

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    IsShowOnHome = true,
                    ParentId = null,
                    SortOrder = 1,
                    Status = Status.Active,

                },
                 new Category()
                 {
                     Id = 2,
                     IsShowOnHome = true,
                     ParentId = null,
                     SortOrder = 2,
                     Status = Status.Active,

                 });

            modelBuilder.Entity<CategoryTranslation>().HasData(

                        new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Áo Nam", LanguageId = "vi-VN", SeoDescription = "Sản phảm áo thời trang nam", SeoAlias = "ao-nam", SeoTitle = "Sản phẩm áo thời trang nam" }, // vì đây là con lên ko cần chỉ ra đủ hết các thuộc tính như thằng chính
                        new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Men Shirt", LanguageId = "en-US", SeoAlias = "men-shirt", SeoDescription = "the shirt product for men", SeoTitle = "the shirt product for men" },

                        new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Áo Nữ", LanguageId = "vi-VN", SeoDescription = "Sản phảm áo thời trang nữ", SeoAlias = "ao-nu", SeoTitle = "Sản phẩm áo thời trang nữ" },
                        new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Women Shirt", LanguageId = "en-US", SeoAlias = "Women-shirt", SeoDescription = "the shirt product for Women", SeoTitle = "the shirt product for Women" }


                    );





            modelBuilder.Entity<Product>().HasData(
              new Product()
              {
                  Id = 1,
                  DateCreated = DateTime.Now,
                  OriginalPrice = 1000000,
                  Price = 2000000,
                  Stock = 0,
                  ViewCount = 0,



              }

                );

            modelBuilder.Entity<ProductTranslation>().HasData(
                new ProductTranslation()
                {
                    Id = 1,
                    ProductId = 1,
                    Name = "Áo sơ mi nam trắng nguyễn lực",
                    LanguageId = "vi-VN",
                    SeoDescription = "Áo sơ mi nam trắng nguyễn lực",
                    SeoAlias = "ao-so-mi-nam-trang-nguyen-luc",
                    SeoTitle = "Áo sơ mi nam trắng nguyễn lực",
                    Details = "Áo sơ mi nam trắng nguyễn lực",
                    Description = "Áo sơ mi nam trắng nguyễn lực"
                },

                new ProductTranslation()
                {
                    Id = 2,
                    ProductId = 1,
                    Name = "nguyen luc Men T-Shirt",
                    LanguageId = "en-US",
                    SeoAlias = "nguyen-luc-men-T-shirt",
                    SeoDescription = "nguyen luc Men T-Shirt",
                    SeoTitle = "nguyen luc Men T-Shirt",
                    Details = "nguyen luc Men T-Shirt",
                    Description = "nguyen luc Men T-Shirt"
                }

                );


            modelBuilder.Entity<ProductInCategory>().HasData(
                 new ProductInCategory() { ProductId = 1, CategoryId = 1, }
                );



            // any guid
            var ROLE_ID = new Guid("3792AF46-9A8F-4AE6-A1C9-C9C910941E5B");
            var ADMIN_ID = new Guid("BE525247-1560-4657-8748-3563E08D7ED3");

            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = ROLE_ID,  // chữ tool trên kia có thể tạo cho ta guild mới nhe
                Name = "admin",
                NormalizedName = "admin",
                Description="Administrator role"
            }) ;

            var hasher = new PasswordHasher<AppUser>(); //
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = ADMIN_ID,
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "nguyenluc2001@gmail.com",
                NormalizedEmail = "nguyenluc2001@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "tl652200"),
                SecurityStamp = string.Empty,
                FirstName = "luc",
                LastName = "van",
                DOB = new DateTime(2001, 06, 16)
            }) ;

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });

            // song thì ta chạy lênh: add-migration SeedIdentityUser  và nhớ update-database

        }
    }
}
