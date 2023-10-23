using Business.Services.ExamService;
using Business.Utils;
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
            return await Get(semester);
        }

        public async Task<ResultModel> Get(string semester, List<string> subjects = null)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var qr = _examTimeRepository.GetAll();

                if (!String.IsNullOrEmpty(semester))
                {
                    qr = _examTimeRepository.FilterSemester(qr, semester);
                }

                if (subjects.Any())
                {
                    qr = _examTimeRepository.FilterSubject(qr, subjects);
                }

                var examList = await _examTimeRepository.GroupBySemester(qr);

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
    }
}
