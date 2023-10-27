using Business.Services.ExamService;
using Business.Utils;
using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Repositories.ExamTimeRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.ExamService
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examTimeRepository;
        private readonly Utils.Utils utils;
        public ExamService(IExamRepository examTimeRepository)
        {
            _examTimeRepository = examTimeRepository;
            utils = new Utils.Utils();
        }

        public async Task<ResultModel> GetCurrent()
        {
            string semester = utils.GetCurrentSemester();
            return await Get(new ExamFilterReqModel { Semester = semester });
        }

        public async Task<ResultModel> Get(ExamFilterReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var qr = _examTimeRepository.GetAll();

                if (!String.IsNullOrEmpty(req.Semester))
                {
                    qr = _examTimeRepository.FilterSemester(qr, req.Semester);
                }

                if (req.Subjects != null)
                {
                    qr = _examTimeRepository.FilterSubject(qr, req.Subjects);
                }

                qr = _examTimeRepository.FilterDate(qr, req.From, req.To);

                qr = _examTimeRepository.FilterTime(qr, req.Start, req.End);

                var examList = await _examTimeRepository.GroupBySemester(qr);

                var currentSemester = utils.GetCurrentSemester();
                // Inline equal care about sensitive case
                if (!examList.ContainsKey(currentSemester) && (req.Semester ?? "").ToUpper().Equals(currentSemester))
                {
                    examList.Add(currentSemester, new List<object>());
                }

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = examList;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public ResultModel GetSemester()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var semesters = _examTimeRepository.GetSemester();

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = semesters;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }
    }
}
