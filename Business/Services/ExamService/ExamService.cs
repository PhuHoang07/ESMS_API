using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Models;
using ESMS_Data.Repositories.ExamRepository;
using ESMS_Data.Repositories.ExamScheduleRepository;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Business.Services.ExamService
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamScheduleRepository _examScheduleRepository;
        private readonly Utils.Utils utils;
        public ExamService(IExamRepository examRepository, IExamScheduleRepository examScheduleRepository)
        {
            _examRepository = examRepository;
            _examScheduleRepository = examScheduleRepository;
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

        public async Task<ResultModel> GetSemester()
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

        public async Task<ResultModel> GetSubject()
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
                    Semester = currentSemester
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

        public async Task<ResultModel> GetAvailableRoom(int idt)
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

                var currentExamSchedule = await _examRepository.GetUpdateExamSchedule(req.Idt, req.SubjectID, req.RoomNumber);

                if (currentExamSchedule == null)
                {
                    throw new Exception("There is no exam schedule with given information");
                }

                await _examScheduleRepository.Delete(currentExamSchedule);

                var updIdt = req.UpdIdt.HasValue ? req.UpdIdt.Value : currentExamSchedule.Idt;
                var updSubjectId = String.IsNullOrEmpty(req.UpdSubjectID) ? currentExamSchedule.SubjectId : req.UpdSubjectID;
                var updRoomNumber = String.IsNullOrEmpty(req.UpdRoomNumber) ? currentExamSchedule.RoomNumber : req.UpdRoomNumber;
                var updForm = String.IsNullOrEmpty(req.UpdForm) ? currentExamSchedule.Form : req.UpdForm;
                var updType = String.IsNullOrEmpty(req.UpdType) ? currentExamSchedule.Type : req.UpdType;
                var updProctor = String.IsNullOrEmpty(req.UpdProctor) ? currentExamSchedule.Proctor : req.UpdProctor;

                var roomList = await _examRepository.GetAvailableRoom(updIdt);

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
                    Idt = updIdt,
                    SubjectId = updSubjectId,
                    RoomNumber = updRoomNumber,
                    Form = updForm,
                    Type = updType,
                    Proctor = updProctor,
                };

                await _examScheduleRepository.Add(updExamSchedule);

                resultModel.IsSuccess = true;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Message = "Update successfully";
                resultModel.Data = updExamSchedule;
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
                var delExamSchedule = await _examRepository.GetUpdateExamSchedule(req.Idt, req.SubjectID, req.RoomNumber);

                if (delExamSchedule == null)
                {
                    throw new Exception("There is no exam schedule with given information");
                }

                await _examScheduleRepository.Delete(delExamSchedule);

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
    }
}
