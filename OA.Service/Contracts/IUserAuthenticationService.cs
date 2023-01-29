using OA.Model;

namespace OA.Service.Contracts
{
    public interface IUserAuthenticationService
    {
        APIResponse UserLogin(LoginRequest request);
    }
}
