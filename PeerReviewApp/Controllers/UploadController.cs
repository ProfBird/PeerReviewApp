using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PeerReviewApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace PeerReviewApp.Controllers
{
    public class UploadController : Controller
    {
        private readonly long _fileSizeLimit = 2097162; // 2,097,162
        private readonly string[] _permittedExtensions = { ".txt", ".csv" };
        private readonly string _targetFilePath = "wwwroot/files";
        public string Result { get; private set; }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /************* File Upload Action Methods ***********/
        [HttpGet]
        public IActionResult Upload()
        {
            UploadedFile model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";
                return View();
            }

            var formFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(formFile, ModelState, _permittedExtensions, _fileSizeLimit);

            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";
                return View();
            }

            // For the file name of the uploaded file stored
            // server-side, use Path.GetRandomFileName to generate a safe
            // random file name. MODIFIED TO KEEP FILE EXTENSIONS!

            //gets the extesion from the uploaded file
            var fileEx = Path.GetExtension(formFile.FileName);
            //generates the random file name, also randomizes the file extension
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            //removes the randomized file extension from the file
            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
            //adds the original file extension back on
            trustedFileNameForFileStorage += fileEx;

            var filePath = Path.Combine(_targetFilePath, trustedFileNameForFileStorage);

            // **WARNING!**
            // In the following example, the file is saved without
            // scanning the file's contents. In most production
            // scenarios, an anti-virus/anti-malware scanner API
            // is used on the file before making the file available
            // for download or for use by other systems. 
            // For more information, see the topic that accompanies 
            // this sample.
            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);

                // To work directly with a FormFile, use the following
                // instead:
                //await FileUpload.FormFile.CopyToAsync(fileStream);
            }

            return RedirectToAction("Index");
        }
    }
}
