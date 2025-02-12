using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PeerReviewApp.Data;
using PeerReviewApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PeerReviewApp.Controllers
{
    public class ZipFileController : Controller
    {
        private readonly string _targetFilePath = "wwwroot/files";
        private IUnitOfWork data { get; set; }
        private ApplicationDbContext context;
        private IHostingEnvironment _IHostingEnviroment;


        public ZipFileController(IUnitOfWork uow, ApplicationDbContext ctx, IHostingEnvironment iHostingEnviroment)
        {
            _IHostingEnviroment = iHostingEnviroment;
            data = uow;
            context = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        public FileResult GenerateAndDownloadZip(int id)
        {
            Assignment assignment = data.GetAssignment(id);

            var webRoot = _IHostingEnviroment.WebRootPath;
            var fileName = assignment.AssignmentName + ".zip";
            var tempOutput = webRoot + "/Files/" + fileName;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                zipOutputStream.SetLevel(9);

                byte[] buffer = new byte[4096];

                var assignmentSubmissionList = data.GetAssignmentSubmissions(id);
                var peerReviewSubmissionList = data.GetPeerReviewSubmissions(id);

                var FileList = new List<string>();
                var NormalizedFileName = new List<string>();

                foreach (AssignmentSubmission a in assignmentSubmissionList)
                {
                    FileList.Add(_targetFilePath + "/studentSubmittedAssignments/" +  a.ScrambledName);
                    NormalizedFileName.Add(a.SubmissionName);
                }

                foreach (PeerReviewSubmission p in peerReviewSubmissionList)
                {
                    FileList.Add(_targetFilePath + "/studentSubmittedPeerReviews/" + p.ScrambledName);
                    NormalizedFileName.Add(p.PeerReviewName);
                }


                for(int i = 0; i < FileList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(FileList[i]));

                    

                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipOutputStream.PutNextEntry(entry);

                    using(FileStream fileStream = System.IO.File.OpenRead(FileList[i]))
                    {
                        
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fileStream.Read(buffer, 0, buffer.Length);
                            zipOutputStream.Write(buffer, 0, sourceBytes);
                        }
                        while(sourceBytes > 0);
                    }
                }

                zipOutputStream.Finish();
                zipOutputStream.Flush();
                zipOutputStream.Close();
            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);

            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }

            if(finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing Found"));
            }
            
            return File(finalResult, "application/zip", fileName);
        }
    }
}
