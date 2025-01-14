using BookleWebApp.Data;
using BookleWebApp.Models;
using BookleWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookleWebApp.Controllers
{
    public class AccountController : Controller
    {

        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> _signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            this.userManager = userManager;
            signInManager = _signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                FullName = model.FullName,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
            
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            { 
                if(model.Role == "User")
                {
                    var roleResult = await userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        User usr = new User();
                        usr.Id = user.Id;
                        usr.Email = user.Email;
                        usr.FullName = user.FullName;
                        usr.PhoneNumber = user.PhoneNumber;
                        _context.AddAsync(usr);
                        _context.SaveChanges();
                        return RedirectToAction("Login");
                    }
                    foreach (var err in roleResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
                else if(model.Role == "Publisher")
                {
                    var roleResult = await userManager.AddToRoleAsync(user, "Publisher");
                    if (roleResult.Succeeded)
                    {
                        Publisher usr = new Publisher();
                        usr.Id = user.Id;
                        usr.Email = user.Email;
                        usr.Name = user.FullName;
                        usr.PhoneNumber = user.PhoneNumber;
                        _context.AddAsync(usr);
                        _context.SaveChanges();
                        return RedirectToAction("Login");
                    }
                    foreach (var err in roleResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
               
                    
            }
            else
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (User.IsInRole("User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (User.IsInRole("Publisher"))
                    {
                        var id = userManager.GetUserId(User);
                        return RedirectToAction("Index", "Publishers");
                    }
                    else if (User.IsInRole("Admin"))
                    {
                        var id = userManager.GetUserId(User);
                        return RedirectToAction("Index", "Home", new { area = "Dashboard" });
                    }

                }
                ModelState.AddModelError("", "Invalid User or password");
                return View(model);

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            IdentityRole role = new IdentityRole
            {
                Name = model.RoleName
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("RolesList");
            }
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }
            return View(model);
        }
        public IActionResult RolesList()
        {
            return View(_roleManager.Roles);
        }
        public async Task<IActionResult> EditRole(string? id)
        {

            if (id != null)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    EditRoleViewModel editeModel = new EditRoleViewModel();
                    editeModel.RoleName = role.Name;
                    editeModel.RoleId = role.Id;
                    return View(editeModel);
                }

            }
            return RedirectToAction("RolesList");
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role != null)
            {
                role.Name = model.RoleName;
            }
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("RolesList");
            }
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteRole(string? id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {

                return RedirectToAction("RolesList");
            }
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }
            return RedirectToAction("RolesList");
        }
    }
}
