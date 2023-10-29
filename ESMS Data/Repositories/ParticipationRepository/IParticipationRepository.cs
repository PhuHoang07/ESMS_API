using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ParticipationRepository
{
    public interface IParticipationRepository : IRepositoryBase<Participation>
    {
        public Task<object> GetStudents(int idt, string subject, string room);
    }
}
