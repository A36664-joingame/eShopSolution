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
                new Language() { Id = "vi", Name = "Tiếng Việt", IsDefault = true },
                new Language() { Id = "en", Name = "Tiếng Anh", IsDefault = false }

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

                        new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Áo Nam", LanguageId = "vi", SeoDescription = "Sản phảm áo thời trang nam", SeoAlias = "ao-nam", SeoTitle = "Sản phẩm áo thời trang nam" }, // vì đây là con lên ko cần chỉ ra đủ hết các thuộc tính như thằng chính
                        new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Men Shirt", LanguageId = "en", SeoAlias = "men-shirt", SeoDescription = "the shirt product for men", SeoTitle = "the shirt product for men" },

                        new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Áo Nữ", LanguageId = "vi", SeoDescription = "Sản phảm áo thời trang nữ", SeoAlias = "ao-nu", SeoTitle = "Sản phẩm áo thời trang nữ" },
                        new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Women Shirt", LanguageId = "en", SeoAlias = "Women-shirt", SeoDescription = "the shirt product for Women", SeoTitle = "the shirt product for Women" }

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
                  IsFeatured = true
              },
               new Product()
               {
                   Id = 2,
                   DateCreated = DateTime.Now,
                   OriginalPrice = 1000000,
                   Price = 2000000,
                   Stock = 0,
                   ViewCount = 0,
                   IsFeatured = true
               },
                new Product()
                {
                    Id = 3,
                    DateCreated = DateTime.Now,
                    OriginalPrice = 1000000,
                    Price = 2500000,
                    Stock = 0,
                    ViewCount = 0,
                    IsFeatured = true
                },
                 new Product()
                 {
                     Id = 4,
                     DateCreated = DateTime.Now,
                     OriginalPrice = 1000000,
                     Price = 3000000,
                     Stock = 0,
                     ViewCount = 0,
                     IsFeatured = true
                 },
                  new Product()
                  {
                      Id = 5,
                      DateCreated = DateTime.Now,
                      OriginalPrice = 1000000,
                      Price = 1200000,
                      Stock = 0,
                      ViewCount = 0,
                      IsFeatured = true
                  },
                   new Product()
                   {
                       Id = 6,
                       DateCreated = DateTime.Now,
                       OriginalPrice = 1000000,
                       Price = 45000000,
                       Stock = 0,
                       ViewCount = 0,
                       IsFeatured = true
                   }

                ) ;

            modelBuilder.Entity<ProductTranslation>().HasData(
                
                new ProductTranslation()
                {
                    Id = 1,
                    ProductId = 1,
                    Name = "Cơm gà",
                    LanguageId = "vi",
                    SeoDescription = "Cơm gà ngon bổ rẻ",
                    SeoAlias = "com-ga",
                    SeoTitle = "Cơm gà",
                    Details = "Cơm gà ngon bổ rẻ",
                    Description = "Cơm gà ngon bổ rẻ"
                },

                new ProductTranslation()
                {
                    Id = 2,
                    ProductId = 2,
                    Name = "Cơm gà",
                    LanguageId = "vi",
                    SeoDescription = "Cơm gà ngon bổ rẻ",
                    SeoAlias = "com-ga",
                    SeoTitle = "Cơm gà",
                    Details = "Cơm gà ngon bổ rẻ",
                    Description = "Cơm gà ngon bổ rẻ"
                },
                  new ProductTranslation()
                  {
                      Id = 3,
                      ProductId = 3,
                      Name = "Áo sơ mi nam trắng nguyễn lực",
                      LanguageId = "vi",
                      SeoDescription = "Áo sơ mi nam trắng nguyễn lực",
                      SeoAlias = "ao-so-mi-nam-trang-nguyen-luc",
                      SeoTitle = "Áo sơ mi nam trắng nguyễn lực",
                      Details = "Áo sơ mi nam trắng nguyễn lực",
                      Description = "Áo sơ mi nam trắng nguyễn lực"
                  },
                    new ProductTranslation()
                    {
                        Id = 4,
                        ProductId = 4,
                        Name = "Áo sơ mi nam trắng nguyễn lực",
                        LanguageId = "vi",
                        SeoDescription = "Áo sơ mi nam trắng nguyễn lực",
                        SeoAlias = "ao-so-mi-nam-trang-nguyen-luc",
                        SeoTitle = "Áo sơ mi nam trắng nguyễn lực",
                        Details = "Áo sơ mi nam trắng nguyễn lực",
                        Description = "Áo sơ mi nam trắng nguyễn lực"
                    },
                      new ProductTranslation()
                      {
                          Id = 5,
                          ProductId = 5,
                          Name = "Áo sơ mi nam trắng nguyễn lực",
                          LanguageId = "vi",
                          SeoDescription = "Áo sơ mi nam trắng nguyễn lực",
                          SeoAlias = "ao-so-mi-nam-trang-nguyen-luc",
                          SeoTitle = "Áo sơ mi nam trắng nguyễn lực",
                          Details = "Áo sơ mi nam trắng nguyễn lực",
                          Description = "Áo sơ mi nam trắng nguyễn lực"
                      },
                        new ProductTranslation()
                        {
                            Id = 6,
                            ProductId = 6,
                            Name = "Áo sơ mi nam trắng nguyễn lực",
                            LanguageId = "vi",
                            SeoDescription = "Áo sơ mi nam trắng nguyễn lực",
                            SeoAlias = "ao-so-mi-nam-trang-nguyen-luc",
                            SeoTitle = "Áo sơ mi nam trắng nguyễn lực",
                            Details = "Áo sơ mi nam trắng nguyễn lực",
                            Description = "Áo sơ mi nam trắng nguyễn lực"
                        }

                );

            modelBuilder.Entity<ProductImage>().HasData(

                new ProductImage()
                {

                    Id=1,
                    Caption="",
                    DateCreated=DateTime.Now,
                    FileSize=100,
                    ImagePath= "0f75e9c5-b794-4f2d-a328-7de10107b36c.jpg",
                    Isdefault=true,
                    ProductId=1,
                    SortOrder=1
                    
                },
                 new ProductImage()
                 {

                     Id = 2,
                     Caption = "",
                     DateCreated = DateTime.Now,
                     FileSize = 100,
                     ImagePath = "4b3f6988-c6ab-4f18-bf60-e5adf20c6ad3.jpeg",
                     Isdefault = true,
                     ProductId = 2,
                     SortOrder = 1

                 },
                  new ProductImage()
                  {

                      Id = 3,
                      Caption = "",
                      DateCreated = DateTime.Now,
                      FileSize = 100,
                      ImagePath = "5b6cd57e-0dab-4a51-9150-36db8773aac8.jpeg",
                      Isdefault = true,
                      ProductId = 3,
                      SortOrder = 1

                  },
                  
                    new ProductImage()
                    {

                        Id = 4,
                        Caption = "",
                        DateCreated = DateTime.Now,
                        FileSize = 100,
                        ImagePath = "804e1e0c-ec76-4fd9-a91f-d400e99ff414.jpeg",
                        Isdefault = true,
                        ProductId = 4,
                        SortOrder = 1

                    },
                     new ProductImage()
                     {

                         Id = 5,
                         Caption = "",
                         DateCreated = DateTime.Now,
                         FileSize = 100,
                         ImagePath = "bd360e3b-e1f1-4ec6-b33b-a6c7a69e665c.jpg",
                         Isdefault = true,
                         ProductId = 5,
                         SortOrder = 1

                     },
                      new ProductImage()
                      {

                          Id = 6,
                          Caption = "",
                          DateCreated = DateTime.Now,
                          FileSize = 100,
                          ImagePath = "e0254137-2422-40d4-ba03-a3cf6559bf17.jpg",
                          Isdefault = true,
                          ProductId = 6,
                          SortOrder = 1

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
                Description = "Administrator role"
            });

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
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });

            modelBuilder.Entity<Slide>().HasData(
              new Slide()
              {
                  Id = 1,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 1,
                  Url = "#",
                  Image = "/themes/images/carousel/1.png",
                  Status = Status.Active
              },
              new Slide()
              {
                  Id = 2,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 2,
                  Url = "#",
                  Image = "/themes/images/carousel/2.png",
                  Status = Status.Active
              },
              new Slide()
              {
                  Id = 3,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 3,
                  Url = "#",
                  Image = "/themes/images/carousel/3.png",
                  Status = Status.Active
              }
              ,
              new Slide()
              {
                  Id = 4,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 4,
                  Url = "#",
                  Image = "/themes/images/carousel/4.png",
                  Status = Status.Active
              }
              ,
              new Slide()
              {
                  Id = 5,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 5,
                  Url = "#",
                  Image = "/themes/images/carousel/5.png",
                  Status = Status.Active
              }
              ,
              new Slide()
              {
                  Id = 6,
                  Name = "Second Thumbnail label",
                  Description = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.",
                  SortOrder = 6,
                  Url = "#",
                  Image = "/themes/images/carousel/6.png",
                  Status = Status.Active
              });

            // song thì ta chạy lênh: add-migration SeedIdentityUser  và nhớ update-database
        }
    }
}