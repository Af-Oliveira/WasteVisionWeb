namespace DDDSample1.Domain.Activation
{
    public interface IActivationTokenService
    {
        string GenerateToken(string userId, string email);
        ActivationToken ValidateToken(string token);
    }
}
