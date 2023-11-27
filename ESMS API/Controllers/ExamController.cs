using Business.Services;
using Business.Services.ExamService;
using ClosedXML.Excel;
using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Entities.RequestModel.ExamScheduleReqModel;
using ESMS_Data.Entities.RequestModel.ExamTimeReqModel;
using ESMS_Data.Entities.RequestModel.ParticipationReqModel;
using ESMS_Data.Entities.RequestModel.RegistrationReqModel;
using ESMS_Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
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

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpGet]
        [Route("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms(int idt)
        {
            var res = await _examService.GetAvailableRooms(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPost]
        [Route("time/add")]
        public async Task<IActionResult> AddTime([FromBody] ExamTimeAddReqModel req)
        {
            var res = await _examService.AddTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPatch]
        [Route("time/update")]
        public async Task<IActionResult> UpdateTime([FromBody] ExamTimeUpdateReqModel req)
        {
            var res = await _examService.UpdateTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpDelete]
        [Route("time/delete")]
        public async Task<IActionResult> DeleteTime([FromBody] ExamTimeDeleteModel req)
        {
            var res = await _examService.DeleteTime(req.Idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("time/export-excel")]
        public async Task<IActionResult> ExportToExcel(int idt)
        {
            try
            {
                var dataTableList = await _examService.ExportToExcel(idt);
                var examTime = await _examService.GetExamTimeInfo(idt);
                var proctorList = await _examService.GetProctorList(idt);
                var count = 0;

                if (proctorList.Count < dataTableList.Count)
                {
                    var difference = dataTableList.Count - proctorList.Count;
                    if (difference == 1)
                    {
                        return BadRequest(new
                        {
                            message = "There is 1 exam schedule that is not assigned to any proctors."
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message = $"There is {difference} exam schedules that are not assigned to any proctors."
                        });
                    }

                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (var examSchedule in dataTableList)
                    {
                        var sheet = wb.AddWorksheet(examSchedule, examSchedule.TableName);
                        sheet.Row(1).InsertRowsAbove(4);
                        sheet.Cell(1, 1).Value = $"Date: {examTime.Date.ToString("dd/MM/yyyy")}";
                        sheet.Cell(2, 1).Value = $"Time: {examTime.Start.ToString(@"hh\:mm")} - {examTime.End.ToString(@"hh\:mm")}";
                        sheet.Cell(3, 1).Value = $"Proctor: {proctorList[count]}";
                        count++;

                        sheet.Columns().AdjustToContents();
                        sheet.Style.Fill.BackgroundColor = XLColor.White;
                        sheet.Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);

                        for (int rowNumber = 5; rowNumber <= sheet.Rows().Count(); rowNumber++)
                        {
                            for (int colNumber = 1; colNumber <= Math.Min(5, sheet.Columns().Count()); colNumber++)
                            {
                                var cell = sheet.Cell(rowNumber, colNumber);
                                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                                // Optionally, set a specific color for the borders
                                cell.Style.Border.TopBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                                cell.Style.Border.BottomBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                                cell.Style.Border.LeftBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                                cell.Style.Border.RightBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                            }
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                            $"{examTime.Date.ToString("dd-MM-yyyy")} ({examTime.Start.ToString(@"hh\-mm")}_{examTime.End.ToString(@"hh\-mm")})");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "An error occurred during Excel export."
                });
            }

        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPatch]
        [Route("time/public")]
        public async Task<IActionResult> PublicizeExamTime([FromBody] List<int> idt)
        {
            var res = await _examService.SetIsPublicExamTime(idt, true, "Publicize successfully!");
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpPatch]
        [Route("time/private")]
        public async Task<IActionResult> PrivatizeExamTime([FromBody] List<int> idt)
        {
            var res = await _examService.SetIsPublicExamTime(idt, false, "Privatize successfully!");
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin, Testing Staff")]
        [HttpGet]
        [Route("time/proctors")]
        public async Task<IActionResult> ViewProctorList(int idt)
        {
            var res = await _examService.ViewProctorList(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPost]
        [Route("time/proctors/add")]
        public async Task<IActionResult> AddProctorToExamTime([FromBody] RegistrationAddRemoveReqModel req)
        {
            var res = await _examService.AddProctorToExamTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpDelete]
        [Route("time/proctors/remove")]
        public async Task<IActionResult> RemoveProctorFromExamTime([FromBody] RegistrationAddRemoveReqModel req)
        {
            var res = await _examService.RemoveProctorFromExamTime(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }



        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPost]
        [Route("schedule/add")]
        public async Task<IActionResult> AddExamSchedule([FromBody] ExamScheduleAddReqModel req)
        {
            var res = await _examService.AddExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPatch]
        [Route("schedule/update")]
        public async Task<IActionResult> UpdateExamSchedule([FromBody] ExamScheduleUpdateReqModel req)
        {
            var res = await _examService.UpdateExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPatch]
        [Route("schedule/update-proctor")]
        public async Task<IActionResult> UpdateProctorInExamSchedule([FromBody] ExamScheduleUpdateProctorReqModel req)
        {
            var res = await _examService.UpdateProctorInExamSchedule(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpDelete]
        [Route("schedule/delete")]
        public async Task<IActionResult> DeleteExamSchedule([FromBody] ExamScheduleDeleteReqModel req)
        {
            var res = await _examService.DeleteExamSchedule(req);
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

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPost]
        [Route("schedule/students/add")]
        public async Task<IActionResult> AddStudents([FromBody] ParticipationAddRemoveReqModel req)
        {
            var res = await _examService.AddStudents(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpDelete]
        [Route("schedule/students/remove")]
        public async Task<IActionResult> RemoveStudents([FromBody] ParticipationAddRemoveReqModel req)
        {
            var res = await _examService.RemoveStudents(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpGet]
        [Route("schedule/proctors/unassigned")]
        public async Task<IActionResult> GetUnassignedProctorOfExamTime([FromQuery] int idt)
        {
            var res = await _examService.GetUnassignedProctorOfExamTime(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Admin, Testing Admin")]
        [HttpPatch]
        [Route("schedule/proctors/random")]
        public async Task<IActionResult> UpdateProctorsToExamSchedule([FromBody] int idt)
        {
            var res = await _examService.UpdateProctorsToExamSchedule(idt);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }




    }
}