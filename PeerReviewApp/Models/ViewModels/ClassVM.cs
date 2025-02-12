using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class ClassVM
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public Assignment SelectedAssignment { get; set; }
    }
}