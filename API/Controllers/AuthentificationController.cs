using Identity.Interfaces;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthentificationController : Controller
    {
        private readonly IIdentityUserService _authSrv;

        public AuthentificationController(IIdentityUserService authSrv)
        {
            _authSrv = authSrv;
        }

        [HttpPost]
        [Route("sign-up")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> SingUpAsync([FromBody] AppUserPostDto dto)
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

        /*
        #if DEBUG
                [HttpPost]
                [Route("generate-100-test-users")]
                [AllowAnonymous]
                public async Task<ActionResult> Generate100TestUsersAsync()
                {
                    await _authSrv.Generate100TestUsersAsync();

                    return Ok();
                }
        #endif
        */

        [HttpGet]
        [Route("get-all")]
        [Authorize]
        public ActionResult<IEnumerable<AppUser>> GetAllAsync()
        {
            var result = _authSrv.GetAllAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("block-selected")]
        [Authorize]
        public IActionResult BlockUsersAsync([FromBody] IEnumerable<Guid> userIds)
        {
            _authSrv.BlockUsersAsync(userIds);

            return Ok();
        }

        [HttpDelete]
        [Route("delete-selected")]
        [Authorize]
        public IActionResult DeleteUsersAsync([FromBody] IEnumerable<Guid> userIds)
        {
            _authSrv.DeleteUsersAsync(userIds);

            return Ok();
        }
    }
}
