using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PeerReviewApp.Models;

namespace PeerReviewApp.Controllers
{
    public class DownloadController : Controller
    {
        private IHostingEnvironment env;

        public DownloadController(IHostingEnvironment _env)
        {
            env = _env;
        }

        public IActionResult Index()
        {
            //gets all the files in the file folder
            string[] filePaths = Directory.GetFiles(Path.Combine(this.env.WebRootPath, "files/"));

            //copies the file names to a list of File models
            List<UploadedFile> files = new List<UploadedFile>();
            foreach (string filePath in filePaths)
                files.Add(new UploadedFile { FileName = Path.GetFileName(filePath) });

            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            //builds the path
            string path = Path.Combine(this.env.WebRootPath, "files/") + fileName;

            //read the file data into the byte array
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //sends the file to download
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}