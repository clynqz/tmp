using Anomaly.Controllers;
using Anomaly.Data;
using Anomaly.Data.Repositories;
using Anomaly.Middlewares.Extensions;
using Anomaly.Services.FileHashers;
using Anomaly.Services.PasswordHasher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Anomaly
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ApiController.CalculateGameFilesHashes(new MD5FileHasher());

            var builder = WebApplication.CreateBuilder(args);

            var securitySection = builder.Configuration.GetSection("Security");
            var securityKey = securitySection["SecurityKey"];

            ArgumentNullException.ThrowIfNull(securityKey);

            AuthOptions.Initialize(securityKey);

            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = AuthOptions.AuthTokenIssuer,
                        ValidAudience = AuthOptions.AuthTokenAudience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.SymmetricSecurityKey,
                    };
                });

            var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(dbConnectionString);
            });

            builder.Services.AddTransient<IPasswordHasher, Sha256PasswordHasher>();
            builder.Services.AddTransient<UsersRepository>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestHeadersComplementaryMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSecurityHeadersComplementaryMiddleware();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
        }
    }
}
