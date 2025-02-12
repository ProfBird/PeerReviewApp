using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PeerReviewApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using PeerReviewApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace PeerReviewApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IUnitOfWork data { get; set; }

        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private RoleManager<IdentityRole> roleManager;
        private ApplicationDbContext context;

        public AdminController(IUnitOfWork uow, UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passHash, RoleManager<IdentityRole> roleMgr, ApplicationDbContext ctx)
        {
            data = uow;
            userManager = usrMgr;
            passwordHasher = passHash;
            roleManager = roleMgr;
            context = ctx;
        }

        public async Task<IActionResult> Index()
        {
            List<AppUser> users = userManager.Users.ToList();
            var roleName = "Student";
            IdentityRole studentRole = await roleManager.FindByNameAsync(roleName);
            Institution inst = new Institution();
            

            if (studentRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            foreach (AppUser user in users)
            {
                user.RoleNames = await userManager.GetRolesAsync(user);
                if(user.RoleNames.Count == 0)
                {
                    await userManager.AddToRoleAsync(user, roleName); //Default users to student role if they do not have a role assigned
                }
                    user.Institution = data.GetInstitution(user.InstitutionId);
            }
            

            UserViewModel model = new UserViewModel
            {
                Users = users, // List of AppUsers
                Roles = roleManager.Roles
            };

            return View(model);
        }



        /********************View/Edit/Delete User********************/
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddUserByInstitution(int institutionId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterVM model)
        {

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName };
                var roleName = "Student";

                IdentityRole userRole = await roleManager.FindByNameAsync(roleName);

                if (userRole == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        public async Task<IActionResult> UserDetails(string id)
        {
            UserViewModel userViewModel = new UserViewModel()
            {
                User = await userManager.FindByIdAsync(id)
            };

            userViewModel.User.RoleNames = await userManager.GetRolesAsync(userViewModel.User);

            if (await userManager.IsInRoleAsync(userViewModel.User, "Instructor"))
            {
                userViewModel.User.Classes = data.GetClassesByInstructor(userViewModel.User.Id);
            }
            else
            {
                userViewModel.User.Classes = data.GetClassesByStudent(userViewModel.User.Id);
            }

            return View(userViewModel);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            UserViewModel userViewModel = new UserViewModel()
            {
                User = await userManager.FindByIdAsync(id)
            };

            userViewModel.User.RoleNames = await userManager.GetRolesAsync(userViewModel.User);
            userViewModel.User.Classes = data.GetClassesByStudent(userViewModel.User.Id);

            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel userViewModel)
        {
            // get user object
            var user = await userManager.FindByIdAsync(userViewModel.User.Id);
            if (ModelState.IsValid)
            {
                //Check if any of the user view model fields have changed and if so, update them in the user variable
                user.UserName = (user.UserName != userViewModel.User.UserName) ? userViewModel.User.UserName : user.UserName;
                user.FirstName = (user.FirstName != userViewModel.User.FirstName) ? userViewModel.User.FirstName : user.FirstName;
                user.LastName = (user.LastName != userViewModel.User.LastName) ? userViewModel.User.LastName : user.LastName;
                user.Email = (user.Email != userViewModel.User.Email) ? userViewModel.User.Email : user.Email;

                await userManager.UpdateAsync(user);

                return RedirectToAction("UserDetails", "Admin", new { id = userViewModel.User.Id });
            }
            else
            {
                return View(userViewModel);
            }
        }

        [HttpPost]
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

        public async Task<IActionResult> Update(string id)
        {
            AppUser user = await userManager.FindByNameAsync(id);
            if(user != null)
                return View(user);
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            AppUser user = await userManager.FindByNameAsync(id);
            if (user != null)
            {
                if(!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                {
                    ModelState.AddModelError("", "Email field can not be empty");
                }

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password field can not be empty");

                if(!string.IsNullOrEmpty(email)&& !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
                
            }
            else
                ModelState.AddModelError("", "User not found");
            return View(user);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }





        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                };

                IdentityResult result = await userManager.CreateAsync(appUser, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = model.Email }, Request.Scheme);
                    EmailHelper emailHelper = new EmailHelper();
                    bool emailResponse = emailHelper.SendEmail(model.Email, confirmationLink);

                    if (emailResponse)
                        return RedirectToAction("Index");
                    else
                    {
                        // log email failed 
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
