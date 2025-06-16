using DDDSample1.Infrastructure;
using DDDSample1.Infrastructure.Shared;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DDDSample1.Configuration.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>
/// to register infrastructure-specific services.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds infrastructure-specific services to the specified <see cref="IServiceCollection"/>.
    /// This includes configuring the DbContext with MySQL, HTTP context accessor,
    /// and MVC controllers.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DDDSample1DbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 25)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()
            ).ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());

        services.AddHttpContextAccessor()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddControllers();

        return services;
    }
}
