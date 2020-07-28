﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopeSolution.Utilities.Constants;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Common;
using eShopSolution.Application.System.Users;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace eShopSolution.BackendApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // CẤU HÌNH ĐỂ KẾT NỖI VỚI DATABSE :https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
            // phải reference thằng Data và Application , utiliti và ViewModel vào nhe
            services.AddDbContext<EShopDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.MainConnectionString))); // ta lấy bên Utilities trong thư mục Constants để có được tên của ConectionString để kết nối với databse

            // NHỚ PHẢI ADD CÁI NÀY VÀO CHO PHÀN ĐĂNG NHẬP NHE
            services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<EShopDBContext>()
            .AddDefaultTokenProviders();

            //  KHAI BÁO DI
            services.AddTransient<IPublicProductService, PublicProductService>();
            services.AddTransient<IMangeProductService, MangeProductService>();
            services.AddTransient<IStorageService, FileStorageService>();
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.AddTransient<IUserService, UserService>();

            services.AddControllersWithViews();

            // tải nuget Swashbuckle.AspNetCore để cấu hình Swashbuckle nó là cái giao diện demo thực thi các phương thức người dùng với databaseS
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger eShop Solution", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // đặt UseSwagger bên dưới UseAuthorization     CÁI NÀY ĐỂ Ý HỘ NHE NHỚ CÓ ĐỂ CÒN CÓ CÁI GIAO DIỆN
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {                     //  đây là đường dẫn vào trong launchSetting thêm "launchUrl":"/swagger"  , khi chạy nó sẽ bật Browser nên nếu ko muốn nó lỗi có thể bỏ dấu /
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger eShop Solution V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}