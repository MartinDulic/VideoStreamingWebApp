using AdministrationModule.Models;
using AutoMapper;
using BL.Model;
using BLL.Services;
using DAL.Model;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationModule.Controllers
{
    public class GenreController : Controller
    {
        private readonly  GenreService _genreService;
        private readonly IMapper _mapper;

        public GenreController(GenreService genreService, IMapper mapper)
        {
            _genreService = genreService;
            _mapper = mapper;
        }

        // GET: GenreController
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<VMGenre>>(await _genreService.GetAll()));
        }

        // GET: GenreController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(_mapper.Map<VMGenre>(await _genreService.GetById(id)));
        }

        // GET: GenreController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenreController/Create
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] VMGenre genre)
        {
            try
            {
                if (await _genreService.GetByName(genre.Name) != null || !ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data is invalid or that genre allready exists!");
                    return View(genre);
                }

                await _genreService.Add(
                    new BLGenre
                    {
                        Name = genre.Name,
                        Description = genre.Description
                    }
                );
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // GET: GenreController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(_mapper.Map<VMGenre>(await _genreService.GetById(id)));
        }

        // POST: GenreController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, VMGenre freshGenre)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data invalid!");
                    return View(freshGenre);
                }

                BLGenre genre = await _genreService.GetById(id);
                genre.Name = freshGenre.Name;
                genre.Description = freshGenre.Description;
                await _genreService.Update(genre);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GenreController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(_mapper.Map<VMGenre>(await _genreService.GetById(id)));
        }

        // POST: GenreController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await _genreService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
