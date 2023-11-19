using Business.Services.StaffService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("allowance")]
        public async Task<IActionResult> GetAllowanceStatistic()
        {
            var res = await _staffService.GetAllowanceStatistic();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}
