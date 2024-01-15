using AdministrationModule.Models;
using AutoMapper;
using BL.Model;
using BLL.Services;
using AdministrationModule.Pager;
using DAL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationModule.Controllers
{
    public class CountryController : Controller
    {
        private readonly CountryService _countryService;
        private readonly IMapper _mapper;

        public CountryController(CountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }


        // GET: CountryController
        public async Task<ActionResult> Index(int page = 1)
        {
            const int pageSize = 2;
            var pager = new MyPager((await _countryService.GetAll()).Count(), page, pageSize);

            int itemsToSkip = (page - 1) * pageSize;

            var data = _mapper.Map<IEnumerable<VMCountry>>(await _countryService.GetAll()).Skip(itemsToSkip)
                .Take(pageSize);
            ViewBag.Pager = pager;
            return View(data);
        }

        // GET: CountryController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(_mapper.Map<VMCountry>(await _countryService.GetById(id)));
        }

        // GET: CountryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CountryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VMCountry country)
        {
            try
            {
                if (await _countryService.GetById(country.Id) != null || !ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data is invalid or that tag allready exists!");

                    return View(country);
                }

                await _countryService.Add(
                    new BLCountry
                    {
                        Name = country.Name,
                        Code = country.Code.ToUpper().Trim()
                    });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return View();
            }
        }

        // GET: CountryController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(_mapper.Map<VMCountry>(await _countryService.GetById(id)));
        }

        // POST: CountryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, VMCountry freshCountry)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data invalid!");
                    return View(freshCountry);
                }

                BLCountry country = await _countryService.GetById(id);
                country.Name = freshCountry.Name;
                country.Code = freshCountry.Code;
                await _countryService.Update(country);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CountryController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(_mapper.Map<VMCountry>(await _countryService.GetById(id)));
        }

        // POST: CountryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await _countryService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
