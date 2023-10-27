using Business.Services.ExamService;
using Business.Utils;
using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Repositories.ExamRepository;
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
        private readonly IExamRepository _examRepository;
        private readonly Utils.Utils utils;
        public ExamService(IExamRepository examRepository)
        {
            _examRepository = examRepository;
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
                var qr = _examRepository.GetAll();

                if (!String.IsNullOrEmpty(req.Semester))
                {
                    qr = _examRepository.FilterSemester(qr, req.Semester);
                }

                if (req.Subjects != null)
                {
                    qr = _examRepository.FilterSubject(qr, req.Subjects);
                }

                qr = _examRepository.FilterDate(qr, req.From, req.To);

                qr = _examRepository.FilterTime(qr, req.Start, req.End);

                var examList = await _examRepository.GroupBySemester(qr);

                if (!String.IsNullOrEmpty(req.Semester) && !examList.ContainsKey(req.Semester.ToUpper()))
                {
                    examList.Add(req.Semester.ToUpper(), new List<object>());
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

        public async Task<ResultModel> GetSemester()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var semesters = await _examRepository.GetSemester();

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

        public async Task<ResultModel> GetSubject()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var subjects = await _examRepository.GetSubject();

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = subjects;
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
