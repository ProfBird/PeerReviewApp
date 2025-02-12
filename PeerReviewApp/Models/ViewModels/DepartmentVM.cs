using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class DepartmentVM
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set;}
        public int InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<AppUser> Instructors { get; set; }
    }
}
