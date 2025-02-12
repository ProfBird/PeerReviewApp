using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
       
        [ForeignKey("AppUser")]
        public string StudentId { get; set; }
        public int ClassId { get; set; }
        public virtual AppUser Student { get; set; }
        public virtual Class Class { get; set; }
    }
}
