using Business.Services.UserService;
using ESMS_Data.Models;
using ESMS_Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.LecturerService
{
    public class LecturerService : ILecturerService
    {
        private readonly IUserRepository _userRepository;

        public LecturerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultModel> GetExamTimeForLecturer(string email)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var user = _userRepository.GetUser(email);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                //resultModel.Data = registrations;
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
