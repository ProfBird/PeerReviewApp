using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerReviewApp.Models
{
    public class InstitutionVM
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int InstructorCode { get; set; } //Code used for instructor to register at institution
        public ICollection<Department> Departments { get; set; } //Collection of Departments Belonging to Institution
        public ICollection<AppUser> Members { get; set; } //Collection of all Members Enrolled at Institution
        public ICollection<Term> Terms { get; set; } //Collection of all terms for Institutions
    }
}
