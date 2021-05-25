using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopeSolution.Utilities.Constants;
using eShopSolution.Application.Catalog.Categories;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Common;
using eShopSolution.Application.System.Languages;
using eShopSolution.Application.System.Roles;
using eShopSolution.Application.System.Users;
using eShopSolution.Application.Utilities.Slides;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.System.Users;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
            services.AddDbContext<EShopDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.MainConnectionString))); // ta lấy bên Utilities trong thư mục Constants để có được tên của ConectionString để kết nối với databse

            // NHỚ PHẢI ADD CÁI NÀY VÀO CHO PHÀN ĐĂNG NHẬP NHE
            services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<EShopDbContext>()
            .AddDefaultTokenProviders();

            //  KHAI BÁO DI
            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IStorageService, FileStorageService>();
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<ICategoryService, CategoryService>();

            services.AddTransient<ILanguageService, LanguageService>();

            services.AddTransient<ISlideService, SlideService>();

            // đây là Registor theo Di lẻ từng thằng 1
            //   services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();// Khai báo cho Validator theo DI
            //   services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

            // đây là registor theo Di cả đám cảu project luân
            // đăng kí tất cả thằng Validator nào có trong cùng prioject  với LoginRequestValidator ,,   NHƯ KIỂU đăng kí thằng LoginRequest với LoginRequestValidator
            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>()); // thêm AddFluentValidation để sử dụng Vlidation tải từ nuget FluentValidation.AspNetCore

            // tải nuget Swashbuckle.AspNetCore để cấu hình Swashbuckle nó là cái giao diện demo thực thi các phương thức người dùng với databaseS
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger eShop Solution", Version = "v1" }); // chỉ ra tiêu đề và version cho thằng Swagger

                // thêm 2 thằng này cho Swagger để tạo khóa   chức năng là tạo khóa khi đăng nhập và đúng token lấy từ đăng nhập thành công chuyền vào để khóa lại map với phương thức bên dưới để chỉ đăng nhập mới có quyền thực hiện các phương thức đó
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    //header cho đăng nhập bên swagger
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
            });

            // SAU KHI Swagger ta lấy ra key và Issuer trong Appseting cảu Api
            // thằng này tạo ra để nếu không đúng cái token mà khi đăng nhập có thì nó sẽ chả ngay ra 401        ( nói chung là phải đăng nhập mới được xem sản phẩm hay bất cứ quền nào của sản phẩm ) cái này rất quan tọng nhe

            string issuer = Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

            // phần validation token từ client chả về nếu không đúng trả nhay về 401 ,TỰ GIẢI MÃ VÀ VALIDATION
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;             // phải cài đặ nuget cho JwtBearerDefaults là Microsoft.AspNetCore.Authentication.JwtBearer
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = System.TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
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

            app.UseAuthentication(); // phải thêm thằng này thứ tự cá middelware phải chuẩn nhe

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