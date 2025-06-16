using System;
using System.Text.Json.Serialization;

namespace DDDSample1.Domain.Activation
{
    public class ActivationToken
    {
        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("exp")]
        public long ExpirationTime { get; set; }

        public ActivationToken(string userId, string email, int expirationMinutes)
        {
            UserId = userId;
            Email = email;
            ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes).ToUnixTimeSeconds();
        }

        [JsonConstructor]
        public ActivationToken(string userId, string email, long expirationTime)
        {
            UserId = userId;
            Email = email;
            ExpirationTime = expirationTime;
        }

        public bool IsExpired()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() > ExpirationTime;
        }
    }
}
