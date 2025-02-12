using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace PeerReviewApp.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; } 
        public string AssignmentInstructions { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public DateTime PeerReviewDueDate { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; }
        public virtual ICollection<PeerReviewSubmission> PeerReviewSubmissions { get; set; }
        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public virtual ICollection<AssignmentTemplate> AssignmentTemplates { get; set; }
        public virtual ICollection<PeerReviewTemplate> PeerReviewTemplates { get; set; }

    }
}
