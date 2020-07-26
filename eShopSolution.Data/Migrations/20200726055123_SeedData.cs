using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopSolution.Data.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeoAlias",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 26, 12, 51, 22, 707, DateTimeKind.Local).AddTicks(1605),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2020, 7, 26, 8, 42, 3, 177, DateTimeKind.Local).AddTicks(3079));

            migrationBuilder.InsertData(
                table: "AppConfigs",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "HomeTitle", "this is a home page eShopeSlution" },
                    { "HomeKeyWord", "this is KeyWord of eShopeSlution" },
                    { "HomeDescription", "this is Description of eShopeSlution" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsShowOnHome", "ParentId", "SortOrder", "Status" },
                values: new object[,]
                {
                    { 1, true, null, 1, 1 },
                    { 2, true, null, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "IsDefault", "Name" },
                values: new object[,]
                {
                    { "vi-VN", true, "Tiếng Việt" },
                    { "en-US", false, "Tiếng Anh" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "DateCreated", "OriginalPrice", "Price" },
                values: new object[] { 1, new DateTime(2020, 7, 26, 12, 51, 22, 724, DateTimeKind.Local).AddTicks(8094), 1000000m, 2000000m });

            migrationBuilder.InsertData(
                table: "CategoryTranslations",
                columns: new[] { "Id", "CategoryId", "LanguageId", "Name", "SeoAlias", "SeoDescription", "SeoTitle" },
                values: new object[,]
                {
                    { 1, 1, "vi-VN", "Áo Nam", "ao-nam", "Sản phảm áo thời trang nam", "Sản phẩm áo thời trang nam" },
                    { 3, 2, "vi-VN", "Áo Nữ", "ao-nu", "Sản phảm áo thời trang nữ", "Sản phẩm áo thời trang nữ" },
                    { 2, 1, "en-US", "Men Shirt", "men-shirt", "the shirt product for men", "the shirt product for men" },
                    { 4, 2, "en-US", "Women Shirt", "Women-shirt", "the shirt product for Women", "the shirt product for Women" }
                });

            migrationBuilder.InsertData(
                table: "ProductInCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "ProductTranslations",
                columns: new[] { "Id", "Description", "Details", "LanguageId", "Name", "ProductId", "SeoAlias", "SeoDescription", "SeoTitle" },
                values: new object[,]
                {
                    { 1, "Áo sơ mi nam trắng nguyễn lực", "Áo sơ mi nam trắng nguyễn lực", "vi-VN", "Áo sơ mi nam trắng nguyễn lực", 1, "ao-so-mi-nam-trang-nguyen-luc", "Áo sơ mi nam trắng nguyễn lực", "Áo sơ mi nam trắng nguyễn lực" },
                    { 2, "nguyen luc Men T-Shirt", "nguyen luc Men T-Shirt", "en-US", "nguyen luc Men T-Shirt", 1, "nguyen-luc-men-T-shirt", "nguyen luc Men T-Shirt", "nguyen luc Men T-Shirt" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeDescription");

            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeKeyWord");

            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeTitle");

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductInCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ProductTranslations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductTranslations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en-US");

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "vi-VN");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "SeoAlias",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 26, 8, 42, 3, 177, DateTimeKind.Local).AddTicks(3079),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 26, 12, 51, 22, 707, DateTimeKind.Local).AddTicks(1605));
        }
    }
}
