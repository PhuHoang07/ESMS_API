﻿using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ExamRepository
{
    public interface IExamRepository : IRepositoryBase<ExamTime>
    {
        public IQueryable<ExamTime> FilterSemester(IQueryable<ExamTime> qr, string semester);
        public IQueryable<ExamTime> FilterSubject(IQueryable<ExamTime> qr, List<string> subject);
        public IQueryable<ExamTime> FilterDate(IQueryable<ExamTime> qr, DateTime from, DateTime to);
        public IQueryable<ExamTime> FilterTime(IQueryable<ExamTime> qr, TimeSpan start, TimeSpan end);
        public Task<Dictionary<string, List<object>>> GroupBySemester(IQueryable<ExamTime> qr);
        public int GetRequireSupervisorAmount(int idt);
        public Task<List<string>> GetSemester();
        public Task<List<string>> GetSubject();
        public Task<int?> GetSlot(TimeSpan start);
        public Task<ExamTime> GetExamTime(int idt);
        public Task<List<string>> GetAvailableRoom(int idt);
        public Task<ExamSchedule> GetExamSchedule(int idt, string subjectID, string roomNumber);
        public Task<List<ExamSchedule>> GetExamScheduleListWithIdt(int idt);
        public Task<List<ExamSchedule>> GetExamScheduleWithDistinctRoom(int idt);
        public Task<List<string>> GetAssignedProctorList(int idt);
        public Task<List<ExamSchedule>> GetExamScheduleHasNoProctor(int idt);
        public Task<List<ExamSchedule>> GetExamScheduleHasProctor(int idt);
        public Task<List<ExamSchedule>> GetExistedExamSchedules(string username);
        public Task<DateTime> GetDate(ExamSchedule examSchedule);
        public Task<TimeSpan> GetStart(ExamSchedule examSchedule);
        public Task<TimeSpan> GetEnd(ExamSchedule examSchedule);
        public Task<List<ExamSchedule>> GetExamScheduleWithSameDateAndRoom(ExamSchedule examSchedule);
        public Task<List<ExamTime>> GetExamTimeList(List<int> idt);
        public Task<List<ExamTime>> GetExamTimeInOneDay(DateTime date);

    }
}