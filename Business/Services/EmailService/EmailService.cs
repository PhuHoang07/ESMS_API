using ESMS_Data.Entities.EmailModel;
using ESMS_Data.Models;
using ESMS_Data.Repositories.ExamRepository;
using ESMS_Data.Repositories.ExamScheduleRepository;
using ESMS_Data.Repositories.RegistrationRepository;
using ESMS_Data.Repositories.UserRepository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IExamRepository _examRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRegistrationRepository _registrationRepository;
        public EmailService(IOptions<EmailSettings> options, IExamRepository examRepository, IUserRepository userRepository, IRegistrationRepository registrationRepository)
        {
            _emailSettings = options.Value;
            _examRepository = examRepository;
            _userRepository = userRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<ExamTime> GetExamTimeToInform(int idt)
        {
            return await _examRepository.GetExamTime(idt);

        }
        public async Task SendEmailToProctorWhenDeleteSchedule(MailRequest mailRequest, int idt, string subjectId, string room)
        {
            var schedule = await _examRepository.GetExamSchedule(idt, subjectId, room);

            var proctorMail = await _userRepository.GetUserMail(schedule.Proctor);

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(proctorMail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            var password = SecretService.SecretService.EmailPassword;

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailToProctorWhenDeleteAndUpdateTime(MailRequest mailRequest, int idt)
        {
            var usernames = await _registrationRepository.GetProctorUsername(idt);

            var proctors = await _userRepository.GetUserList(usernames);

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);

            foreach (var proctor in proctors)
            {
                email.To.Add(MailboxAddress.Parse(proctor.Email.ToLower()));
            }
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            var password = SecretService.SecretService.EmailPassword;

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
