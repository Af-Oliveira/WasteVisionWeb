using DDDSample1.Application.Services;
using DDDSample1.Domain.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DDDSample1.Controllers
{
    /*
    // Single role
    [Authorize(Roles = RoleConstants.Admin)]
    // Multiple roles (OR condition)
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Doctor}")]
    // Using policies
    [Authorize(Policy = "AdminOnly")]
    */

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string? clientRootURL = null, [FromQuery] string? activationToken = null)
        {
            Console.WriteLine($"Received clientRootURL in login: {clientRootURL ?? "not provided"}");
            Console.WriteLine($"Received activationToken in login: {activationToken ?? "not provided"}");
            return _authenticationService.ChallengeAsync(clientRootURL, activationToken);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string? clientRootURL = null, [FromQuery] string? activationToken = null)
        {
            Console.WriteLine($"Received clientRootURL in callback: {clientRootURL ?? "not provided"}");
            Console.WriteLine($"Received activationToken in callback: {activationToken ?? "not provided"}");
            return await _authenticationService.HandleCallbackAsync(clientRootURL, activationToken);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout() => await _authenticationService.LogoutAsync();

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> RequestUserDeletion() => await _authenticationService.RequestUserDeletion();

        [HttpGet("me/confirm-delete")]
        [Authorize]
        public async Task<IActionResult> ConfirmUserDeletion() => await _authenticationService.ConfirmUserDeletion();

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> UserInfo() => await _authenticationService.GetUserInfoAsync();

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo([FromBody] ProfileDtoForUpdate updatePatientApiDto) => await _authenticationService.UpdateUserInfo(updatePatientApiDto);
    }
}



