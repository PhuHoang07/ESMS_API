using Business;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService) {
            _userService = userService;
        }
        
        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _userService.GetAll();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
