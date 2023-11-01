using Business.Services;
using Business.Services.ExamService;
using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Entities.RequestModel.ExamScheduleReqModel;
using ESMS_Data.Entities.RequestModel.ExamTimeReqModel;
using ESMS_Data.Entities.RequestModel.ParticipationReqModel;
using ESMS_Data.Entities.RequestModel.RegistrationReqModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ESMS_API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var res = await _examService.GetCurrent();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> Get([FromQuery] ExamFilterReqModel req)
        {
            var res = await _examService.Get(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("semesters")]
        public async Task<IActionResult> GetSemesters()
        {
            var res = await _examService.GetSemesters();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var res = await _examService.GetSubjects();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms(int idt, string subjectId)
        {
            var res = await _examService.GetAvailableRooms(idt, subjectId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("time/add")]
        public async Task<IActionResult> AddTime([FromBody] ExamTimeAddReqModel req)
        {
            var res = await _examService.AddTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPatch]
        [Route("time/update")]
        public async Task<IActionResult> UpdateTime([FromBody] ExamTimeUpdateReqModel req)
        {
            var res = await _examService.UpdateTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete]
        [Route("time/delete")]
        public async Task<IActionResult> DeleteTime([FromBody] ExamTimeDeleteModel req)
        {
            var res = await _examService.DeleteTime(req.Idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("schedule/add")]
        public async Task<IActionResult> AddExamSchedule([FromBody] ExamScheduleAddReqModel req)
        {
            var res = await _examService.AddExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPatch]
        [Route("schedule/update")]
        public async Task<IActionResult> UpdateExamSchedule([FromBody] ExamScheduleUpdateReqModel req)
        {
            var res = await _examService.UpdateExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete]
        [Route("schedule/delete")]
        public async Task<IActionResult> DeleteExamSchedule([FromBody] ExamScheduleDeleteReqModel req)
        {
            var res = await _examService.DeleteExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("time/proctors")]
        public async Task<IActionResult> GetProctorListOfExamTime([FromQuery] int idt)
        {
            var res = await _examService.GetProctorListOfExamTime(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("time/proctors/add")]
        public async Task<IActionResult> AddProctorToExamTime([FromBody] RegistrationAddRemoveReqModel req)
        {
            var res = await _examService.AddProctorToExamTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete]
        [Route("time/proctors/remove")]
        public async Task<IActionResult> RemoveProctorFromExamTime([FromBody] RegistrationAddRemoveReqModel req)
        {
            var res = await _examService.RemoveProctorFromExamTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("schedule/students")]
        public async Task<IActionResult> GetStudents([FromQuery] int idt, [FromQuery] string subject, [FromQuery] string room)
        {
            var res = await _examService.GetStudents(idt, subject, room);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("schedule/students/add")]
        public async Task<IActionResult> AddStudents([FromBody] ParticipationAddRemoveReqModel req)
        {
            var res = await _examService.AddStudents(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpDelete]
        [Route("schedule/students/remove")]
        public async Task<IActionResult> RemoveStudents([FromBody] ParticipationAddRemoveReqModel req)
        {
            var res = await _examService.RemoveStudents(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}