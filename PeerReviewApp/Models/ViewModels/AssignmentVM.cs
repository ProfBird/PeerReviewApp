using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace PeerReviewApp.Models
{
    public class AssignmentVM
    {
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string AssignmentInstructions { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public DateTime PeerReviewDueDate { get; set; }
        public int ClassId { get; set; }
        public AssignmentTemplate AssignmentTemplate { get; set; }
        public PeerReviewTemplate PeerReviewTemplate { get; set; }
        public AssignmentSubmission AssignmentSubmission { get; set; }
        public PeerReviewSubmission PeerReviewSubmission { get; set; }
        public AssignmentSubmission PartnerAssignmentSubmission { get; set; }
        public PeerReviewSubmission PartnerPeerReviewSubmission { get; set; }
        public AppUser User { get; set; }
        public AppUser PeerReviewPartner { get; set; }
        public IFormFile AssignmentTemplateFormFile { get; set; }
        public IFormFile PeerReviewTemplateFormFile { get; set; }
        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
    }
}