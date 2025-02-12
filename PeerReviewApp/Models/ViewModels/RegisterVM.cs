using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PeerReviewApp.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Please enter a username")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please enter your first name")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Please enter your last name")]
        [StringLength (100)]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Please enter a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool IsInstructor { get; set; }

        public int InstructorCode { get;set; }

        [Required(ErrorMessage = "Please enter and Institution")]
        [StringLength(250)]
        public string InstitutionName { get; set; }
        public int InstitutionId { get; set; }
        public List<Institution> Institutions { get; set; }
    }
}
