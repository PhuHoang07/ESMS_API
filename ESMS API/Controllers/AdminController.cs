using Business;
using Business.Interfaces;
using Business.Services;
using ESMS_Data.Entities.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        public AdminController(IAdminService service) {
            _service = service;
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUserList()
        {
            var res = await _service.GetUserList();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        [Route("users/find")]
        public async Task<IActionResult> FindByUserName([FromQuery] String username)
        {
            var res = await _service.FindByUserName(username);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        [Route("users/{userName}")]
        public async Task<IActionResult> GetUserDetails(String userName)
        {
            var res = await _service.GetUserDetails(userName);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        [Route("users/update")]
        public async Task<IActionResult> SetRole([FromBody] UserReqModel req)
        {            
            var res = await _service.SetRole(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
