using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopSolution.Data.Migrations
{
    public partial class SeedIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 26, 20, 20, 46, 207, DateTimeKind.Local).AddTicks(4833),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2020, 7, 26, 19, 58, 25, 344, DateTimeKind.Local).AddTicks(1392));

            migrationBuilder.InsertData(
                table: "AppRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("3792af46-9a8f-4ae6-a1c9-c9c910941e5b"), "ccf2ea50-7cc7-4f49-8cd8-08fba20911fa", "Administrator role", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { new Guid("be525247-1560-4657-8748-3563e08d7ed3"), new Guid("3792af46-9a8f-4ae6-a1c9-c9c910941e5b") });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DOB", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("be525247-1560-4657-8748-3563e08d7ed3"), 0, "bd0b6108-36fd-4ea3-a75b-b115370afbb4", new DateTime(2001, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "nguyenluc2001@gmail.com", true, "luc", "van", false, null, "nguyenluc2001@gmail.com", "admin", "AQAAAAEAACcQAAAAENkIF+sMAhCspadokINgv+km4xwOy/1Y2yTSR9pm7/s10Fwnt7gklCNbbA73hjL7PA==", null, false, "", false, "admin" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2020, 7, 26, 20, 20, 46, 223, DateTimeKind.Local).AddTicks(4136));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("3792af46-9a8f-4ae6-a1c9-c9c910941e5b"));

            migrationBuilder.DeleteData(
                table: "AppUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { new Guid("be525247-1560-4657-8748-3563e08d7ed3"), new Guid("3792af46-9a8f-4ae6-a1c9-c9c910941e5b") });

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("be525247-1560-4657-8748-3563e08d7ed3"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 26, 19, 58, 25, 344, DateTimeKind.Local).AddTicks(1392),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 26, 20, 20, 46, 207, DateTimeKind.Local).AddTicks(4833));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2020, 7, 26, 19, 58, 25, 360, DateTimeKind.Local).AddTicks(2981));
        }
    }
}
