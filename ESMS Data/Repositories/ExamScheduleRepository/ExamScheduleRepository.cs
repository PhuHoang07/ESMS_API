using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace ESMS_Data.Repositories.ExamScheduleRepository
{
    public class ExamScheduleRepository : RepositoryBase<ExamSchedule>, IExamScheduleRepository
    {
        private ESMSContext _context;
        private DbSet<ExamSchedule> _examSchedules;
        private DbSet<ExamTime> _examTimes;
        private DbSet<Room> _rooms;
        private DbSet<Subject> _subjects;

        public ExamScheduleRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _rooms = _context.Set<Room>();
            _examSchedules = _context.Set<ExamSchedule>();
            _examTimes = _context.Set<ExamTime>();
            _subjects = _context.Set<Subject>();    
        }

        public IQueryable<Room> GetRoom()
        {
            return _rooms;
        }

        public IQueryable<Room> GetAvailableRoom(IQueryable<Room> qr, int Idt)
        {
            //Find exception room in one idt
            var filteredExamTimesByIdt = _examTimes
                                    .Include(et => et.ExamSchedules)
                                    .Where(et => et.Idt == Idt);

            var roomExceptByIdt = filteredExamTimesByIdt
                .SelectMany(et => et.ExamSchedules.Select(es => es.RoomNumber));
            
            var exceptRoomsByIdt = qr.Where(room => roomExceptByIdt.Contains(room.Number));

            //Find exception room in one day
            var date =  _examTimes.Where(et => et.Idt == Idt)
                                .Select(et => et.Date).FirstOrDefault();

            var start = _examTimes.Where(et => et.Date == date)
                                .Select(et => et.Start).FirstOrDefault();

            var filteredExamTimesByDate = _examTimes
                                    .Include(et => et.ExamSchedules)
                                    .Where(et => et.Date == date
                                        && et.Idt != Idt
                                        && et.Start <= start
                                        && et.End > start);

            var roomExceptByDay = filteredExamTimesByDate
                .SelectMany(et => et.ExamSchedules.Select(es => es.RoomNumber));

            var exceptRoomsByDay = qr.Where(room => roomExceptByDay.Contains(room.Number));

            //Get available room
            return _rooms.Except(exceptRoomsByDay.Concat(exceptRoomsByIdt));
        }

        public List<String> GetSubjectId()
        {
            return _subjects.Select(s => s.Id).ToList();
        }
    }
}
