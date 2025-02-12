using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int CourseId { get; set; }
        public int TermId { get; set; }

        [ForeignKey("AppUser")]
        public string InstructorId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Term Term { get; set; }
        public virtual AppUser Instructor { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
