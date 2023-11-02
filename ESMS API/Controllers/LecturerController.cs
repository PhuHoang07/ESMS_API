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
        [Route("exams")]
        public Task<IActionResult> GetExamTime()
        {
            var currentUser = GetCurrentUser();

            var res = await _examService.GetExamTimeForLecturer(currentUser.Email);
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
