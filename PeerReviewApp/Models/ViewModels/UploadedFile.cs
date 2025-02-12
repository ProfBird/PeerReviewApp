using Microsoft.AspNetCore.Http;

namespace PeerReviewApp.Models
{
    public class UploadedFile
    {
        public IFormFile FormFile { get; set; }
      
        public string FileName { get; set; }
    }
}
