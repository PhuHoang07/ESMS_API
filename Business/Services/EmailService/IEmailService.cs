using ESMS_Data.Entities.EmailModel;
using ESMS_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.EmailService
{
    public interface IEmailService
    {
        public Task<ExamTime> GetExamTimeToInform(int idt);
        public Task SendEmailToProctorWhenDeleteSchedule(MailRequest mailRequest, int idt, string subjectId, string room);
        public Task SendEmailToProctorWhenDeleteAndUpdateTime(MailRequest mailRequest, int idt);

    }
}
