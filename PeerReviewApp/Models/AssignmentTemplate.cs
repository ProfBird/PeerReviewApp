using System.ComponentModel.DataAnnotations;

namespace PeerReviewApp.Models
{
    public class AssignmentTemplate
    {
        [Key]
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int Version { get; set; }
        public string ScrambledName { get; set; }
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
    }
}
