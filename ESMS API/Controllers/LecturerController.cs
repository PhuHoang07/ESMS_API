using Business.Services.LecturerService;
using ESMS_Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESMS_API.Controllers
{
    [Route("api/lecturer")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;

        public LecturerController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        [Route("exams/registered")]
        public async Task<IActionResult> GetRegisteredExamTimes()
        {
            var currentUser = GetCurrentUser();

            var res = await _lecturerService.GetRegisteredExamTimes(currentUser.Email);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        [Route("exams/available")]
        public async Task<IActionResult> GetAvailableExamTimes()
        {
            var currentUser = GetCurrentUser();

            var res = await _lecturerService.GetAvailableExamTimes(currentUser.Email);
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
