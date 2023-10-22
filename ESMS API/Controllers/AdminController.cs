using Business;
using Business.Services;
using Business.Services.AdminService;
using ESMS_Data.Entities;
using ESMS_Data.Entities.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) {
            _adminService = adminService;
        }
        /// <summary>
        /// Get all account list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUserList()
        {
            var res = await _adminService.GetUserList();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        [Route("users/find")]
        public async Task<IActionResult> GetUserList([FromQuery] String username)
        {
            var res = await _adminService.GetUserList(username);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        [Route("users/{userName}")]
        public async Task<IActionResult> GetUserDetails(String userName)
        {
            var res = await _adminService.GetUserDetails(userName);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        [Route("users/update")]
        public async Task<IActionResult> SetRole([FromBody] UserReqModel req)
        {            
            var res = await _adminService.SetRole(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
