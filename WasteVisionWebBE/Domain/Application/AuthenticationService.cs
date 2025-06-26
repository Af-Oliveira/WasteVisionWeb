using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Security.Claims;
using DDDSample1.Application.Shared;
using System.Linq;
using DDDSample1.Domain.Activation;
using DDDSample1.Domain.Application;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.Roles;

namespace DDDSample1.Application.Services
{
    /// <summary>
    /// Handles authentication-related operations including OAuth2/OpenID Connect flows.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IRoleService _roleService;
        private readonly ILogManager _logManager;

        public AuthenticationService(
            IConfiguration configuration,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IRoleService roleService,
            ILogManager logManager
        )
        {
            _configuration = configuration;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _roleService = roleService;
            _logManager = logManager;
        }

        private IUrlHelper UrlHelper => _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        public IActionResult ChallengeAsync(string? clientRootURL = null, string? activationToken = null)
        {
            Console.WriteLine($"Received clientRootURL in login: {clientRootURL ?? "not provided"}");
            Console.WriteLine($"Received activationToken in login: {activationToken ?? "not provided"}");

            var properties = new AuthenticationProperties
            {
                RedirectUri = string.IsNullOrEmpty(clientRootURL)
                    ? $"{UrlHelper.Action("Callback", "Auth")}?activationToken={activationToken ?? ""}"
                    : $"{UrlHelper.Action("Callback", "Auth")}?clientRootURL={Uri.EscapeDataString(clientRootURL)}&activationToken={activationToken ?? ""}",
                Items =
        {
            { "prompt", "login" },
        },
                Parameters = { { "prompt", "login" } }
            };

            return new ChallengeResult("OpenIdConnect", properties);
        }

       
        public async Task<IActionResult> HandleCallbackAsync(string? clientRootURL = null, string? activationToken = null)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

                // Log all claims for debugging
            

                activationToken ??= context.Items["ActivationToken"]?.ToString() ??
                                context.Request.Query["activationToken"].ToString();

                clientRootURL ??= context.Items["ClientRootURL"]?.ToString() ??
                                    context.Request.Query["clientRootURL"].ToString();

                if (string.IsNullOrEmpty(email))
                {
                    return ApiResponse.For<bool>().WithMessage("Email not in claims").AsError().Build(StatusCodeEnum.BadRequestError);
                }

                if (!string.IsNullOrEmpty(activationToken))
                {
                    // Activation token handling code...
                    // ...existing code...
                }

                var dbUser = await _userService.GetByEmailWithRoleAsync(email);
                Console.WriteLine($"User lookup result: {(dbUser == null ? "null" : dbUser.Id)}");

                // Move the null check before accessing dbUser properties
                if (dbUser == null)
                {
                    // Time to create a new user
                    var roleUser = await _roleService.GetRoleByDescriptionAsync(new Description("User"));
                    if (roleUser == null)
                    {
                        return ApiResponse.For<bool>().WithMessage("Role not found").AsError().Build(StatusCodeEnum.NotFoundError);
                    }

                    var username = email.Split('@')[0];
                    username = System.Text.RegularExpressions.Regex.Replace(username, @"[^a-zA-Z0-9]", "");

                    try
                    {
                        var newUser = await _userService.CreateAsync(
                            new CreatingUserDto(email, username, roleUser.Id.ToString())
                        );
                           



                        // Get the newly created user
                        dbUser = await _userService.GetByEmailWithRoleAsync(email);
                        Console.WriteLine($"New user created: {(dbUser == null ? "null" : dbUser.Id)}");

                        if (dbUser == null)
                        {
                            return ApiResponse.For<bool>().WithMessage("User not found after creation").AsError().Build(StatusCodeEnum.NotFoundError);
                        }

                        // Get user information from claims
                        var fullName = context.User.FindFirst("name")?.Value ??
                                       context.User.FindFirst(ClaimTypes.Name)?.Value ??
                                       context.User.FindFirst("preferred_username")?.Value ??
                                       email;
                        Console.WriteLine($"Using fullName: {fullName}");

                      
                    }
                    catch (Exception userEx)
                    {
                        Console.WriteLine($"Error creating or updating user: {userEx.Message}");
                        Console.WriteLine(userEx.StackTrace);
                        return ApiResponse.For<bool>().WithMessage($"Error creating user: {userEx.Message}").AsError().Build(StatusCodeEnum.ServerError);
                    }
                }

                // Safety check - dbUser should never be null at this point
                if (dbUser == null)
                {
                    Console.WriteLine("Critical error: dbUser is null after creation attempt");
                    return ApiResponse.For<bool>().WithMessage("User not found or created properly").AsError().Build(StatusCodeEnum.ServerError);
                }

              

