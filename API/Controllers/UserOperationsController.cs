using API.Interfaces;
using API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class UserOperationsController : Controller
    {
        private readonly IUserOperationsService _userOpSrv;

        public UserOperationsController(IUserOperationsService userOpSrv)
        {
            _userOpSrv = userOpSrv;
        }

        [HttpGet]
        [Route("get-all")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppUserGetDto>>> GetAllAsync()
        {
            if (await _userOpSrv.CheckUserStatus(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
                return BadRequest("You are blocked");

            var result = await _userOpSrv.GetAllAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("change-users-status")]
        [Authorize]
        public async Task<IActionResult> ChangeUsersStatusAsync([FromBody] ChangeUsersStatusDto dto)
        {
            if (await _userOpSrv.CheckUserStatus(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
                return BadRequest("You are blocked");

            await _userOpSrv.ChangeUsersStatusAsync(dto);

            return Ok();
        }

        [HttpDelete]
        [Route("delete-selected")]
        [Authorize]
        public async Task<IActionResult> DeleteUsersAsync([FromQuery] IEnumerable<Guid> userIds)
        {
            if (await _userOpSrv.CheckUserStatus(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
                return BadRequest("You are blocked");

            await _userOpSrv.DeleteUsersAsync(userIds);

            return Ok();
        }

#if DEBUG
        [HttpPost]
        [Route("generate-100-test-users")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate100TestUsersAsync()
        {
            await _userOpSrv.Generate100TestUsersAsync();

            return Ok();
        }
#endif
    }
}
