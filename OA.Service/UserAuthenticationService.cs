using OA.Model;
using OA.Repo.Contracts;
using OA.Service.Contracts;

namespace OA.Service
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private IAuthenticationRepository _authRepository;
        public UserAuthenticationService(IAuthenticationRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public APIResponse UserLogin(LoginRequest request)
        {
            return _authRepository.UserLogin(request);
        }

        
    }
}
