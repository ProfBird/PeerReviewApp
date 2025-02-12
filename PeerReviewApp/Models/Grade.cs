using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal GradeDecimal { get; set; }
        public string InstructorComment { get; set; }

        [ForeignKey("AppUser")]
        public string StudentId { get; set; }
        public int? SubmissionId { get; set; }
        public int? PeerReviewId { get; set; }

        public virtual AppUser Student { get; set; }
        public virtual AssignmentSubmission AssignmentSubmission { get; set; }
        public virtual PeerReviewSubmission PeerReviewSubmission { get; set; }
    }
}
