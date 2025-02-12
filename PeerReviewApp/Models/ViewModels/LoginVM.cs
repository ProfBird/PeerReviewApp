using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeerReviewApp.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(255, MinimumLength = 1)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter a password.")]
        [StringLength(255, MinimumLength = 1)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
