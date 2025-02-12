using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PeerReviewApp.Models
{
    public class CreateClassVM
    {        
        public Course Course { get; set; }
        public Term Term { get; set; }       
        public string ClassName { get; set; }
        public AppUser Instructor { get; set; }
        public List<Term> Terms { get; set; }
        public List<Course> Courses { get; set; }
        public Institution Institution { get; set; }
        public Department Department { get; set; }
    }
}
