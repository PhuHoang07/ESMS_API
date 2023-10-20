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
        private readonly IUserRepository _repository;
        public AdminService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<ResultModel> GetUserList()
        {
            // Similar to find by username, empty string return all values
            return await FindByUserName("");
        }

        public async Task<ResultModel> FindByUserName(String userName)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var userList = await _repository.GetUserList(userName);

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
                var user = await _repository.GetUserDetails(userName);

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

        public async Task<ResultModel> SetRole(RoleReqModel req)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var user = await _repository.GetUser(req.UserName);

                user.RoleId = req.RoleId;
                await _repository.Update(user);

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
