using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class UserCoursesVM
    {   
        public List<Course> Courses { get; set; }
        public ICollection<Class> Classes { get; set; }
        public Class SelectedCourse { get; set; }
        public AppUser _User { get; set; }

        public UserCoursesVM()
        {
            Classes = new List<Class>();
        }
    }
}
