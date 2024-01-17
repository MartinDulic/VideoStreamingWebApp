using AutoMapper;
using BLL.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicModule.Models;
using System.Security.Claims;
using DAL.Requests;

namespace PublicModule.Controllers
{
    public class UserController : Controller
    {

        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly CountryService _countryService;
        private readonly IMapper _mapper;

        public UserController(UserService userService, NotificationService notificationService, CountryService countryService,
            IMapper mapper)
        {
            _userService = userService;
            _notificationService = notificationService;
            _countryService = countryService;
            _mapper = mapper;
        }

        private async Task LoadViewBagData()
        {
            var country = _mapper.Map<IEnumerable<VMCountry>>(await _countryService.GetAll());
            ViewBag.CountryOfResidenceId = new SelectList(country, "Id", "Name");
        }

        // GET: UserController/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = false;
            ViewBag.ShowLogout = true;
            ViewBag.ShowUsername = true;
            return View(_mapper.Map<VMUser>(await _userService.GetById(id)));
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = true;
            ViewBag.ShowLogout = false;
            ViewBag.ShowUsername = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUserLogin loginInfo)
        {
            try
            {
                if (!ModelState.IsValid) return View(loginInfo);

                var user = await _userService.GetConfirmedUser(
                    loginInfo.Username,
                    loginInfo.Password);

                // if user is null, add error to ModelState and return view
                if (user == null)
                {
                    ViewBag.ShowLogin = false;
                    ViewBag.ShowRegister = true;
                    ViewBag.ShowLogout = false;
                    ViewBag.ShowUsername = false;
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(loginInfo);
                } else if (user.DeletedAt != null)
                {
                    ViewBag.ShowLogin = false;
                    ViewBag.ShowRegister = true;
                    ViewBag.ShowLogout = false;
                    ViewBag.ShowUsername = false;
                    ModelState.AddModelError("", "Your account has been deleted!");
                    return View(loginInfo);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties());

                return RedirectToAction("Index", "Video");
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                throw;
            }
        }

        public async Task ConfirmEmail(string email, string securityToken)
        {
            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = false;
            ViewBag.ShowLogout = false;
            ViewBag.ShowUsername = false;

            var dalUser = (await _userService.GetAll()).FirstOrDefault(u => 
                u.Email == email && u.SecurityToken == securityToken);

            dalUser.IsConfirmed = true;
            await _userService.Update(dalUser);
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.ShowLogin = true;
            ViewBag.ShowRegister = false;
            ViewBag.ShowLogout = false;
            ViewBag.ShowUsername = false;
            await LoadViewBagData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMUserRegister user)
        {
            try
            {
                var users = await _userService.GetAll();
                var existingUser = users.FirstOrDefault(c => c.Username == user.Username);
                var existingEmail = users.FirstOrDefault(c => c.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "The username is already in use, Please try another.");
                    await LoadViewBagData();
                    return View(user);
                }
                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "The email is already in use, Please try another.");
                    await LoadViewBagData();
                    return View(user);
                }

                await _userService.Add(
                         new UserRequest
                         {
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             Username = user.Username,
                             Email = user.Email,
                             Phone = user.Phone,
                             Country = user.Country,
                             Password = user.Password
                         }
                     );

                var dalUser = (await _userService.GetAll()).FirstOrDefault(u => u.Email == user.Email);

                await ConfirmEmail(dalUser.Email, dalUser.SecurityToken);

                return RedirectToAction(nameof(Login));
            }
            catch
            {
                ViewBag.ShowLogin = false;
                ViewBag.ShowRegister = false;
                ViewBag.ShowLogout = true;
                ViewBag.ShowUsername = true;
                return View();
            }
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = false;
            ViewBag.ShowLogout = true;
            ViewBag.ShowUsername = true;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(VMChangePassword changePassword)
        {
            try
            {
                if (!ModelState.IsValid) return View(changePassword);
                await _userService.ChangePassword(changePassword.Username, changePassword.NewPassword);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Check your credentials, something is wrong!");
                return View(changePassword);
            }

            return RedirectToAction("Index", "Video");
        }
    }
}
