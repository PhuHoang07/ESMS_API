using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ParticipationRepository
{
    public class ParticipationRepository : RepositoryBase<Participation>, IParticipationRepository
    {
        private ESMSContext _context;
        private DbSet<Participation> _participations;
        private DbSet<User> _users;

        public ParticipationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _participations = _context.Set<Participation>();
            _users = _context.Set<User>();
        }

        public async Task<object> GetStudents(int idt, string subject, string room)
        {
            var studentUserNames = await _participations.Where(p => p.Idt == idt
                                                     && p.SubjectId.Equals(subject)
                                                     && p.RoomNumber.Equals(room))
                                            .Select(p => p.UserName)
                                            .ToListAsync();

            var students = _users.Where(u => studentUserNames.Contains(u.UserName))
                                 .Select(u => new
                                 {
                                     u.UserName,
                                     u.Name,
                                     u.RollNumber
                                 });

            return await students.ToListAsync();
        }
    }
}
