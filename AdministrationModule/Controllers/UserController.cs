using AdministrationModule.Models;
using AutoMapper;
using BL.Model;
using BLL.Services;
using DAL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace AdministrationModule.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;

        private readonly UserService _userService;
        private readonly CountryService _countryService;

        public UserController(IMapper mapper, UserService userService, CountryService countryService)
        {
            _mapper = mapper;
            _userService = userService;
            _countryService = countryService;
        }

        private async Task LoadViewBagData()
        {
            var country = _mapper.Map<IEnumerable<VMCountry>>(await _countryService.GetAll());
            ViewBag.CountryOfResidenceId = new SelectList(country, "Id", "Name");
        }
        // GET: UserController
        public async Task<ActionResult> Index(string sortBy = null)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                Request.Cookies.TryGetValue("UserFilter", out string value);
                if (value != null) sortBy = value;
                else sortBy = "";
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };
                Response.Cookies.Append("UserFilter", sortBy, cookieOptions);
            }

            var users = _mapper.Map<IEnumerable<VMUser>>(await _userService.GetAll());

            switch (sortBy.Trim().ToLower())
            {
                case "Last name":
                    users = users.OrderBy(u => u.LastName);
                    break;
                case "Username":
                    users = users.OrderBy(u => u.Username);
                    break;
                case "Country":
                    users = users.OrderBy(u => u.CountryOfResidence.Name);
                    break;
                case "First name desc":
                    users = users.OrderByDescending(u => u.FirstName);
                    break;
                case "Last name desc":
                    users = users.OrderByDescending(u => u.LastName);
                    break;
                case "Username desc":
                    users = users.OrderByDescending(u => u.Username);
                    break;
                case "Country desc":
                    users = users.OrderByDescending(u => u.CountryOfResidence.Name);
                    break;
                default:
                    users = users.OrderBy(u => u.FirstName);
                    break;
            }

            return View(users);
        }

        // GET: UserController/Create
        public async Task<ActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VMCreateUser user)
        {
            try
            {
                var users = await _userService.GetAll();
                if (users.FirstOrDefault(u => u.Username == user.Username) != null ||
                    users.FirstOrDefault(u => u.Email == user.Email) != null) 
                {
                    ModelState.AddModelError(string.Empty, "The email or username is already in use, Please try another.");
                    await LoadViewBagData();
                    return View(user);
                }

                await _userService.Add(
                       new DAL.Requests.UserRequest
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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return View();
            }
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await LoadViewBagData();
            return View(_mapper.Map<VMUser>(await _userService.GetById(id)));
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, VMUser user)
        {
            try
            {
                var dalUser = await _userService.GetById(id);

                dalUser.Username = user.Username;
                dalUser.FirstName = user.FirstName;
                dalUser.LastName = user.LastName;
                dalUser.Email = user.Email;
                dalUser.Phone = user.Phone;
                dalUser.IsConfirmed = user.IsConfirmed;
                dalUser.CountryOfResidenceId = user.CountryOfResidenceId;

                await _userService.Update(dalUser);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(_mapper.Map<VMUser>(await _userService.GetById(id)));
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, string placeholder)
        {
            try
            {
                var dalUser = await _userService.GetById(id);
                dalUser.DeletedAt = DateTime.UtcNow;
                await _userService.Update(dalUser);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
