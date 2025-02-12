using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerReviewApp.Data;
using PeerReviewApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PeerReviewApp.Controllers
{
    public class InstitutionController : Controller
    {
        private IUnitOfWork data { get; set; }
        private ApplicationDbContext context;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;

        public InstitutionController(IUnitOfWork uow, ApplicationDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            data = uow;
            context = ctx;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var institutions = data.Institutions;

            return View(institutions);
        }

        public async Task<IActionResult> Details(int id)
        {
            Institution institution = data.GetInstitution(id);

            InstitutionVM institutionVM = new InstitutionVM()
            {
                InstitutionName = institution.InstitutionName,
                Departments = institution.Departments,
                Members = institution.Members,
                // Terms = data.Terms(id).ToList()
                Terms = institution.Terms,
            };
            foreach (AppUser user in institutionVM.Members)
            {
                user.RoleNames = await userManager.GetRolesAsync(user);
            }

            return View(institutionVM);
        }

        /*********Create/Update/Delete Institution*********/
        [HttpGet]
        public IActionResult Create(int id)
        {
            var institution = data.GetInstitution(id);
            return View(institution);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Institution institution)
        {
            if (ModelState.IsValid)
            {
                Institution inst = new Institution
                {
                    InstitutionName = institution.InstitutionName,
                    InstructorCode = institution.InstructorCode,
                };
                await data.InstitutionsRepo.AddAsync(inst);
            }
            else
            {
                ModelState.AddModelError("", "Error");
            }

            var _institutions = data.Institutions;

            return View("Index", _institutions);

        }

        public async Task<IActionResult> Edit(int id)
        {
            Institution institution = data.GetInstitution(id);

            InstitutionVM institutionVM = new InstitutionVM()
            {
                InstitutionId = id,
                InstructorCode = institution.InstructorCode,
                InstitutionName = institution.InstitutionName,
                Departments = institution.Departments,
                Members = institution.Members,
                Terms = institution.Terms,
            };
            foreach (AppUser user in institutionVM.Members)
            {
                user.RoleNames = await userManager.GetRolesAsync(user);
            }
            return View(institutionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Institution institution)
        {
            if (ModelState.IsValid)
            {
                await data.InstitutionsRepo.UpdateAsync(institution);
            }
            else
            {
                return View(institution.InstitutionId);
            }

            var _institutions = data.Institutions;

            return View("Index", _institutions);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Institution institution = data.GetInstitution(id);
            return View(institution);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Institution institution)
        {
            await data.InstitutionsRepo.DeleteAsync(institution);

            var _institutions = data.Institutions;

            return View("Index", _institutions);
        }

        /*********Create/Update/Delete Department*********/
        [HttpGet]
        public IActionResult CreateDepartment(int id)
        {
            DepartmentVM departmentVM = new DepartmentVM();

            departmentVM.InstitutionId = id;
            ViewBag.Caption = "Create";
            return View(departmentVM);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(DepartmentVM departmentVM)
        {
            if (ModelState.IsValid)
            {
                if(departmentVM.DepartmentId == 0)
                {
                    Department department = new Department()
                    {
                        DepartmentName = departmentVM.DepartmentName,
                        InstitutionId = departmentVM.InstitutionId
                    };
                    await data.DepartmentsRepo.AddAsync(department);
                }
                else
                {
                    Department department = new Department()
                    {
                        DepartmentId = departmentVM.DepartmentId,
                        DepartmentName = departmentVM.DepartmentName,
                        InstitutionId = departmentVM.InstitutionId
                    };

                    await data.DepartmentsRepo.UpdateAsync(department);
                }
            }
            else
            {
                ViewBag.Caption = "Create";
                return View(departmentVM);
            }
            Institution institution = data.GetInstitution(departmentVM.InstitutionId);
            return View("Edit", institution);
        }
        [HttpGet]
        public IActionResult EditDepartment(int id)
        {
            Department department = data.GetDepartment(id);

            DepartmentVM departmentVM = new DepartmentVM()
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                InstitutionId = department.InstitutionId,
            };

            ViewBag.Caption = "Edit";

            return View("CreateDepartment", departmentVM);
        }

        [HttpGet]
        public IActionResult DeleteDepartment(int id)
        {
            Department department = data.GetDepartment(id);

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(DepartmentVM departmentVM)
        {
            Institution institution = data.GetInstitution(departmentVM.InstitutionId);
            Department department = data.GetDepartment(departmentVM.DepartmentId);

            await data.DepartmentsRepo.DeleteAsync(department);

            return View("Edit", institution);
        }
        /***********Create/Update/Delete Term************/
        [HttpGet]
        public IActionResult CreateTerm(int id)
        {
            Term term = new Term();

            term.InstitutionId = id;

            TermVM termVM = new TermVM()
            {
                TermId = id,
                TermName = term.TermName,
                TermBeginDate = term.TermBeginDate,
                TermEndDate = term.TermEndDate,
                InstitutionId = (int)term.InstitutionId,
            };

            ViewBag.Caption = "Create";
            return View(termVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTerm(TermVM termVM)
        {
            if (ModelState.IsValid)
            {
                if (termVM.TermId == 0)
                {
                    Term term = new Term()
                    {
                        TermName = termVM.TermName,
                        TermBeginDate = termVM.TermBeginDate,
                        TermEndDate = termVM.TermEndDate,
                        InstitutionId = termVM.InstitutionId,
                    };
                    await data.TermsRepo.AddAsync(term);
                }
                else
                {
                    Term term = new Term()
                    {
                        TermId = termVM.TermId,
                        TermName = termVM.TermName,
                        TermBeginDate = termVM.TermBeginDate,
                        TermEndDate = termVM.TermEndDate,
                        InstitutionId = termVM.InstitutionId,
                    };
                    await data.TermsRepo.UpdateAsync(term);
                }
            }
            else
            {
                ViewBag.Caption = "Create";
                return View(termVM);
            }
            Institution institution = data.GetInstitution(termVM.InstitutionId);
            return View("Edit", institution);
        }
 
        public IActionResult EditTerm(int id)
        {
            Term term = data.GetTerm(id);

            TermVM termVM = new TermVM()
            {
                TermId = id,
                TermName = term.TermName,
                TermBeginDate = term.TermBeginDate,
                TermEndDate = term.TermEndDate,
                InstitutionId = (int)term.InstitutionId,
            };

            ViewBag.Caption = "Edit";

            return View("CreateTerm", termVM);
        }

        [HttpGet]
        public IActionResult DeleteTerm(int id)
        {
            Term term = data.GetTerm(id);

            return View(term);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTerm(TermVM termVM)
        {
            Institution institution = data.GetInstitution(termVM.InstitutionId);
            Term term = data.GetTerm(termVM.TermId);

            await data.TermsRepo.DeleteAsync(term);

            return View("Edit", institution);
        }
    }

}
