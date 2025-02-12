using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReviewApp.Data;
using PeerReviewApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerReviewApp.Controllers
{
    public class AccountController : Controller
    {
        private IUnitOfWork data { get; set; }
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
        private ApplicationDbContext context;
        public List<Institution> institutions;
        public int institutionId;


        public AccountController(IUnitOfWork uow, UserManager<AppUser> userMngr, SignInManager<AppUser> signInMngr, ApplicationDbContext ctx, RoleManager<IdentityRole> roleMngr)
        {
            data = uow;
            userManager = userMngr;
            signInManager = signInMngr;
            context = ctx;
            roleManager = roleMngr;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM inModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    inModel.UserName,
                    inModel.Password,
                    isPersistent: inModel.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Course");
                }
            }
            ModelState.AddModelError("", "Invalid username/password.");
            return View(inModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterVM registerVM = new RegisterVM();
            institutions = context.Institutions.OrderBy(i => i.InstitutionName).ToList();

            registerVM.Institutions = institutions;

            return View(registerVM);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM inModel)
        {
            institutions = context.Institutions.OrderBy(i => i.InstitutionName).ToList();

            if (ModelState.IsValid)
            {

                Institution _institution = new Institution();

                string selected = inModel.InstitutionName;

                foreach (Institution inst in institutions)
                {
                    if (inst.InstitutionName == selected)
                    {
                        _institution = inst;
                        institutionId = inst.InstitutionId;
                    };
                }

                //Check if user identified as instrctor, if so, verify instructor code. If code is incorrect throw error 
                if (inModel.InstructorCode != _institution.InstructorCode && inModel.IsInstructor)
                {
                    ModelState.AddModelError("", "Invalid instructor code");
                    inModel.Institutions = institutions;
                    return View(inModel);
                }

                var user = new AppUser
                {
                    UserName = inModel.UserName,
                    FirstName = inModel.FirstName,
                    LastName = inModel.LastName,
                    Email = inModel.Email,
                    InstitutionId = institutionId,
                };

                var result = await userManager.CreateAsync(user, inModel.Password);
                if (result.Succeeded)
                {
                    var roleName = "";

                    if (inModel.IsInstructor)
                    {
                        roleName = "Instructor";
                    }
                    else
                    {
                        roleName = "Student";
                    }

                    //Check to see if role exists, if not, create it. Otherwise add user to appropriate role
                    IdentityRole identityRole = await roleManager.FindByNameAsync(roleName);
                    if (identityRole == null)
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (roleResult.Succeeded)
                        {
                            var addUserResult = await userManager.AddToRoleAsync(user, roleName);
                            if (addUserResult.Succeeded)
                            {
                                await signInManager.SignInAsync(user, isPersistent: false);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        var addResult = await userManager.AddToRoleAsync(user, roleName);
                        if (addResult.Succeeded)
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            inModel.Institutions = institutions;
            return View(inModel);
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager?.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public ViewResult AccessDenied()
        {
            return View();
        }

        /********************View/Edit/Delete User********************/
        [HttpGet]
        public IActionResult AddUserByInstitution(int id)
        {
            Institution institution = data.GetInstitution(id);

            RegisterVM registerVM = new RegisterVM()
            {
                InstitutionName = institution.InstitutionName,
                InstitutionId = id,
            };
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserByInstitution(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = registerVM.UserName,
                    FirstName = registerVM.FirstName,
                    LastName = registerVM.LastName,
                    Email = registerVM.Email,
                    InstitutionId = registerVM.InstitutionId,
                };

                var result = await userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    var roleName = "";

                    if (registerVM.IsInstructor)
                    {
                        roleName = "Instructor";
                    }
                    else
                    {
                        roleName = "Student";
                    }
                }

                return RedirectToAction("Details", "Institution", new { id = registerVM.InstitutionId });

            }
            else
            {
                return View(registerVM);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User not found");

            return View("Index", userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByNameAsync(id);
            if (user != null)
                return View(user);
            else
            {
                return RedirectToAction("Index");
            }
        }

        /********************Modify Roles********************/
        public async Task<IActionResult> AddToAdmin(string id)
        {
            IdentityRole adminRole = await roleManager.FindByNameAsync("Admin");
            AppUser user = await userManager.FindByIdAsync(id);

            if (adminRole == null)
            {
                TempData["Message"] = "Admin role does not exist. " +
                "Click 'Create Admin Role' button to create it";
            }
            else
            {
                await userManager.AddToRoleAsync(user, adminRole.Name);
            }

            return RedirectToAction("Edit", "Institution", new { id = user.InstitutionId});
        }

        public async Task<IActionResult> RemoveFromAdmin(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            var result = await userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded) { }

            return RedirectToAction("Edit", "Institution", new { id = user.InstitutionId });
        }

        public async Task<IActionResult> AddToinstructor(string id)
        {
            IdentityRole instructorRole = await roleManager.FindByNameAsync("Instructor");
            AppUser user = await userManager.FindByIdAsync(id);

            if (instructorRole == null)
            {
                TempData["Message"] = "Admin role does not exist. " +
                "Click 'Create Admin Role' button to create it";
            }
            else
            {
                await userManager.AddToRoleAsync(user, instructorRole.Name);
            }

            return RedirectToAction("Edit", "Institution", new { id = user.InstitutionId });
        }

        public async Task<IActionResult> RemoveFromInstructor(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            IdentityRole instructorRole = await roleManager.FindByNameAsync("Instructor");

            var result = await userManager.RemoveFromRoleAsync(user, instructorRole.Name);
            if (result.Succeeded) { }

            return RedirectToAction("Edit", "Institution", new { id = user.InstitutionId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminRole()
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            return RedirectToAction("Index");
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}

