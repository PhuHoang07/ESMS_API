using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel.UserReqModel
{
    public class UserSettingsReqModel
    {
        [Required]
        public string UserName { set; get; }
        public int? RoleId { set; get; }
        public bool? IsActive { set; get; }
    }
}