                // Sign in the user
                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, dbUser.Id),
                new Claim(ClaimTypes.Email, dbUser.Email),
                new Claim(ClaimTypes.Role, dbUser.RoleName),
                    }, "custom"))
                );

                // Rest of the method remains the same
                var cookieData = new Dictionary<string, string>();
                foreach (var cookie in context.Request.Cookies)
                {
                    cookieData.Add(cookie.Key, cookie.Value);
                }

                // Serialize cookies to JSON
                var cookieJson = System.Text.Json.JsonSerializer.Serialize(cookieData);
                var encodedCookieJson = Uri.EscapeDataString(cookieJson);

                // Check if it's a mobile app request (you can identify this by URL pattern or query param)
                bool isMobileApp = !string.IsNullOrEmpty(clientRootURL) && clientRootURL.Contains("exp");
                clientRootURL = _configuration["Urls:ReactClient"];

                // debug cookies

                if (isMobileApp)
                {
                    // For mobile, redirect with JWT token
                    Console.WriteLine($"Redirecting to: {clientRootURL}?success=true&mobile=1");
                    return new RedirectResult($"{clientRootURL}?success=true&mobile=1");
                }
                else
                {
                    // For web, continue with existing cookie authentication
                    return !string.IsNullOrEmpty(clientRootURL)
                        ? new RedirectResult($"{clientRootURL}?success=true")
                        : ApiResponse.For(new
                        {
                            success = true,
                            message = "Authentication successful",
                            cookies = cookieData
                        }).Build(StatusCodeEnum.Success);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HandleCallbackAsync global exception: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return !string.IsNullOrEmpty(clientRootURL)
                    ? new RedirectResult($"{clientRootURL}?success=false&error={Uri.EscapeDataString(ex.Message)}")
                    : ApiResponse.For<bool>().WithMessage(ex.Message).AsError().Build(StatusCodeEnum.ServerError);
            }
        }



        /// <summary>
        /// Performs user logout from both local and OpenID provider sessions.
        /// </summary>
        /// <returns>Logout URL for client-side redirect.</returns>
        public async Task<IActionResult> LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var userEmail = httpContext.User?.Claims?
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?
                .Value ?? "Unknown user";

            // Sign out from our application's cookie authentication
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out from OpenId Connect
            await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // Construct Keycloak logout URL without id_token_hint
            var keycloakLogoutUrl = $"{_configuration["Keycloak:auth-server-url"]}realms/{_configuration["Keycloak:realm"]}/protocol/openid-connect/logout";

            // Add redirect_uri parameter
            var redirectUri = UrlHelper.Action("Login", "Auth", null, httpContext.Request.Scheme, httpContext.Request.Host.ToUriComponent());
            var logoutUrl = $"{keycloakLogoutUrl}?redirect_uri={Uri.EscapeDataString(redirectUri)}";

            // Clear any additional cookies or session data if needed
            foreach (var cookie in httpContext.Request.Cookies.Keys)
            {
                httpContext.Response.Cookies.Delete(cookie);
            }

            if (httpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                _logManager.Write(LogType.Auth, $"User {userEmail} logged out");
            }
            return ApiResponse.For(logoutUrl).WithMessage("User logged out successfully.").Build(StatusCodeEnum.Success);
        }


        /// <summary>
        /// Retrieves authenticated user information including claims.
        /// </summary>
        /// <returns>User details and associated claims.</returns>
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return ApiResponse.For<object>().WithMessage("Email not provided in the authentication claims.").AsError().Build(StatusCodeEnum.BadRequestError);
            }

            try
            {
                var userDto = await _userService.GetByEmailWithRoleAsync(email);
                if (userDto == null)
                {
                    return ApiResponse.For<object>().WithMessage("User not found in database.").AsError().Build(StatusCodeEnum.NotFoundError);
                }

                ProfileDto response = await GetProfileDtoFromLoggedInUserAsync();

                return ApiResponse.For(response).WithMessage("User information retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetUserInfoAsync: {ex.Message}");
                return ApiResponse.For<object>().WithMessage("An error occurred while retrieving user information.").AsError().Build(StatusCodeEnum.ServerError);
            }
        }

        public async Task<ProfileDto> GetProfileDtoFromLoggedInUserAsync()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email not provided in the authentication claims.");
            }

            var userDto = await _userService.GetByEmailWithRoleAsync(email);
            if (userDto == null)
            {
                throw new Exception("User not found in database.");
            }

            ProfileDto response = ProfileMapper.ToDto(userDto);



            return response;
        }

        public Task<IActionResult> UpdateUserInfo(ProfileDtoForUpdate profileDtoForUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> RequestUserDeletion()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ConfirmUserDeletion()
        {
            throw new NotImplementedException();
        }
    }
}