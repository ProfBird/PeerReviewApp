using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReviewApp.Data;
using PeerReviewApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace PeerReviewApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly long _fileSizeLimit = 2097162; // 2,097,162
        private readonly string[] _permittedExtensions = { ".txt", ".csv" };
        private readonly string _targetFilePath = "wwwroot/files";

        private IUnitOfWork data { get; set; }
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
        private ApplicationDbContext context;
        private IWebHostEnvironment env;

        public CourseController(IUnitOfWork uow, UserManager<AppUser> userMngr, RoleManager<IdentityRole> roleMngr)
        {
            data = uow;
            userManager = userMngr;
            roleManager = roleMngr;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.GetUserAsync(User);
            user.RoleNames = await userManager.GetRolesAsync(user);

            UserCoursesVM userCourses = new UserCoursesVM()
            {
                _User = user
            };

            if (user.RoleNames.Contains("Instructor"))
            {
                userCourses.Classes = data.GetClassesByInstructor(user.Id);
            }
            else
            {
                userCourses.Classes = data.GetClassesByStudent(user.Id);
            }

            return View(userCourses);
        }

        /********************Course View********************/
        [HttpGet]
        public IActionResult ClassPage(int id)
        {
            Class selectedCourse = data.GetClass(id);
            ClassVM classVM = new ClassVM()
            {
                ClassId = selectedCourse.ClassId,
                ClassName = selectedCourse.ClassName,
                Assignments = selectedCourse.Assignments
            };
            return View(classVM);
        }

        /********************Peer Review Partner Settings********************/
        [HttpGet]
        public async Task<IActionResult> PeerReviewSettingsAsync(int id)
        {
            PeerReviewPartnersVM prPartnersVM = new PeerReviewPartnersVM();
            prPartnersVM.Class = data.GetClass(id);

            List<SelectListItem> li = new List<SelectListItem>();

            List<PeerReviewPartners> prLi = new List<PeerReviewPartners>();

            prLi = data.GetPRPartnersByClass(id);
            prPartnersVM.UnassignedStudents = data.GetStudentsByClass(id);

            foreach (PeerReviewPartners p in prLi)
            {
                var student1 = await userManager.FindByIdAsync(p.Student1Id);
                var student2 = await userManager.FindByIdAsync(p.Student2Id);
                var stu1 = student1.FirstName + " " + student1.LastName;
                var stu2 = student2.FirstName + " " + student2.LastName;
                var prId = p.Id.ToString();
                var partners = stu1 + " , " + stu2;
                li.Add(new SelectListItem { Text = partners, Value = prId });

                //Remove students who have already been assigned from the unassigned list
                prPartnersVM.UnassignedStudents.Remove(student1);
                prPartnersVM.UnassignedStudents.Remove(student2);
            }

            ViewData["partners"] = li;
            return View(prPartnersVM);
        }
        
        [HttpPost]
        public async Task<IActionResult> PeerReviewSettingsAsync(PeerReviewPartnersVM prPartnersVM)
        {
            if (ModelState.IsValid)
            {
                PeerReviewPartners prPartners = new PeerReviewPartners()
                {
                    ClassId = prPartnersVM.Class.ClassId,
                    Student1Id = prPartnersVM.Student1.Id,
                    Student2Id = prPartnersVM.Student2.Id,
                };

                await data.prPartnersRepo.AddAsync(prPartners);

                return RedirectToAction("PeerReviewSettings", "Course", new { id = prPartnersVM.Class.ClassId });
            }
            return RedirectToAction("PeerReviewSettings", "Course", new { id = prPartnersVM.Class.ClassId });
        }

        [HttpGet]
        public async Task<IActionResult> PeerReviewUnassign(int id)
        {
            PeerReviewPartners prPartners = data.GetPeerReviewPartners(id);
            PeerReviewPartnersVM prPartnersVM = new PeerReviewPartnersVM();
            prPartnersVM.Class = data.GetClass(prPartners.ClassId);

            await data.prPartnersRepo.DeleteAsync(prPartners);

            return RedirectToAction("PeerReviewSettings", "Course", new { id = prPartnersVM.Class.ClassId });
        }
        
        /********************Assignment Page********************/
        [HttpGet]
        public async Task<IActionResult> AssignmentPage(int id)
        {
            AppUser user = await userManager.GetUserAsync(User);
            Assignment selectedAssignment = data.GetAssignment(id);
            AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(id);
            PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(id);
            AssignmentSubmission assignmentSubmission = await data.GetAssignmentSubmissionByStudent(id, user.Id);
            PeerReviewSubmission peerReviewSubmission = await data.GetPeerReviewSubmissionByStudent(id, user.Id);
            PeerReviewPartners peerReviewPartners = await data.GetPRPartnersByStudent(selectedAssignment.ClassId, user.Id);

            AssignmentVM assignmentVM = new AssignmentVM()
            {
                AssignmentId = selectedAssignment.AssignmentId,
                AssignmentName = selectedAssignment.AssignmentName,
                AssignmentInstructions = selectedAssignment.AssignmentInstructions,
                AssignmentDueDate = selectedAssignment.AssignmentDueDate,
                PeerReviewDueDate = selectedAssignment.PeerReviewDueDate,
                ClassId = selectedAssignment.ClassId,
            };
            if (assignmentTemplate != null)
            {
                assignmentVM.AssignmentTemplate = assignmentTemplate;
            }
            if (peerReviewTemplate != null)
            {
                assignmentVM.PeerReviewTemplate = peerReviewTemplate;
            }
            if (assignmentSubmission != null)
            {
                assignmentVM.AssignmentSubmission = assignmentSubmission;
            }
            if (peerReviewSubmission != null)
            {
                assignmentVM.PeerReviewSubmission = peerReviewSubmission;
            }
            if (peerReviewPartners != null)
            {
                assignmentVM.User = user;
                if (peerReviewPartners.Student1.Id == user.Id)
                {
                    assignmentVM.PeerReviewPartner = peerReviewPartners.Student2;
                }
                else
                {
                    assignmentVM.PeerReviewPartner = peerReviewPartners.Student1;
                }
            }
            AssignmentSubmission partnerAssignmentSubmission = null;
            PeerReviewSubmission partnerPeerReviewSubmission = null;

            if (assignmentVM.PeerReviewPartner != null)
            {
                partnerAssignmentSubmission = await data.GetAssignmentSubmissionByStudent(id, assignmentVM.PeerReviewPartner.Id);
                partnerPeerReviewSubmission = await data.GetPeerReviewSubmissionByStudent(id, assignmentVM.PeerReviewPartner.Id);
            }
            if (partnerAssignmentSubmission != null)
            {
                assignmentVM.PartnerAssignmentSubmission = partnerAssignmentSubmission;
            }
            if (partnerPeerReviewSubmission != null)
            {
                assignmentVM.PartnerPeerReviewSubmission = partnerPeerReviewSubmission;
            }

            return View(assignmentVM);
        }

        /********************Upload/Download/Delete Assignment Template********************/
        [HttpPost]
        public async Task<IActionResult> UploadAssignmentTemplate(AssignmentVM assignmentVM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = assignmentVM.AssignmentId });
            }

            byte[] formFileContent;
            try
            {
                formFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.FormFile, ModelState, _permittedExtensions, _fileSizeLimit);
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = assignmentVM.AssignmentId });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = assignmentVM.AssignmentId });
            }

            var fileEx = Path.GetExtension(assignmentVM.FormFile.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
            trustedFileNameForFileStorage += fileEx;

            var filePath = Path.Combine(_targetFilePath, "instructorAssignmentTemplates", trustedFileNameForFileStorage);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);
            }
            AssignmentTemplate assignmentTemplate = new AssignmentTemplate()
            {
                TemplateName = assignmentVM.FormFile.FileName,
                AssignmentId = assignmentVM.AssignmentId,
                ScrambledName = trustedFileNameForFileStorage
            };
            await data.AssignmentTemplatesRepo.AddAsync(assignmentTemplate);

            return RedirectToAction("AssignmentPage", "Course", new { id = assignmentVM.AssignmentId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAssignmentTemplate(int id)
        {
            AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(id);

            string path = Path.Combine(env.WebRootPath, "files/instructorAssignmentTemplates/") + assignmentTemplate.ScrambledName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", assignmentTemplate.TemplateName);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssignmentTemplate(int id, string view)
        {
            AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(id);

            string path = Path.Combine(env.WebRootPath, "files/instructorAssignmentTemplates/") + assignmentTemplate.ScrambledName;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                await data.AssignmentTemplatesRepo.DeleteAsync(assignmentTemplate);
            }
            return RedirectToAction(view, "Course", new { id = assignmentTemplate.AssignmentId });
        }

        /********************Upload/Download/Delete Peer Review Template********************/
        [HttpPost]
        public async Task<IActionResult> UploadPeerReviewTemplate(AssignmentVM assignmentVM)
        {

            var formFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.FormFile, ModelState, _permittedExtensions, _fileSizeLimit);

            var fileEx = Path.GetExtension(assignmentVM.FormFile.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
            trustedFileNameForFileStorage += fileEx;

            var filePath = Path.Combine(_targetFilePath, "instructorPeerReviewTemplates", trustedFileNameForFileStorage);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);
            }

            PeerReviewTemplate peerReviewTemplate = new PeerReviewTemplate()
            {
                TemplateName = assignmentVM.FormFile.FileName,
                AssignmentId = assignmentVM.AssignmentId,
                ScrambledName = trustedFileNameForFileStorage
            };
            await data.PeerReviewTemplatesRepo.AddAsync(peerReviewTemplate);

            return RedirectToAction("AssignmentPage", "Course", new { id = assignmentVM.AssignmentId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPeerReviewTemplate(int id)
        {
            PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(id);

            string path = Path.Combine(env.WebRootPath, "files/instructorPeerReviewTemplates/") + peerReviewTemplate.ScrambledName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", peerReviewTemplate.TemplateName);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePeerReviewTemplate(int id, string view)
        {
            PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(id);

            string path = Path.Combine(env.WebRootPath, "files/instructorPeerReviewTemplates/") + peerReviewTemplate.ScrambledName;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                await data.PeerReviewTemplatesRepo.DeleteAsync(peerReviewTemplate);
            }
            return RedirectToAction(view, "Course", new { id = peerReviewTemplate.AssignmentId });
        }

        /********************Upload/Download/Delete Assignment Submission********************/
        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(AssignmentVM inModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = inModel.AssignmentId });
            }

            byte[] formFileContent;
            try
            {
                formFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(inModel.FormFile, ModelState, _permittedExtensions, _fileSizeLimit);
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = inModel.AssignmentId });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignmentPage", "Course", new { id = inModel.AssignmentId });
            }

            var fileEx = Path.GetExtension(inModel.FormFile.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
            trustedFileNameForFileStorage += fileEx;

            var filePath = Path.Combine(_targetFilePath, "studentSubmittedAssignments", trustedFileNameForFileStorage);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);
            }

            AppUser user = await userManager.GetUserAsync(User);

            AssignmentSubmission submission = new AssignmentSubmission()
            {
                SubmissionName = inModel.FormFile.FileName,
                ScrambledName = trustedFileNameForFileStorage,
                AssignmentId = inModel.AssignmentId,
                StudentId = user.Id
            };
            await data.AssignmentSubmissionsRepo.AddAsync(submission);

            return RedirectToAction("AssignmentPage", "Course", new { id = inModel.AssignmentId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAssignmentSubmission(int id)
        {
            AssignmentSubmission assignmentSubmission = await data.GetAssignmentSubmission(id);

            string path = Path.Combine(env.WebRootPath, "files/studentSubmittedAssignments/") + assignmentSubmission.ScrambledName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", assignmentSubmission.SubmissionName);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssignmentSubmission(int id)
        {
            AssignmentSubmission assignmentSubmission = await data.GetAssignmentSubmission(id);

            string path = Path.Combine(env.WebRootPath, "files/studentSubmittedAssignments/") + assignmentSubmission.ScrambledName;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                await data.AssignmentSubmissionsRepo.DeleteAsync(assignmentSubmission);
            }
            return RedirectToAction("AssignmentPage", "Course", new { id = assignmentSubmission.AssignmentId });
        }

        /********************Upload/Download/Delete Peer Review Submission********************/

        [HttpPost]
        public async Task<IActionResult> SubmitPeerReview(AssignmentVM inModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var formFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(inModel.FormFile, ModelState, _permittedExtensions, _fileSizeLimit);

            if (formFileContent == null)
            {
                ModelState.AddModelError("", "Please upload a file for the assignment");
                return View(inModel);
            }

            var fileEx = Path.GetExtension(inModel.FormFile.FileName);
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
            trustedFileNameForFileStorage += fileEx;

            var filePath = Path.Combine(_targetFilePath, "studentSubmittedPeerReviews", trustedFileNameForFileStorage);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);
            }

            AppUser user = await userManager.GetUserAsync(User);

            PeerReviewSubmission submission = new PeerReviewSubmission()
            {
                PeerReviewName = inModel.FormFile.FileName,
                ScrambledName = trustedFileNameForFileStorage,
                AssignmentId = inModel.AssignmentId,
                StudentId = user.Id
            };
            await data.PeerReviewSubmissionsRepo.AddAsync(submission);

            return RedirectToAction("AssignmentPage", "Course", new { id = inModel.AssignmentId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPeerReview(int id)
        {
            PeerReviewSubmission peerReviewSubmission = await data.GetPeerReviewSubmission(id);

            string path = Path.Combine(env.WebRootPath, "files/studentSubmittedPeerReviews/") + peerReviewSubmission.ScrambledName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", peerReviewSubmission.PeerReviewName);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePeerReview(int id)
        {
            PeerReviewSubmission peerReviewSubmission = await data.GetPeerReviewSubmission(id);

            string path = Path.Combine(env.WebRootPath, "files/studentSubmittedPeerReviews/") + peerReviewSubmission.ScrambledName;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                await data.PeerReviewSubmissionsRepo.DeleteAsync(peerReviewSubmission);
            }
            return RedirectToAction("AssignmentPage", "Course", new { id = peerReviewSubmission.AssignmentId });
        }

        /********************Create/Edit/Delete Assignment********************/

        [HttpGet]
        public IActionResult CreateAssignment(int id)
        {
            AssignmentVM assignmentVM = new AssignmentVM()
            {
                ClassId = id
            };
            ViewBag.Caption = "Create";
            return View(assignmentVM);
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignement(int id)
        {
            Assignment assignment = data.GetAssignment(id);
            AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(id);
            PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(id);

            AssignmentVM assignmentVM = new AssignmentVM()
            {
                AssignmentId = assignment.AssignmentId,
                AssignmentName = assignment.AssignmentName,
                AssignmentInstructions = assignment.AssignmentInstructions,
                AssignmentDueDate = assignment.AssignmentDueDate,
                PeerReviewDueDate = assignment.PeerReviewDueDate,
                ClassId = assignment.ClassId

            };
            ViewBag.Caption = "Edit";
            ViewData["AssignmentTemplate"] = assignmentTemplate;
            ViewData["PeerReviewTemplate"] = peerReviewTemplate;

            return View("CreateAssignment", assignmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment(AssignmentVM assignmentVM)
        {
            if (ModelState.IsValid)
            {

                if (assignmentVM.AssignmentId == 0)
                {
                    if (assignmentVM.AssignmentTemplateFormFile == null)
                    {
                        ModelState.AddModelError("", "Please upload a file for the Assignment Template");
                        return View(assignmentVM);
                    }

                    if (assignmentVM.PeerReviewTemplateFormFile == null)
                    {
                        ModelState.AddModelError("", "Please upload a file for the Peer Review Template");
                        return View(assignmentVM);
                    }

                    var assignmentFormFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.AssignmentTemplateFormFile, ModelState, _permittedExtensions, _fileSizeLimit);

                    var fileEx = Path.GetExtension(assignmentVM.AssignmentTemplateFormFile.FileName);
                    var trustedFileNameForFileStorage = Path.GetRandomFileName();
                    trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
                    trustedFileNameForFileStorage += fileEx;

                    var filePath = Path.Combine(_targetFilePath, "instructorAssignmentTemplates", trustedFileNameForFileStorage);

                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        await fileStream.WriteAsync(assignmentFormFileContent);
                    }

                    var prFormFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.PeerReviewTemplateFormFile, ModelState, _permittedExtensions, _fileSizeLimit);

                    var prfileEx = Path.GetExtension(assignmentVM.PeerReviewTemplateFormFile.FileName);
                    var prTrustedFileNameForFileStorage = Path.GetRandomFileName();
                    prTrustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(prTrustedFileNameForFileStorage);
                    prTrustedFileNameForFileStorage += prfileEx;

                    var prFilePath = Path.Combine(_targetFilePath, "instructorPeerReviewTemplates", prTrustedFileNameForFileStorage);

                    using (var fileStream = System.IO.File.Create(prFilePath))
                    {
                        await fileStream.WriteAsync(prFormFileContent);
                    }

                    Assignment assignment = new Assignment()
                    {
                        AssignmentName = assignmentVM.AssignmentName,
                        AssignmentInstructions = assignmentVM.AssignmentInstructions,
                        AssignmentDueDate = assignmentVM.AssignmentDueDate,
                        PeerReviewDueDate = assignmentVM.PeerReviewDueDate,
                        ClassId = assignmentVM.ClassId,
                    };
                    await data.AssignmentsRepo.AddAsync(assignment);

                    
                    AssignmentTemplate assignmentTemplate = new AssignmentTemplate()
                    {
                        TemplateName = assignmentVM.AssignmentTemplateFormFile.FileName,
                        AssignmentId = assignment.AssignmentId,
                        ScrambledName = trustedFileNameForFileStorage
                    };
                    await data.AssignmentTemplatesRepo.AddAsync(assignmentTemplate);

                    PeerReviewTemplate peerReviewTemplate = new PeerReviewTemplate()
                    {
                        TemplateName = assignmentVM.PeerReviewTemplateFormFile.FileName,
                        AssignmentId = assignment.AssignmentId,
                        ScrambledName = prTrustedFileNameForFileStorage
                    };
                    await data.PeerReviewTemplatesRepo.AddAsync(peerReviewTemplate);

                    return RedirectToAction("ClassPage", "Course", new { id = assignment.ClassId });
                }
                else
                {
                    Assignment assignment = new Assignment()
                    {
                        AssignmentName = assignmentVM.AssignmentName,
                        AssignmentInstructions = assignmentVM.AssignmentInstructions,
                        AssignmentDueDate = assignmentVM.AssignmentDueDate,
                        PeerReviewDueDate = assignmentVM.PeerReviewDueDate,
                        ClassId = assignmentVM.ClassId,
                        AssignmentId = assignmentVM.AssignmentId,
                    };
                    await data.AssignmentsRepo.UpdateAsync(assignment);

                    AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(assignment.AssignmentId);
                    PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(assignment.AssignmentId);

                    if (assignmentTemplate == null)
                    {
                        if (assignmentVM.AssignmentTemplateFormFile == null)
                        {
                            ModelState.AddModelError("", "Please upload a file for the Assignment Template");
                            return View(assignmentVM);
                        }
                        else
                        {
                            var assignmentFormFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.AssignmentTemplateFormFile, ModelState, _permittedExtensions, _fileSizeLimit);

                            var fileEx = Path.GetExtension(assignmentVM.AssignmentTemplateFormFile.FileName);
                            var trustedFileNameForFileStorage = Path.GetRandomFileName();
                            trustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(trustedFileNameForFileStorage);
                            trustedFileNameForFileStorage += fileEx;

                            var filePath = Path.Combine(_targetFilePath, "instructorAssignmentTemplates", trustedFileNameForFileStorage);

                            using (var fileStream = System.IO.File.Create(filePath))
                            {
                                await fileStream.WriteAsync(assignmentFormFileContent);
                            }

                            AssignmentTemplate asmtTemplate = new AssignmentTemplate()
                            {
                                TemplateName = assignmentVM.AssignmentTemplateFormFile.FileName,
                                AssignmentId = assignment.AssignmentId,
                                ScrambledName = trustedFileNameForFileStorage
                            };
                            await data.AssignmentTemplatesRepo.AddAsync(asmtTemplate);
                        }
                    }

                    if (peerReviewTemplate == null)
                    {

                            var prFormFileContent = await FileHelpers.ProcessFormFile<UploadedFile>(assignmentVM.PeerReviewTemplateFormFile, ModelState, _permittedExtensions, _fileSizeLimit);

                            var prfileEx = Path.GetExtension(assignmentVM.PeerReviewTemplateFormFile.FileName);
                            var prTrustedFileNameForFileStorage = Path.GetRandomFileName();
                            prTrustedFileNameForFileStorage = Path.GetFileNameWithoutExtension(prTrustedFileNameForFileStorage);
                            prTrustedFileNameForFileStorage += prfileEx;

                            var prFilePath = Path.Combine(_targetFilePath, "instructorPeerReviewTemplates", prTrustedFileNameForFileStorage);

                            using (var fileStream = System.IO.File.Create(prFilePath))
                            {
                                await fileStream.WriteAsync(prFormFileContent);
                            }

                            PeerReviewTemplate prTemplate = new PeerReviewTemplate()
                            {
                                TemplateName = assignmentVM.PeerReviewTemplateFormFile.FileName,
                                AssignmentId = assignment.AssignmentId,
                                ScrambledName = prTrustedFileNameForFileStorage
                            };
                            await data.PeerReviewTemplatesRepo.AddAsync(prTemplate);
                    }


                    return RedirectToAction("ClassPage", "Course", new { id = assignment.ClassId });
                }

            }
            else
            {
                if (assignmentVM.AssignmentId != 0)
                {
                    AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(assignmentVM.AssignmentId);
                    PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(assignmentVM.AssignmentId);

                    ViewData["AssignmentTemplate"] = assignmentTemplate;
                    ViewData["PeerReviewTemplate"] = peerReviewTemplate;
                }


                ViewBag.Caption = "Create";
                return View(assignmentVM);
            }
        }

        [HttpGet]
        public IActionResult DeleteAssignment(int id)
        {
            Assignment assignment = data.GetAssignment(id);
            return View(assignment);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAssignment(Assignment assignment)
        {
            int id = assignment.ClassId;
            AssignmentTemplate assignmentTemplate = await data.GetAssignmentTemplate(assignment.AssignmentId);
            PeerReviewTemplate peerReviewTemplate = await data.GetPeerReviewTemplate(assignment.AssignmentId);
            assignment.AssignmentSubmissions = data.GetAssignmentSubmissions(assignment.AssignmentId);
            assignment.PeerReviewSubmissions = data.GetPeerReviewSubmissions(assignment.AssignmentId);
            
            foreach (AssignmentSubmission a in assignment.AssignmentSubmissions)
            {
                string path = Path.Combine(env.WebRootPath, "files/studentSubmittedAssignments/") + a.ScrambledName;
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                    await data.AssignmentSubmissionsRepo.DeleteAsync(a);
                }
            }

            foreach (PeerReviewSubmission pr in assignment.PeerReviewSubmissions)
            {
                string path = Path.Combine(env.WebRootPath, "files/studentSubmittedPeerReviews/") + pr.ScrambledName;
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                    await data.PeerReviewSubmissionsRepo.DeleteAsync(pr);
                }
                return RedirectToAction("AssignmentPage", "Course", new { id = pr.AssignmentId });
            }

            if (assignmentTemplate != null)
            {
                string aPath = Path.Combine(env.WebRootPath, "files/instructorAssignmentTemplates/") + assignmentTemplate.ScrambledName;
                FileInfo aFile = new FileInfo(aPath);
                if (aFile.Exists)
                {
                    aFile.Delete();
                }
            }

            if(peerReviewTemplate != null)
            {
                string prPath = Path.Combine(env.WebRootPath, "files/instructorPeerReviewTemplates/") + peerReviewTemplate.ScrambledName;
                FileInfo prFile = new FileInfo(prPath);
                if (prFile.Exists)
                {
                    prFile.Delete();
                }
            }

            await data.AssignmentsRepo.DeleteAsync(assignment);

            return RedirectToAction("ClassPage", "Course", new { id = id });

        }

        /********************Enrollment********************/
        [HttpGet]
        public async Task<IActionResult> Enrollment(int id)
        {
            EnrollmentVM enrollmentVM = new EnrollmentVM();

            enrollmentVM.Class = data.GetClass(id);
            enrollmentVM.UnenrolledStudents = (List<AppUser>)await userManager.GetUsersInRoleAsync("Student");

            foreach (Enrollment e in enrollmentVM.Class.Enrollments)
            {
                enrollmentVM.UnenrolledStudents.Remove(e.Student);//Remove enrolled student from the list     
            }

            return View(enrollmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> Enrollment(EnrollmentVM enrollmentVM, string studentId)
        {
            Enrollment enrollment = new Enrollment();

            enrollment.StudentId = studentId;
            enrollment.ClassId = enrollmentVM.Class.ClassId;
            await data.EnrollmentsRepo.AddAsync(enrollment);

            enrollmentVM.Class = data.GetClass(enrollment.ClassId);
            enrollmentVM.UnenrolledStudents = (List<AppUser>)await userManager.GetUsersInRoleAsync("Student");

            foreach (Enrollment e in enrollmentVM.Class.Enrollments)
            {
                enrollmentVM.UnenrolledStudents.Remove(e.Student);//Remove enrolled student from the list    
            }

            return View(enrollmentVM);
        }
        [HttpGet]
        public IActionResult Unenrollment(int id)
        {
            Enrollment enrollment = data.GetEnrollment(id);
            return View(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> Unenrollment(Enrollment enrollment)
        {
            EnrollmentVM enrollmentVM = new EnrollmentVM();

            await data.EnrollmentsRepo.DeleteAsync(enrollment);

            enrollmentVM.Class = data.GetClass(enrollment.ClassId);
            enrollmentVM.UnenrolledStudents = (List<AppUser>)await userManager.GetUsersInRoleAsync("Student");

            foreach (Enrollment e in enrollmentVM.Class.Enrollments)
            {
                enrollmentVM.UnenrolledStudents.Remove(e.Student);//Remove enrolled student from the list    
            }

            return View("Enrollment", enrollmentVM);
        }


        /********************Create Course********************/
        [HttpGet]
        public IActionResult CreateCourse()
        {
            CreateCourseVM createCourseVM = new CreateCourseVM();
            createCourseVM.Institutions = context.Institutions.OrderBy(i => i.InstitutionName).ToList();
            createCourseVM.Departments = context.Departments.OrderBy(d => d.DepartmentName).ToList();

            return View(createCourseVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseVM createCourseVM)
        {
            if (ModelState.IsValid)
            {
                Course newCourse = new Course();
                newCourse.CourseId = createCourseVM.CourseId;
                newCourse.CourseName = createCourseVM.CourseName;
                newCourse.DepartmentId = createCourseVM.Department.DepartmentId;

                await data.CourseRepo.AddAsync(newCourse);

                return RedirectToAction("Index", "Course");
            }
            else
            {
                createCourseVM.Institutions = context.Institutions.OrderBy(i => i.InstitutionName).ToList();
                return View("CreateCourse", createCourseVM);
            }
        }
        /********************Create Class********************/
        [HttpGet]
        public async Task<IActionResult> CreateClass()
        {
            CreateClassVM createClassVM = new CreateClassVM();

            createClassVM.Instructor = await userManager.GetUserAsync(User);
            createClassVM.Institution = data.GetInstitution(createClassVM.Instructor.InstitutionId);
            createClassVM.Courses = data.Courses.ToList();
            createClassVM.Terms = data.Terms(createClassVM.Instructor.InstitutionId).ToList();

            return View(createClassVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(CreateClassVM createClassVM)
        {
            if (ModelState.IsValid)
            {
                createClassVM.Course = data.GetCourse(createClassVM.Course.CourseId);
                createClassVM.Term = data.GetTerm(createClassVM.Term.TermId);

                Class newClass = new Class();
                newClass.CourseId = createClassVM.Course.CourseId;
                newClass.TermId = createClassVM.Term.TermId;
                newClass.Instructor = await userManager.FindByIdAsync(createClassVM.Instructor.Id);
                newClass.ClassName = createClassVM.Course.CourseName + " - " + createClassVM.Term.TermName;

                await data.ClassesRepo.AddAsync(newClass);

                AppUser user = await userManager.GetUserAsync(User);
                user.RoleNames = await userManager.GetRolesAsync(user);

                UserCoursesVM userCourses = new UserCoursesVM();
                userCourses._User = user;

                if (user.RoleNames.Contains("Instructor"))
                {
                    userCourses.Classes = data.GetClassesByInstructor(user.Id);
                }
                else
                {
                    userCourses.Classes = data.GetClassesByStudent(user.Id);
                }

                return View("Index", userCourses);
            }
            else
            {
                return View(createClassVM);
            }
        }
        /********************Delete Class********************/
        [HttpGet]
        public IActionResult RemoveClass(int id)
        {
            Class cls = data.GetClass(id);
            return View(cls);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveClass(Class cls)
        {
            await data.ClassesRepo.DeleteAsync(cls);

            AppUser user = await userManager.GetUserAsync(User);
            user.RoleNames = await userManager.GetRolesAsync(user);

            UserCoursesVM userCourses = new UserCoursesVM();
            userCourses._User = user;

            if (user.RoleNames.Contains("Instructor"))
            {
                userCourses.Classes = data.GetClassesByInstructor(user.Id);
            }
            else
            {
                userCourses.Classes = data.GetClassesByStudent(user.Id);
            }

            return View("Index",userCourses);
        }
    }
}
