using DI.MVCTemplate.Data;
using DI.MVCTemplate.Models;
using DI.MVCTemplate.Rules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DI.MVCTemplate
{
    public class Startup
    {
        IWebHostEnvironment env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment _env)
        {
            Configuration = configuration;
            env = _env;
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //  .AddRoles<IdentityRole>()
            //  .AddEntityFrameworkStores<ApplicationDbContext>();

            //добавляются специфичные для Identity сервисы
            services.AddDefaultIdentity<AppUser>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false; //Необходимо указать число от 0-9 и пароль.
                options.Password.RequiredLength = 4; //Минимальная длина пароля.
                options.Password.RequiredUniqueChars = 2; //Требует количества уникальных символов в пароле.
                options.Password.RequireLowercase = false; //Требуется нижний регистр символов в пароле.
                options.Password.RequireNonAlphanumeric = false; //Требуется не буквенно-цифровых символов в пароле.
                options.Password.RequireUppercase = false; //Требуется символ верхнего регистра в пароле.

                // User settings
                options.User.RequireUniqueEmail = true;  //Требуется иметь уникальный адрес электронной почты пользователя.
                options.SignIn.RequireConfirmedAccount = false; //Истина, если у пользователя должна быть подтвержденная учетная запись, прежде чем он сможет войти, в противном случае - ложь.

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // добавляем сервис компрессии
            // Configure Compression level
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    // General
                    "text/plain",
                    "text/html",
                    "text/css",
                    "font/woff2",
                    "application/javascript",
                    "image/x-icon",
                    "image/png"
                };
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
                options.MimeTypes = MimeTypes;

            });

            //Нижний регистр для url-адресов
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllersWithViews();

            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var optionsRewrite = new RewriteOptions()
               .AddRedirect("(.*)/$", "$1"); //Удаление косой черты в конце
            optionsRewrite.Add(new RedirectLowerCaseRule()); //Перенаправление на нижний регистр
            app.UseRewriter(optionsRewrite);

            // обработка ошибок HTTP
            app.UseStatusCodePagesWithReExecute("/Home/ErrorStatus/{0}");

            app.UseHttpsRedirection();

            // подключаем компрессию
            app.UseResponseCompression();

            //Кэширование статичных файлов
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (ctx) =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=31536000");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
