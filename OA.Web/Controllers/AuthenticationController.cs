using Microsoft.AspNetCore.Mvc;
using OA.Model;
using OA.Repo.Contracts.Common;
using OA.Repo.Repositories.Common;
using OA.Service.Contracts;
using OA.Utility;
using OA.Web.Helper;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OA.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserAuthenticationService _authService;
        private readonly IExtendedHubHelper _extendedHub;
        private IUtilityServiceProvider _utilityProvider;

        public AuthenticationController(IUserAuthenticationService authService, IUtilityServiceProvider utilityProvider, IExtendedHubHelper extendedHub)
        {
            _authService = authService;
            _utilityProvider = utilityProvider;
            _extendedHub = extendedHub;
        }        

        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> UserLogin(LoginRequest request)
        {
            var responseModel = new APIResponse();
            try
            {               
                responseModel = _authService.UserLogin(request);

                string stattusCode = responseModel.StatusCode == 200 ? "Succeeded" : "Failed";
                _extendedHub.SendOutMessage(responseModel.Message, stattusCode);
            }
            catch (Exception ex)
            {
                responseModel = new APIResponse
                {
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            return Ok(responseModel);
        }



    }
}
