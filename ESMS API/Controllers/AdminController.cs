using Business;
using Business.Services;
using Business.Services.UserService;
using ESMS_Data.Entities;
using ESMS_Data.Entities.RequestModel.UserReqModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
        [Route("users/update")]
        public async Task<IActionResult> Update([FromBody] UserSettingsReqModel req)
        {            
            var res = await _userService.UpdateSettings(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
