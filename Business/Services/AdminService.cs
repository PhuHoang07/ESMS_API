using Business.Interfaces;
using ESMS_Data.Repositories;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESMS_Data.Interfaces;
using ESMS_Data.Entities.RequestModel;

namespace Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResultModel> GetUserList()
        {
            // Empty string return all values
            return await GetUserList("");
        }

        public async Task<ResultModel> GetUserList(String userName)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var userList = await _userRepository.GetUserList(userName);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = userList;
            } catch(Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetUserDetails(String userName)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var user = await _userRepository.GetUserDetails(userName);

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

        public async Task<ResultModel> SetRole(UserReqModel req)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var user = await _userRepository.GetUser(req.UserName);

                user.RoleId = req.RoleId;
                await _userRepository.Update(user);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = 200;
                resultModel.Message = "Update thành công!";
            }catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }
    }
}
