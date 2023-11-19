using ESMS_Data.Repositories.ExamRepository;
using ESMS_Data.Repositories.RegistrationRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.StaffService
{
    public class StaffService : IStaffService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IExamRepository _examRepository;
        public StaffService(IRegistrationRepository registrationRepository, IExamRepository examRepository)
        {
            _registrationRepository = registrationRepository;
            _examRepository = examRepository;
        }

        public async Task<ResultModel> GetAllowanceStatistic()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var usernameList = await _registrationRepository.GetSupervisorList();
                var semesterList = await _examRepository.GetSemester();
                var allowanceStatistic = await _registrationRepository.GetAllowanceStatistic(usernameList, semesterList);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = allowanceStatistic.ToList();
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
