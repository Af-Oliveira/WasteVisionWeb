using System;
using System.Security.Claims;
using System.Text;

namespace DDDSample1.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static string PrintClaims(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return "ClaimsPrincipal is null";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Claims:");

            foreach (var claim in claimsPrincipal.Claims)
            {
                sb.AppendLine($"  Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            return sb.ToString();
        }

        public static void LogClaims(this ClaimsPrincipal claimsPrincipal)
        {
            Console.WriteLine(PrintClaims(claimsPrincipal));
        }

        public static string GetClaimValue(this ClaimsPrincipal user, string claimType)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.FindFirst(claimType)?.Value;
        }


    }
}
