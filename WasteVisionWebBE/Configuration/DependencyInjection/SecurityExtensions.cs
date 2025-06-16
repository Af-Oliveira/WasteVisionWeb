using DDDSample1.Domain.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace DDDSample1.Configuration.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>
/// to configure security-related services, including authentication and authorization.
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Adds and configures security services, including authentication (Cookie and OpenID Connect)
    /// and authorization policies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add security services to.</param>
    /// <param name="configuration">The application configuration, used for Keycloak settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            // Use cookie authentication as the default and only scheme
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(ConfigureCookie)
        .AddOpenIdConnect(options => ConfigureOpenIdConnect(options, configuration));

        services.AddAuthorization(options =>
        {
            // Set up the default policy to use only cookie authentication
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

            // Role-based policies
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(RolesConstants.Admin));
            options.AddPolicy("AdminOrUser", policy =>
                policy.RequireRole(RolesConstants.Admin, RolesConstants.User));
            options.AddPolicy("AnyRole", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole(RolesConstants.Admin) ||
                    context.User.IsInRole(RolesConstants.User)));
        });

        return services;
    }

    /// <summary>
    /// Configures cookie authentication options.
    /// </summary>
    /// <param name="options">The <see cref="CookieAuthenticationOptions"/> to configure.</param>
    private static void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.Cookie = new CookieBuilder
        {
            Name = ApiConstants.CookieName,
            HttpOnly = true,
            SameSite = SameSiteMode.Unspecified, // Consider SameSiteMode.Lax or SameSiteMode.Strict for better security if applicable
            SecurePolicy = CookieSecurePolicy.None, // Should be CookieSecurePolicy.Always in production (HTTPS)
            IsEssential = true,
            Path = "/"
        };

        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    }

    /// <summary>
    /// Configures OpenID Connect authentication options, typically for integration with an
    /// identity provider like Keycloak.
    /// </summary>
    /// <param name="options">The <see cref="OpenIdConnectOptions"/> to configure.</param>
    /// <param name="configuration">The application configuration containing Keycloak settings.</param>
    private static void ConfigureOpenIdConnect(
        OpenIdConnectOptions options,
        IConfiguration configuration)
    {
        var keycloakUrl = configuration["Keycloak:auth-server-url"];
        var realm = configuration["Keycloak:realm"];

        options.Authority = $"{keycloakUrl}realms/{realm}";
        options.ClientId = configuration["Keycloak:resource"];
        options.ClientSecret = configuration["Keycloak:credentials:secret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        // Add these lines to handle correlation
        // In production, ensure these are set to CookieSecurePolicy.Always if using HTTPS
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;
        options.CorrelationCookie.SameSite = SameSiteMode.Unspecified; // Consider SameSiteMode.None if cross-site requests are necessary and secure
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.None;
        options.NonceCookie.SameSite = SameSiteMode.Unspecified; // Consider SameSiteMode.None

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name", // Or "preferred_username" depending on Keycloak configuration
            RoleClaimType = "role", // Ensure Keycloak is configured to provide roles in this claim
            ValidateIssuer = true
        };

        options.RequireHttpsMetadata = false; // Should be true in production

        // Modify the events
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                // Only redirect to IdP for auth-specific paths, otherwise return 401
                if (!context.HttpContext.Request.Path
                    .StartsWithSegments("/api/auth")) // Adjust path as needed
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.HandleResponse();
                }
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // This is a workaround for potential correlation issues.
                // Investigate the root cause of correlation errors if they occur.
                if (context.ProtocolMessage != null)
                {
                    context.ProtocolMessage.Parameters.Remove("correlation_id");
                }
                return Task.CompletedTask;
            }
        };
    }
}
