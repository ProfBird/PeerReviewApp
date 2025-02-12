using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerReviewApp.Models
{
    public class PeerReviewPartners
    {
        [Key]
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Student1Id { get; set; }
        public string Student2Id { get; set; }
        public Class Class { get; set; }
        public AppUser Student1 { get; set; }
        public AppUser Student2 { get; set; }
    }
}