using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace DDDSample1.Configuration.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> and <see cref="IApplicationBuilder"/>
/// to configure Swagger (OpenAPI) documentation.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger generation services to the specified <see cref="IServiceCollection"/>.
    /// Configures Swagger document information and XML comments integration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(ApiConstants.SwaggerVersion, new OpenApiInfo
            {
                Title = ApiConstants.SwaggerTitle,
                Version = ApiConstants.SwaggerVersion,
                Description = "DDDSample1 API Documentation",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "team@example.com"
                }
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) // Check if the XML file exists before including
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Configures the application to use Swagger and Swagger UI.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseSwaggerMiddleware(
        this IApplicationBuilder app)
    {
        return app.UseSwagger()
                  .UseSwaggerUI(c =>
                    c.SwaggerEndpoint(
                        $"/swagger/{ApiConstants.SwaggerVersion}/swagger.json",
                        $"{ApiConstants.SwaggerTitle} {ApiConstants.SwaggerVersion}"
                    ));
    }
}
