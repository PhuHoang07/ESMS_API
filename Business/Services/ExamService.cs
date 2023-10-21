using Business.Interfaces;
using ESMS_Data.Interfaces;
using ESMS_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamTimeRepository _examTimeRepository;
        public ExamService(IExamTimeRepository examTimeRepository)
        {
            _examTimeRepository = examTimeRepository;
        }

        public async Task<ResultModel> GetAll()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var examList = await _examTimeRepository.GetAll();

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
