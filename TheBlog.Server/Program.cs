using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlog.MVC.Services;
using System.Net.Mail;
using System.Net;
using TheBlog.MVC.Wrappers;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.Server
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<TheBlogDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDbContext<TheBlogDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IRepository<User>, Repository<User>>();

            builder.Services.AddTransient((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();

                var smtpClient = new SmtpClient
                {
                    Host = config.GetValue<string>("Email:Smtp:Host"),
                    Port = config.GetValue<int>("Email:Smtp:Port"),
                    EnableSsl = config.GetValue<bool>("Email:Smtp:EnableSsl"),
                    Credentials = new NetworkCredential(
                        userName: Environment.GetEnvironmentVariable("SMTP_USERNAME", EnvironmentVariableTarget.User),
                        password: Environment.GetEnvironmentVariable("SMTP_PASSWORD", EnvironmentVariableTarget.User)
                        )
                };

                return smtpClient;
            });

            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
            builder.Services.AddScoped<IRepository<ArticleUserRating>, Repository<ArticleUserRating>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<ReportedComment>, Repository<ReportedComment>>();

            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserAccessService, UserAccessService>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<ILinkGenerationService, LinkGenerationService>();
            builder.Services.AddScoped<IHttpRequestUrlWrapper, HttpRequestUrlWrapper>();
            builder.Services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
            builder.Services.AddScoped<ISignInManagerWrapper<User>, SignInManagerWrapper<User>>();
            builder.Services.AddScoped<IUserManagerWrapper<User>, UserManagerWrapper<User>>();
            builder.Services.AddScoped<IRoleSeederService, RoleSeederService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IRoleManagerWrapper, RoleManagerWrapper>();
            builder.Services.AddScoped<IArticleUserRatingService, ArticleUserRatingService>();
            builder.Services.AddScoped<IArticleCommentService, ArticleCommentService>();
            builder.Services.AddScoped<IArticleFilteringService, ArticleFilteringService>();
            builder.Services.AddScoped<IHomePageService, HomePageService>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddReact();

            builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
                .AddV8();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TheBlogDbContext>();
                context.Database.Migrate();

                var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeederService>();
                await roleSeeder.SeedRolesAsync();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseReact(config =>
            {
                config
                  .AddScript("~/js/bundle.js");
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapFallbackToController("Index", "Home");

            app.Run();
        }
    }
}