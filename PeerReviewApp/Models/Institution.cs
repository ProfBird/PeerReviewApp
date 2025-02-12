using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeerReviewApp.Models
{
    public class Institution
    {
        [Key]
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        [Required]
        public int InstructorCode { get; set; } //Code used for instructor to register at institution
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<AppUser> Members { get; set; }
        public virtual ICollection<Term> Terms { get; set; }
    }
}
