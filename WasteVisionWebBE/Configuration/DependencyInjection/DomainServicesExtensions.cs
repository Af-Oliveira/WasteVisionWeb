using DDDSample1.Application.Services;
using DDDSample1.Domain.Activation;
using DDDSample1.Domain.Detections;

using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Infrastructure;
using DDDSample1.Infrastructure.RoboflowModels;
using DDDSample1.Infrastructure.Roles;
using DDDSample1.Infrastructure.Users;
using DDDSample1.Infrastructure.Predictions;
using Microsoft.Extensions.DependencyInjection;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Infrastructure.ObjectPredictions;
using System;


namespace DDDSample1.Configuration.DependencyInjection;

public static class DomainServicesExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        // Event and messaging infrastructure
        services.AddScoped<IExceptionHandler, DatabaseExceptionHandler>();


            // Configure HttpClient
        services.AddHttpClient("DetectionClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("User-Agent", "WasteVisionApp/1.0");
        });

    // Register your services
    services.AddScoped<IDetection, Detection>();

        // Core services
        services.AddTransient<IUnitOfWork, UnitOfWork>()
                .AddScoped<IAuthenticationService, AuthenticationService>();

        // Repositories
        AddRepositories(services);

        // Domain services
        AddDomainEntityServices(services);

        // Application services
        AddApplicationServices(services);


        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddTransient<IRoleRepository, RoleRepository>()
                .AddTransient<IObjectPredictionRepository, ObjectPredictionRepository>()
                .AddTransient<IPredictionRepository, PredictionRepository>()
                .AddTransient<IRoboflowModelRepository, RoboflowModelRepository>()
                .AddTransient<IUserRepository, UserRepository>();
    }

    private static void AddDomainEntityServices(IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>()
                .AddScoped<IObjectPredictionService, ObjectPredictionService>()
                .AddScoped<IPredictionService, PredictionService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IImageProcessorService, ImageProcessorService>()
                .AddScoped<IDetection, Detection>()
                .AddScoped<IRoboflowModelService, RoboflowModelService>()
                .AddScoped<IRoboflowModelInfoService, RoboflowModelInfoService>()
                .AddTransient<IDetectionService, DetectionService>()
                .AddHostedService<TimedBackgroundService>();
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IActivationTokenService, ActivationTokenService>();
    }
}