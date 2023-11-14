using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Entities.RequestModel.ExamScheduleReqModel;
using ESMS_Data.Entities.RequestModel.ExamTimeReqModel;
using ESMS_Data.Models;
using ESMS_Data.Repositories.ExamRepository;
using ESMS_Data.Repositories.ExamScheduleRepository;
using ESMS_Data.Repositories.RegistrationRepository;
using ESMS_Data.Repositories.ParticipationRepository;
using System.Net;
using ESMS_Data.Entities.RequestModel.ParticipationReqModel;
using ESMS_Data.Entities.RequestModel.RegistrationReqModel;
using System.Data;
using ClosedXML.Excel;
using Azure.Identity;

namespace Business.Services.ExamService
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamScheduleRepository _examScheduleRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IParticipationRepository _participationRepository;
        private readonly Utils.Utils utils;
        public ExamService(IExamRepository examRepository, IExamScheduleRepository examScheduleRepository, IRegistrationRepository registrationRepository, IParticipationRepository participationRepository)
        {
            _examRepository = examRepository;
            _examScheduleRepository = examScheduleRepository;
            _registrationRepository = registrationRepository;
            _participationRepository = participationRepository;
            utils = new Utils.Utils();
        }

        public async Task<ResultModel> GetCurrent()
        {
            string semester = utils.GetCurrentSemester();
            return await Get(new ExamFilterReqModel { Semester = semester });
        }

        public async Task<ResultModel> Get(ExamFilterReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var qr = _examRepository.GetAll();

                if (!String.IsNullOrEmpty(req.Semester))
                {
                    qr = _examRepository.FilterSemester(qr, req.Semester);
                }

                if (req.Subjects != null)
                {
                    qr = _examRepository.FilterSubject(qr, req.Subjects);
                }

                qr = _examRepository.FilterDate(qr, req.From, req.To);

                qr = _examRepository.FilterTime(qr, req.Start, req.End);

                var examList = await _examRepository.GroupBySemester(qr);

                if (!String.IsNullOrEmpty(req.Semester) && !examList.ContainsKey(req.Semester.ToUpper()))
                {
                    examList.Add(req.Semester.ToUpper(), new List<object>());
                }

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = examList;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetSemesters()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var semesters = await _examRepository.GetSemester();

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = semesters;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetSubjects()
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var subjects = await _examRepository.GetSubject();

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = subjects;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        private void ValidateTime(TimeSpan start, TimeSpan end)
        {
            if (start >= end)
            {
                throw new Exception("Invalid time: Start >= End");
            }
        }

        private void ValidateDate(DateTime date, DateTime publishDate)
        {
            if (date > publishDate)
            {
                throw new Exception("Invalid date: Date > Publish date");
            }

            if (date <= DateTime.Now.Date)
            {
                throw new Exception("Invalid date: Date <= current date");
            }
        }

        public async Task<ResultModel> AddTime(ExamTimeAddReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                ValidateTime(req.Start, req.End);
                ValidateDate(req.Date, req.PublishDate);

                var slot = await _examRepository.GetSlot(req.Start);

                if (slot == 0)
                {
                    throw new Exception("Invalid time: Start does not belong to any Slot");
                }

                var currentSemester = utils.GetCurrentSemester();

                if (!currentSemester.Equals(utils.GetSemester(req.Date)))
                {
                    throw new Exception("Invalid date: Date does not belong to current semester");
                }

                var examTime = new ExamTime
                {
                    Date = req.Date,
                    Start = req.Start,
                    End = req.End,
                    PublishDate = req.PublishDate,
                    SlotId = slot.Value,
                    Semester = currentSemester,
                    IsPublic = false
                };

                await _examRepository.Add(examTime);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> UpdateTime(ExamTimeUpdateReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var registrations = await _registrationRepository.GetRegistration(req.Idt);

                if (registrations.Count > 0)
                {
                    await _registrationRepository.DeleteRange(registrations);
                }

                ValidateTime(req.Start, req.End);
                ValidateDate(req.Date, req.PublishDate);

                var slot = await _examRepository.GetSlot(req.Start);

                if (slot == 0)
                {
                    throw new Exception("Invalid time: Start does not belong to any Slot");
                }

                var currentSemester = utils.GetCurrentSemester();

                if (!currentSemester.Equals(utils.GetSemester(req.Date)))
                {
                    throw new Exception("Invalid date: Date does not belong to current semester");
                }

                var currentExamTime = await _examRepository.GetExamTime(req.Idt);

                currentExamTime.Date = req.Date;
                currentExamTime.Start = req.Start;
                currentExamTime.End = req.End;
                currentExamTime.PublishDate = req.PublishDate;
                currentExamTime.SlotId = slot;

                await _examRepository.Update(currentExamTime);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> DeleteTime(int idt)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var currentExamTime = await _examRepository.GetExamTime(idt);

                var participations = await _participationRepository.GetParticipationListWithIdt(idt);
                var examSchedules = await _examRepository.GetExamScheduleListWithIdt(idt);
                var registrations = await _registrationRepository.GetRegistration(idt);

                if (participations.Count > 0)
                {
                    await _participationRepository.DeleteRange(participations);
                }

                if (examSchedules.Count > 0)
                {
                    await _examScheduleRepository.DeleteRange(examSchedules);
                }

                if (registrations.Count > 0)
                {
                    await _registrationRepository.DeleteRange(registrations);
                }

                await _examRepository.Delete(currentExamTime);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Delete successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetAvailableRooms(int idt)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var room = await _examRepository.GetAvailableRoom(idt);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = room;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> AddExamSchedule(ExamScheduleAddReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var roomList = await _examRepository.GetAvailableRoom(req.Idt);

                if (String.IsNullOrEmpty(req.RoomNumber))
                {
                    req.RoomNumber = roomList.FirstOrDefault();
                }
                else if (!roomList.Contains(req.RoomNumber))
                {
                    throw new Exception("The enter room is not avalable");

                }

                var subjects = await _examRepository.GetSubject();

                if (!subjects.Contains(req.SubjectID))
                {
                    throw new Exception("Wrong subject id");
                }

                var examSchedule = new ExamSchedule
                {
                    Idt = req.Idt,
                    SubjectId = req.SubjectID,
                    RoomNumber = req.RoomNumber,
                    Form = req.Form,
                    Type = req.Type,
                };

                await _examScheduleRepository.Add(examSchedule);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateExamSchedule(ExamScheduleUpdateReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {

                var currentExamSchedule = await _examRepository.GetExamSchedule(req.Idt, req.SubjectID, req.RoomNumber);

                if (currentExamSchedule == null)
                {
                    throw new Exception("There is no exam schedule with given information");
                }

                var studentList = await _participationRepository.GetStudentListInParticipation(req.Idt, req.SubjectID, req.RoomNumber);
                var participationList = await _participationRepository.GetParticipationList(req.Idt, req.SubjectID, req.RoomNumber);
                if (participationList.Count > 0)
                {
                    await _participationRepository.DeleteRange(participationList);
                }
                await _examScheduleRepository.Delete(currentExamSchedule);

                var updSubjectId = String.IsNullOrEmpty(req.UpdSubjectID) ? currentExamSchedule.SubjectId : req.UpdSubjectID;
                var updRoomNumber = String.IsNullOrEmpty(req.UpdRoomNumber) ? currentExamSchedule.RoomNumber : req.UpdRoomNumber;
                var updForm = String.IsNullOrEmpty(req.UpdForm) ? currentExamSchedule.Form : req.UpdForm;
                var updType = String.IsNullOrEmpty(req.UpdType) ? currentExamSchedule.Type : req.UpdType;

                var roomList = await _examRepository.GetAvailableRoom(req.Idt);

                if (!roomList.Contains(updRoomNumber))
                {
                    throw new Exception("The enter room is not avalable");
                }

                var subjects = await _examRepository.GetSubject();
                if (!subjects.Contains(req.SubjectID))
                {
                    throw new Exception("Wrong subject id");
                }

                var updExamSchedule = new ExamSchedule
                {
                    Idt = req.Idt,
                    SubjectId = updSubjectId,
                    RoomNumber = updRoomNumber,
                    Form = updForm,
                    Type = updType,
                    Proctor = currentExamSchedule.Proctor,
                };

                await _examScheduleRepository.Add(updExamSchedule);

                if (participationList.Count > 0)
                {
                    var newParticipations = new List<Participation>();
                    foreach (var student in studentList)
                    {
                        newParticipations.Add(new Participation
                        {
                            UserName = student,
                            Idt = req.Idt,
                            RoomNumber = updRoomNumber,
                            SubjectId = updSubjectId
                        });
                    }

                    await _participationRepository.AddRange(newParticipations);
                }

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> DeleteExamSchedule(ExamScheduleDeleteReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var delExamSchedule = await _examRepository.GetExamSchedule(req.Idt, req.SubjectID, req.RoomNumber);
                var participations = await _participationRepository.GetParticipationList(req.Idt, req.SubjectID, req.RoomNumber);


                if (participations.Count > 0)
                {
                    await _participationRepository.DeleteRange(participations);
                }

                if (delExamSchedule == null)
                {
                    throw new Exception("There is no exam schedule with given information");
                }

                await _examScheduleRepository.Delete(delExamSchedule);
                if (delExamSchedule.Proctor != null)
                {
                    var registration = await _registrationRepository.GetRegistrationOfProctor(req.Idt, delExamSchedule.Proctor);
                    await _registrationRepository.Delete(registration);
                }

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Delete successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> AddProctorToExamTime(RegistrationAddRemoveReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var requireSupervisor = _examRepository.GetRequireSupervisorAmount(req.Idt);
                var registeredAmount = _registrationRepository.GetRegisteredAmount(req.Idt);

                if (registeredAmount + req.ProctorList.Count() > requireSupervisor)
                {
                    throw new Exception("Proctor amount exceed the limit");
                }

                var registrations = new List<Registration>();
                foreach (var proctor in req.ProctorList)
                {
                    registrations.Add(new Registration
                    {
                        UserName = proctor,
                        Idt = req.Idt
                    });
                }

                await _registrationRepository.AddRange(registrations);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
                resultModel.Data = registrations;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> RemoveProctorFromExamTime(RegistrationAddRemoveReqModel req)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var removingRegistrations = new List<Registration>();
                foreach (var proctor in req.ProctorList)
                {
                    removingRegistrations.Add(new Registration
                    {
                        UserName = proctor,
                        Idt = req.Idt
                    });
                }

                var schedulesOfProctorsRemoving = _examScheduleRepository.GetAll()
                                                                 .Where(es => es.Idt == req.Idt
                                                                           && req.ProctorList.Contains(es.Proctor))
                                                                 .ToList<ExamSchedule>();

                foreach (var schedule in schedulesOfProctorsRemoving)
                {
                    schedule.Proctor = null;
                }

                // set proctor of schedule to null first, else error conflicting foreign key
                await _examScheduleRepository.UpdateRange(schedulesOfProctorsRemoving);
                await _registrationRepository.DeleteRange(removingRegistrations);


                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Delete successfully";
                resultModel.Data = removingRegistrations;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public async Task<ResultModel> GetStudents(int idt, string subject, string room)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var studentList = await _participationRepository.GetStudentListInExam(idt, subject, room);
                var total = await _participationRepository.GetTotalStudentInRoom(idt, room);
                var capacity = await _participationRepository.GetRoomCapacity(room);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = new
                {
                    total,
                    capacity = capacity.Value,
                    studentList
                };
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> AddStudents(ParticipationAddRemoveReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var examSchedule = await _examRepository.GetExamSchedule(req.Idt, req.Subject, req.Room);

                var currentDate = await _examRepository.GetDate(examSchedule);
                var currentStart = await _examRepository.GetStart(examSchedule);
                var currentEnd = await _examRepository.GetEnd(examSchedule);
                var semester = utils.GetSemester(currentDate);

                var roomCapacity = await _participationRepository.GetRoomCapacity(req.Room);
                var currentAmount = await _participationRepository.GetTotalStudentInRoom(req.Idt, req.Room);

                if (currentAmount + req.Students.Count > roomCapacity)
                {
                    throw new Exception("Student list exceed the room's capacity");
                }

                foreach (var student in req.Students)
                {
                    var examCheckList = await _examRepository.GetExistedExamSchedules(student);
                    foreach (var examCheck in examCheckList)
                    {
                        var checkDate = await _examRepository.GetDate(examCheck);
                        var semesterCheck = utils.GetSemester(checkDate);

                        if (semester.Equals(semesterCheck)
                            && examSchedule.SubjectId.Equals(examCheck.SubjectId)
                            && examSchedule.Form.Equals(examCheck.Form))
                        {
                            throw new Exception($"Failed! In semester {semester}, student {student} has already participation in subject {examSchedule.SubjectId} - {examSchedule.Form}");
                        }

                        var checkStart = await _examRepository.GetStart(examCheck);
                        var checkEnd = await _examRepository.GetEnd(examCheck);

                        if ((currentDate == checkDate
                            && currentStart >= checkStart
                            && currentStart <= checkEnd)
                            ||
                            (currentDate == checkDate
                            && currentEnd >= checkStart
                            && currentEnd <= checkEnd))
                        {
                            throw new Exception($"Failed! In date {checkDate}, student {student} has already participation in exam schedule ( {checkStart} - {checkEnd} )");
                        }
                    }
                }

                var participations = new List<Participation>();
                foreach (var u in req.Students)
                {
                    participations.Add(new Participation
                    {
                        UserName = u,
                        SubjectId = req.Subject,
                        RoomNumber = req.Room,
                        Idt = req.Idt
                    });
                }

                await _participationRepository.AddRange(participations);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Add successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> RemoveStudents(ParticipationAddRemoveReqModel req)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var participations = await _participationRepository.GetParticipationsOnList(req.Idt, req.Subject, req.Room, req.Students);

                await _participationRepository.DeleteRange(participations);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Remove successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> ViewProctorList(int idt)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var proctorList = await _registrationRepository.GetProctors(idt);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = proctorList;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateProctorInExamSchedule(ExamScheduleUpdateProctorReqModel req)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var examSchedule = await _examRepository.GetExamSchedule(req.Idt, req.SubjectID, req.RoomNumber);
                var examScheduleList = await _examRepository.GetExamScheduleWithSameDateAndRoom(examSchedule);

                foreach (var exam in examScheduleList)
                {
                    exam.Proctor = req.UpdProctor;
                }

                await _examScheduleRepository.UpdateRange(examScheduleList);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> UpdateProctorsToExamSchedule(int idt)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var assignedProctorList = await _examRepository.GetAssignedProctorList(idt);
                var proctorList = await _registrationRepository.GetAvailableProctors(idt, assignedProctorList);

                var examSchedules = await _examRepository.GetExamScheduleWithDistinctRoom(idt);

                if (examSchedules.Count == 0)
                {
                    throw new Exception("There is no idt / There is no room having none proctor");
                }

                int minCount = Math.Min(proctorList.Count, examSchedules.Count);

                for (int i = 0; i < minCount; i++)
                {
                    examSchedules[i].Proctor = proctorList[i];
                }

                await _examScheduleRepository.UpdateRange(examSchedules);

                var updatedExamSchedules = await _examRepository.GetExamScheduleHasProctor(idt);
                var remainExamSchedules = await _examRepository.GetExamScheduleHasNoProctor(idt);

                foreach (var exam in remainExamSchedules)
                {
                    var similarExam = updatedExamSchedules.FirstOrDefault(e =>
                        e.RoomNumber == exam.RoomNumber &&
                        e.SubjectId != exam.SubjectId
                    );

                    if (similarExam != null)
                    {
                        exam.Proctor = similarExam.Proctor;
                    }
                }

                await _examScheduleRepository.UpdateRange(remainExamSchedules);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully";
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetUnassignedProctorOfExamTime(int idt)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var assignedProctorList = await _examRepository.GetAssignedProctorList(idt);
                var proctorList = await _registrationRepository.GetAvailable(idt, assignedProctorList);

                var getInfo = proctorList.Select(proctor => new { proctor.UserName, proctor.Name }).ToList();


                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = getInfo;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> PublicExamTime()
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                var examTimeList = _examRepository.GetAll().ToList();

                foreach (var examTime in examTimeList)
                {
                    examTime.IsPublic = true;
                }

                await _examRepository.UpdateRange(examTimeList);
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }

        public async Task<List<DataTable>> ExportToExcel(int idt)
        {
            try
            {

                var examSchedules = await _examRepository.GetExamScheduleListWithIdt(idt);

                if (examSchedules.Count <= 0)
                {
                    throw new Exception("There is no exam schedule to export");
                }

                return await _participationRepository.GetExamScheduleInfoToExport(idt);
            }
            catch (Exception ex)
            {
                throw new Exception("There is no exam schedule to export.");
            }

        }

        public async Task<ExamTime> GetExamTimeInfo(int idt)
        {
            return await _examRepository.GetExamTime(idt);
        }

        public async Task<List<string>> GetProctorList(int idt)
        {
            return await _examRepository.GetAssignedProctorList(idt);
        }

    }
}
