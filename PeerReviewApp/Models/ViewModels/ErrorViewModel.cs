using System.ComponentModel.DataAnnotations;
using System;

namespace PeerReviewApp.Models
{
    public class ErrorViewModel
    {
        [Key]
        [Required]
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
