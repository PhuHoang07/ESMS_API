using Business;
using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        public AdminController(IAdminService service) {
            _service = service;
        }
        
        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAll();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
