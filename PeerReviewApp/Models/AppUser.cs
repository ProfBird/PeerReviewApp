using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; internal set; }
        public int InstitutionId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Institution Institution { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public virtual ICollection<PeerReviewSubmission> PeerReviewSubmissions { get; set; }

        [NotMapped]
        public IList<String> RoleNames { get; set; }
    }
}
