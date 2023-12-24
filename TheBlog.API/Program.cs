using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TheBlog.API.Services;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.Services;
using TheBlog.MVC.Wrappers;

namespace TheBlog.API
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<TheBlogDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDbContext<TheBlogDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            builder.Services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();
            builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
            builder.Services.AddScoped<IRepository<ArticleUserRating>, Repository<ArticleUserRating>>();

            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IRoleSeederService, RoleSeederService>();
            builder.Services.AddScoped<IRoleManagerWrapper, RoleManagerWrapper>();
            builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            builder.Services.AddScoped<IUserManagerWrapper<User>, UserManagerWrapper<User>>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IArticleFilteringService, ArticleFilteringService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ISSUER_KEY", EnvironmentVariableTarget.User))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                 };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TheBlogDbContext>();
                context.Database.Migrate();

                var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeederService>();
                await roleSeeder.SeedRolesAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}