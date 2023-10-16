using Business.Interfaces;
using ESMS_Data.Repositories;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESMS_Data.Interfaces;

namespace Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _repository;
        public AdminService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<ResultModel> Get()
        {
            // Similar to find by username, empty string return all values
            return await FindByUserName("");
        }

        public async Task<ResultModel> FindByUserName(String userName)
        {
            ResultModel resultModel = new ResultModel();
            var userList = await _repository.Get(userName);

            resultModel.IsSuccess = true;
            resultModel.StatusCode = (int)HttpStatusCode.OK;
            resultModel.Data = userList;

            return resultModel;
        }

        public async Task<ResultModel> GetUserDetails(String userName)
        {
            ResultModel resultModel = new ResultModel();
            var user = await _repository.GetUserDetails(userName);

            resultModel.IsSuccess = true;
            resultModel.StatusCode = (int)HttpStatusCode.OK;
            resultModel.Data = user;

            return resultModel;
        }
    }
}
