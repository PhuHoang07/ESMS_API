﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IExamService
    {
        public Task<ResultModel> GetCurrent();
        public Task<ResultModel> Get(String semester);
    }
}
