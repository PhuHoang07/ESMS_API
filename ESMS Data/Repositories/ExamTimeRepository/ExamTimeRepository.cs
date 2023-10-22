﻿using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ExamTimeRepository
{
    public class ExamTimeRepository : RepositoryBase<ExamTime>, IExamTimeRepository
    {
        private ESMSContext _context;
        private DbSet<ExamTime> _examTimes;
        private DbSet<ExamSchedule> _examSchedules;
        public ExamTimeRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _examTimes = _context.Set<ExamTime>();
            _examSchedules = _context.Set<ExamSchedule>();
        }

        public new async Task<List<object>> GetAll(string semester)
        {
            var list = await _examTimes
                                .Where(et => et.Semester.Equals(semester))
                                .ToListAsync();

            var qr = list
                        .GroupBy(e => e.Semester)
                        .Select(group => new
                        {
                            Semester = group.Key,
                            Exams = group.Select(i => new
                            {
                                i.Idt,
                                Date = i.Date.ToString("dd/MM/yyyy"),
                                Start = i.Start.ToString(@"hh\:mm"),
                                End = i.End.ToString(@"hh\:mm"),
                                PublishDate = i.PublishDate?.ToString("dd/MM/yyyy"),
                                Slot = i.SlotId,
                                ExamSchedules = _examSchedules.Where(es => es.Idt == i.Idt)
                                                              .Select(es => new
                                                              {
                                                                  Subject = es.SubjectId,
                                                                  Room = es.RoomNumber,
                                                                  es.Form,
                                                                  es.Type
                                                              })
                            })
                        });

            return qr.ToList<object>();
        }
    }
}