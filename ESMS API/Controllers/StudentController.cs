using Business.Services.StudentService;
using ESMS_Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESMS_API.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        //[Authorize(Roles = "Student")]
        [HttpGet]
        [Route("exams/schedule/own")]
        public async Task<IActionResult> GetRegisteredExamTimes()
        {
            var currentUser = GetCurrentUser();

            var res = await _studentService.GetAssignedExamSchedules(currentUser.Email);
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
