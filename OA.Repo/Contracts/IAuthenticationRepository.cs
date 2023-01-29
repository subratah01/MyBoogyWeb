using OA.Model;

namespace OA.Repo.Contracts
{
    public interface IAuthenticationRepository 
    {
        APIResponse UserLogin(LoginRequest request);
        string GenerateJwt(string Email, string Role, string JwtKey, string Issuer, string Audience);
        
    }
}
