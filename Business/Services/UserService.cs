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
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        public UserService(UserRepository repository)
        {
            _repository = repository;
        }
        public async Task<ResultModel> GetAll()
        {
            ResultModel resultModel = new ResultModel();
            var res = await _repository.GetAll();

            resultModel.IsSuccess = true;
            resultModel.StatusCode = (int)HttpStatusCode.OK;
            resultModel.Data = res;

            return resultModel;
        }
    }
}
