using Business.Interfaces;
using Business.Utils;
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
        private readonly Utils.Utils utils;
        public ExamService(IExamTimeRepository examTimeRepository)
        {
            _examTimeRepository = examTimeRepository;
            utils = new Utils.Utils();
        }        

        public async Task<ResultModel> GetCurrent()
        {
            String semester = utils.GetCurrentSemester();
            return await Get(semester);            
        }

        public async Task<ResultModel> Get(string semester)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var examList = await _examTimeRepository.GetAll(semester);

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
