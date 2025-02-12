using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PeerReviewApp.Models
{
    public class PeerReviewPartnersVM
    {
        public int Id { get; set; }
        public Class Class { get; set; }
        public AppUser Student1 { get; set; }
        public AppUser Student2 { get; set; }
        public ICollection<AppUser> UnassignedStudents { get; set; }
        public ICollection<AppUser> AssignedStudents { get; set; }
    }
}