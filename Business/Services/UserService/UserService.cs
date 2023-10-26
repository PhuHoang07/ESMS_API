using ESMS_Data.Entities;
using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultModel> GetUserList()
        {
            // Empty string return all values
            return await GetUserList("");
        }

        public async Task<ResultModel> GetUserList(string userName)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var userList = await _userRepository.GetUserList(userName ?? "");

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = userList;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetUserDetails(string userNameOrEmail)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var user = await _userRepository.GetUserDetails(userNameOrEmail);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = user;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> UpdateSettings(UserSettingsReqModel req)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var user = await _userRepository.GetUser(req.UserName);

                if (req.RoleId != null)
                {
                    user.RoleId = req.RoleId.Value;
                }

                if (req.IsActive != null)
                {
                    user.IsActive = req.IsActive;
                }

                await _userRepository.Update(user);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = 200;
                resultModel.Message = "Update successfully!";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> UpdateUser(UserModel currentUser, UserProfileReqModel userProfileReqModel)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var user = await _userRepository.GetUser(currentUser.Email);

                user.Name = userProfileReqModel.Name;
                user.DateOfBirth = userProfileReqModel.DateOfBirth;
                user.Gender = userProfileReqModel.Gender;
                user.Idcard = userProfileReqModel.Idcard;
                user.Address = userProfileReqModel.Address;
                user.PhoneNumber = userProfileReqModel.PhoneNumber;

                await _userRepository.Update(user);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully!";
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
