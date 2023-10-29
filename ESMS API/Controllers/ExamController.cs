﻿using Business.Services;
using Business.Services.ExamService;
using ESMS_Data.Entities.RequestModel;
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
        public async Task<IActionResult> GetSemester()
        {
            var res = await _examService.GetSemester();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("subjects")]
        public async Task<IActionResult> GetSubject()
        {
            var res = await _examService.GetSubject();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("get-available-room")]
        public async Task<IActionResult> GetAvailableRoom(int idt)
        {
            var res = await _examService.GetAvailableRoom(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("add-time")]
        public async Task<IActionResult> AddTime([FromBody] ExamTimeAddReqModel req)
        {
            var res = await _examService.AddTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("update-time")]
        public async Task<IActionResult> UpdateTime([FromBody] ExamTimeUpdReqModel req)
        {
            var res = await _examService.UpdateTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("delete-time")]
        public async Task<IActionResult> DeleteTime([FromBody] ExamTimeDelModel req)
        {
            var res = await _examService.DeleteTime(req.Idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("add-exam-schedule")]
        public async Task<IActionResult> AddExamSchedule([FromBody] ExamScheduleAddReqModel req)
        {
            var res = await _examService.AddExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        
        //[Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("update-exam-schedule")]
        public async Task<IActionResult> UpdateExamSchedule([FromBody] ExamScheduleUpdReqModel req)
        {
            var res = await _examService.UpdateExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        
    }
}