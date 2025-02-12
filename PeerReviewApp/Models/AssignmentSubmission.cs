using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int SubmissionId { get; set; }
        public string SubmissionName { get; set; }
        public string ScrambledName { get; set; }
        public int Version { get; set; }

        public int AssignmentId { get; set; }
        [ForeignKey("AppUser")]
        public string StudentId { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual AppUser Student { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}
