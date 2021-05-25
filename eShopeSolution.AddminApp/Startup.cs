using eShopSolution.ApiIntergration;
using eShopSolution.ViewModels.System.Users;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace eShopeSolution.AddminApp
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
            // add thêm AddHttpClient vào đây yêu cầu bên UserApiClient cho mấy thằng translation
            services.AddHttpClient();

            // add thêm nó cho phần cookies cho bên UseController sử dụng vào :https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Index";// đường dẫn đến trang login khi chưa đăng nhập
                    options.AccessDeniedPath = "/Account/Forbidden";  // AccessDeniedPath(từ chối chuy cập) thì về trang theo đường dẫn này /Account/Forbidden
                });

            // add valiator cho tất cả poject
            services.AddControllersWithViews() // add them nuget FluentValidation
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>()); // chúng ta cũng muốn dùng chung các valigator các hạn chế cho request bên này nên cũng cho thằng này vào nó sẽ valigator tất cả project

            // add thêm Token để sử dụng cho UsersController bên Api phải add nó sau AddControllersWithViews và ở dưới phải thêm app.UseSession() sau thằng Authorization()
            // ta phải add nuget Microsoft.AspNetCore.Session vào AdminAp
            services.AddSession(options =>
            {
                // set thời gian thoát của Session //quy định nhỏ nhất là 20'
                options.IdleTimeout = TimeSpan.FromMinutes(30);  // lưu Session trong 30 phút sau đó tự động remove
            });

            // add thằng này cho UserApiClient ở phương thức GetById   nó cũng như Di
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  // là để lấy ra sessions bên BaseApiClient mà xem lấy

            // tiêm Di tro user  sử dụng trong UserController
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IRoleApiClient, RoleApiClient>();
            services.AddTransient<ILanguageApiClient, LanguageApiClient>();
            services.AddTransient<ICategoryApiClient, CategoryApiClient>();

            //tiêm Di cho product
            services.AddTransient<IProductApiClient, ProductApiClient>();

            // < !--add thêm nuget Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation vào để chạy cùng chương trình-- >
            IMvcBuilder builder = services.AddRazorPages();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
#if DEBUG
            if (environment == Environments.Development)
            {
                builder.AddRazorRuntimeCompilation();
            }

#endif
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
                app.UseExceptionHandler("/Home/Error");// sử dụng sử lý ngoại lệ
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // thêm thằng này để sử dụng cấu hình cookies ở trên kia nó phải nằm đúng chỗ nhe
            app.UseAuthentication(); // sử dụng xác thực  ở đây ta cấu hình cookies nên nó sẽ sử dụng cookies để xác thực

            app.UseRouting();

            app.UseAuthorization();// sử dụng ủy quyền

            // add thằng này phải để ở dưới UseAuthorization
            app.UseSession();  // phải seur dụng nó mới có Session sử dụng được

            app.UseEndpoints(endpoints =>  // sử dụng điểm cuối để trả lên
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}