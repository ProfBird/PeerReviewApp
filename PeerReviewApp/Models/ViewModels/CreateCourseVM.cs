using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PeerReviewApp.Models
{
    public class CreateCourseVM
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please enter a course name")]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Please enter a Institution")]
        public Institution Institution { get; set; }
        public List<Institution> Institutions { get; set; } //List of institutions 

        [Required(ErrorMessage = "Please enter a department")]
        public Department Department { get; set; }
        public List<Department> Departments { get; set; }
    }
}
