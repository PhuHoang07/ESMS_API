using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class User
    {
        public User()
        {
            Participations = new HashSet<Participation>();
            Registrations = new HashSet<Registration>();
        }

        public string UserName { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Idcard { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public int RoleId { get; set; }
        public int? CurrentSemester { get; set; }
        public string RollNumber { get; set; }
        public bool? IsActive { get; set; }

        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
    }
}
