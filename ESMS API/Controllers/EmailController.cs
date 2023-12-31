﻿using Business.Services.EmailService;
using ESMS_Data.Entities.EmailModel;
using ESMS_Data.Entities.RequestModel.EmailReqModel;
using ESMS_Data.Entities.RequestModel.ExamScheduleReqModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("send/delete/schedule")]
        public async Task<IActionResult> SendMailToLecturerWhenDeleteSchedule([FromBody] ExamScheduleDeleteReqModel req)
        {
            try
            {
                var examTime = await _emailService.GetExamTimeToInform(req.Idt);

                MailRequest mailRequest = new MailRequest();
                mailRequest.Subject = "Changing in the exam schedule";
                mailRequest.Body = $"Your registration in {examTime.Date.ToString("dd/MM/yyyy")} ({examTime.Start.ToString(@"hh\:mm")} - {examTime.End.ToString(@"hh\:mm")}) is cancelled. Please view the web again for newest information";

                await _emailService.SendEmailToProctorWhenDeleteSchedule(mailRequest, req.Idt, req.SubjectID, req.RoomNumber);
                return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the email." });
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("send/delete-update/time")]
        public async Task<IActionResult> SendEmailToProctorWhenDeleteAndUpdateTime([FromBody] int idt)
        {
            try
            {
                var examTime = await _emailService.GetExamTimeToInform(idt);

                MailRequest mailRequest = new MailRequest();
                mailRequest.Subject = "Changing in the exam time";
                mailRequest.Body = $"There is changing on your registration in {examTime.Date.ToString("dd/MM/yyyy")} ({examTime.Start.ToString(@"hh\:mm")} - {examTime.End.ToString(@"hh\:mm")}). Please view the web again for newest information";

                await _emailService.SendEmailToProctorWhenDeleteAndUpdateTime(mailRequest, idt);
                return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the email." });
            }
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPost]
        [Route("send/all")]
        public async Task<IActionResult> SendEmailToAllStudentAndLecturer([FromBody] SendingEmailReqModel req)
        {
            try
            {
                MailRequest mailRequest = new MailRequest();
                mailRequest.Subject = req.Subject;
                mailRequest.Body = req.Body;
                if (string.IsNullOrEmpty(req.Subject) || string.IsNullOrEmpty(req.Body))
                {
                    throw new Exception("Please input all the information!");
                }
                await _emailService.SendEmailToAllStudentAndLecturer(mailRequest);
                return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the email." });
            }
        }
    }
}
