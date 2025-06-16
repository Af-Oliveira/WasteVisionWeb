using DDDSample1.Configuration.DependencyInjection;
using DDDSample1.Domain.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace DDDSample1;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddSingleton<ILogManager>(
            new LogManager(Path.Combine(Directory.GetCurrentDirectory(), "mylogs.csv")));

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Set-Cookie", "Authorization")
                    .SetIsOriginAllowed(origin => true);
            });
        });

        // Add application services grouped by concern
        builder.Services
            .AddInfrastructure(builder.Configuration)
            .AddSecurityServices(builder.Configuration)
            .AddSwaggerServices()
            .AddDomainServices();

        // Configure the HTTP request pipeline
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Serve static files from wwwroot (including wwwroot/uploads)
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Unspecified,
            Secure = CookieSecurePolicy.None,
            HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
            OnAppendCookie = cookieContext =>
            {
                cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
                cookieContext.CookieOptions.Secure = false;
            }
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseSwaggerMiddleware();

        app.Run();
    }
}