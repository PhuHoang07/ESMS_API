using Business.Services.UserService;
using ESMS_Data.Entities;
using ESMS_Data.Models;
using ESMS_Data.Repositories.ExamRepository;
using ESMS_Data.Repositories.RegistrationRepository;
using ESMS_Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.LecturerService
{
    public class LecturerService : ILecturerService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IExamRepository _examRepository;
        private readonly Utils.Utils utils;

        public LecturerService(IUserRepository userRepository, IRegistrationRepository registrationRepository, IExamRepository examRepository)
        {
            _userRepository = userRepository;
            _registrationRepository = registrationRepository;
            _examRepository = examRepository;
            utils = new Utils.Utils();

        }

        public async Task<ResultModel> GetRegisteredExamTimes(string email)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var currentUser = await _userRepository.GetUser(email);
                var registeredExamTimes = await _registrationRepository.FormatRegisteredExamTimes(currentUser.UserName, utils.GetCurrentSemester());

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = registeredExamTimes;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetAvailableExamTimes(string email)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var currentUser = await _userRepository.GetUser(email);

                var registeredExamTimes = await _registrationRepository.GetRegisteredExamTimes(currentUser.UserName, utils.GetCurrentSemester());
                var availableExamTimes = await _registrationRepository.GetAvailableExamTimes(registeredExamTimes, utils.GetCurrentSemester());

                var removeList = new List<ExamTimeInfoModel>();
                var currentDate = DateTime.Now;
                foreach (var examTimes in availableExamTimes)
                {
                    var date = DateTime.ParseExact(examTimes.Date, "dd/MM/yyyy", null);
                    var difference = date - currentDate;
                    if ((examTimes.Registered >= examTimes.Required) || (difference.TotalDays <= 1))
                    {
                        removeList.Add(examTimes);
                    }
                }

                availableExamTimes = availableExamTimes.Except(removeList).ToList();

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = availableExamTimes;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;

        }

        public async Task<ResultModel> RegisterExamTime(string email, int idt)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var currentUser = await _userRepository.GetUser(email);

                var username = currentUser.UserName;
                var registration = new Registration
                {
                    Idt = idt,
                    UserName = username
                };
                await _registrationRepository.Add(registration);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Register successfully";
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
