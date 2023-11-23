using Business;
using Business.Services;
using Business.Services.UserService;
using ESMS_Data.Entities.RequestModel.UserReqModel;
using ESMS_Data.Entities.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESMS_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService) {
            _userService = userService;
        }
   
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUserList()
        {
            var res = await _userService.GetUserList();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("users/search")]
        public async Task<IActionResult> GetUserList([FromQuery] string? username)
        {
            var res = await _userService.GetUserList(username);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("users/{userName}")]
        public async Task<IActionResult> GetUserDetails(string userName)
        {
            var res = await _userService.GetUserDetails(userName);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("users/update")]
        public async Task<IActionResult> Update([FromBody] UserSettingsReqModel req)
        {
            var currentUser = GetCurrentUser();
            
            var res = await _userService.UpdateSettings(currentUser.Email, req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
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
