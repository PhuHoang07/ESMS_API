using Business.Services.UserService;
using ESMS_Data.Entities;
using ESMS_Data.Entities.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESMS_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> GetUserDetails()
        {
            var currentUser = GetCurrentUser();

            if (currentUser != null)
            {
                var res = await _userService.GetUserDetails(currentUser.Email);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }

            return BadRequest("User authentication failed");
        }

        [Authorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UserProfileReqModel userProfileReqModel)
        {
            var currentUser = GetCurrentUser();

            if (currentUser != null)
            {
                var res = await _userService.UpdateUser(currentUser, userProfileReqModel);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }

            return BadRequest("User authentication failed");
        }

        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserModel
                {
                    Email = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value,
                };
            }

            return null;
        }
    }
}
