using Business.Interfaces;
using ESMS_Data.Repositories;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class AdminService : IUserService
    {
        private readonly UserRepository _repository;
        public AdminService(UserRepository repository)
        {
            _repository = repository;
        }
        public async Task<ResultModel> GetAll()
        {
            ResultModel resultModel = new ResultModel();
            var userList = await _repository.GetBasic();

            resultModel.IsSuccess = true;
            resultModel.StatusCode = (int)HttpStatusCode.OK;
            resultModel.Data = userList;

            return resultModel;
        }
    }
}
