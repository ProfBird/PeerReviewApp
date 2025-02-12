using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PeerReviewApp.Models
{
    public class UserViewModel
    {
        public AppUser User { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public Institution Institution { get; set; }
    }
}
