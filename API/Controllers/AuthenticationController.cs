using API.Models.Dto;
using Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IIdentityUserService _authSrv;

        public AuthenticationController(IIdentityUserService authSrv)
        {
            _authSrv = authSrv;
        }

        [HttpPost]
        [Route("sign-up")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> SingUpAsync([FromBody] RegistrationDto dto)
        {
            var result = await _authSrv.RegistrateUserAsync(dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("sign-in")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> SignInAsync([FromBody] LoginDto dto)
        {
            var result = await _authSrv.LoginAsync(dto);

            return Ok(result);
        }
    }
}
