using Business.Services;
using Business.Services.ExamService;
using ESMS_Data.Entities.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var res = await _examService.GetCurrent();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> Get([FromQuery] ExamFilterReqModel req)
        {
            var res = await _examService.Get(req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}