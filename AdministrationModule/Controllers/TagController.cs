using AdministrationModule.Models;
using AutoMapper;
using BL.Model;
using BLL.Services;
using DAL.Model;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationModule.Controllers
{
    public class TagController : Controller
    {
        private readonly TagService _tagService;
        private readonly IMapper _mapper;

        public TagController(TagService tagService, IMapper mapper)
        {
            this._tagService = tagService;
            _mapper = mapper;
        }

        // GET: TagController
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<VMTag>>(await _tagService.GetAll()));
        }

        // GET: TagController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(_mapper.Map<VMTag>(await _tagService.GetById(id)));
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VMTag tag)
        {
            try
            {
                if (await _tagService.GetByName(tag.Name) != null || !ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data is invalid or that tag allready exists!");
                    return View(tag);
                }

                await _tagService.Add(
                    new BLTag
                    {
                        Name = tag.Name,
                    }
                );
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(_mapper.Map<VMTag>(await _tagService.GetById(id)));
        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, VMTag freshTag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Data invalid!");
                    return View(freshTag);
                }

                BLTag tag = await _tagService.GetById(id);
                tag.Name = freshTag.Name;
                await _tagService.Update(tag);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(_mapper.Map<VMTag>(await _tagService.GetById(id)));
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await _tagService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
