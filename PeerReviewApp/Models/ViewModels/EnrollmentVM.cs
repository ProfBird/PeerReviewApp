using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class EnrollmentVM
    {
        public AppUser Student { get; set; }
        public Class Class { get; set; }
        public AppUser Instructor { get; set; }
        public List<Class> Classes { get; set; }
        public List<AppUser> UnenrolledStudents { get; set; }
    }
}
