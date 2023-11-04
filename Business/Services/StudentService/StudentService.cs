using ESMS_Data.Models;
using ESMS_Data.Repositories.ParticipationRepository;
using ESMS_Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly IParticipationRepository _participationRepository;
        private readonly IUserRepository _userRepository;
        private readonly Utils.Utils utils;

        public StudentService(IParticipationRepository participationRepository, IUserRepository userRepository)
        {
            _participationRepository = participationRepository;
            _userRepository = userRepository;
            utils = new Utils.Utils();
        }

        public async Task<ResultModel> GetAssignedExamSchedules(string email)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var currentUser = await _userRepository.GetUser(email);
                var examScheduleList = await _participationRepository.GetOwnExamSchedule(currentUser.UserName, utils.GetCurrentSemester());

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = examScheduleList;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetPreviewExamSchedule()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var examScheduleList = await _participationRepository.GetPreviewExamScheduleList(utils.GetCurrentSemester());

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = examScheduleList;
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
