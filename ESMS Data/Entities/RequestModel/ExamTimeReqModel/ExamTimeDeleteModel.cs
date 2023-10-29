using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel.ExamTimeReqModel
{
    public class ExamTimeDeleteModel
    {
        [Required]
        public int Idt { get; set; }
    }
}
