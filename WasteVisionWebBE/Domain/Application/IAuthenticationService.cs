using DDDSample1.Domain.Application;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DDDSample1.Application.Services
{
    public interface IAuthenticationService
    {
        IActionResult ChallengeAsync(string? clientRootURL = null, string? activationToken = null);
        Task<IActionResult> LogoutAsync();
        Task<IActionResult> HandleCallbackAsync(string? clientRootURL = null, string? activationToken = null);
        Task<IActionResult> GetUserInfoAsync();
        Task<ProfileDto> GetProfileDtoFromLoggedInUserAsync();

        Task<IActionResult> UpdateUserInfo(ProfileDtoForUpdate profileDtoForUpdate);
        Task<IActionResult> RequestUserDeletion();
        Task<IActionResult> ConfirmUserDeletion();
    }
}