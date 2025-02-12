using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        public string CourseName { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
