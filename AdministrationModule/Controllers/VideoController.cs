using AdministrationModule.Models;
using AdministrationModule.Pager;
using AutoMapper;
using BL.Model;
using BLL.Services;
using DAL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace AdministrationModule.Controllers
{
    public class VideoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly VideoService _videoService;
        private readonly TagService _tagService;
        private readonly GenreService _genreService;
        private readonly ImageService _imageService;
        private readonly VideoTagService _videoTagService;

        public VideoController(IMapper mapper, VideoService videoService, TagService tagService,
            GenreService genreService, ImageService imageService, VideoTagService videoTagService)
        {
            _mapper = mapper;
            _videoService = videoService;
            _tagService = tagService;
            _genreService = genreService;
            _imageService = imageService;
            _videoTagService = videoTagService;
        }

        private async Task LoadViewBagData()
        {
            var genres = _mapper.Map<IEnumerable<VMGenre>>(await _genreService.GetAll());
            ViewBag.GenreId = new SelectList(genres, "Id", "Name");
            
            var images = _mapper.Map<IEnumerable<VMImage>>(await _imageService.GetAll());
            ViewBag.ImageId = new SelectList(images, "Id", "Content");
            ViewBag.Images = images;

            var vtags = _mapper.Map<IEnumerable<VMTag>>(await _tagService.GetAll());
            ViewBag.Tags = new SelectList(vtags, "Id", "Name");
        }

        // GET: VideoController
        public async Task<ActionResult> Index(string sortBy = null, int page = 1)
        {
            if(string.IsNullOrEmpty(sortBy))
            {
                Request.Cookies.TryGetValue("VideoFilter", out string value);
                if (value != null) sortBy = value;
            } 
            else
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };
                Response.Cookies.Append("VideoFilter", sortBy, cookieOptions);
            }

            var videos = _mapper.Map<IEnumerable<VMVideo>>(await _videoService.GetAll());

            switch (sortBy.Trim().ToLower())
            {
                case "genre":
                    videos = videos.OrderBy(v => v.Genre.Name);
                    break;
                case "name desc":
                    videos = videos.OrderByDescending(v => v.Name);

                    break;
                case "genre desc":
                    videos = videos.OrderByDescending(v => v.Genre.Name);
                    break;
                default:
                    videos = videos.OrderBy(v => v.Name);
                    break;
            }

            const int pageSize = 2;
            var pager = new MyPager((await _videoService.GetAll()).Count(), page, pageSize);

            var data = videos.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.Pager = pager;
            ViewBag.CurrentSortOrder = sortBy;

            return View(data);
        }

        // GET: VideoController/Create
        public async Task<ActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        // POST: VideoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VMVideo video)
        {
            try
            {

                /*
                string[] tags = video.NewTags.Split(',');
                var allTags = await _tagService.GetAll();
                var existingTags = allTags.Where(t => tags.Contains(t.Name));
                */

                var dalVideo = new BLVideo
                {
                    Id = video.Id,
                    CreatedAt = video.CreatedAt,
                    Name = video.Name,
                    Description = video.Description,
                    GenreId = video.GenreId,
                    TotalSeconds = video.TotalSeconds,
                    StreamingUrl = video.StreamingUrl,
                    ImageId = video.ImageId,
                    //VideoTags = existingTags.Select(t => new BLVideoTag { TagId = t.Id }).ToList()
                };

                /*
                foreach (var tag in tags)
                {
                    if (!allTags.Any(t => t.Name == tag))
                    {
                        var newTag = new BLTag { Name = tag };
                        await _tagService.Add(newTag);
                        dalVideo.VideoTags.Add(new BLVideoTag { Tag = await _tagService.GetByName(newTag.Name) });
                    }
                }
                */

                await _videoService.Add(dalVideo);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VideoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await LoadViewBagData();
            return View(_mapper.Map<VMVideo>(await _videoService.GetById(id)));
        }

        // POST: VideoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, VMVideo freshVideo)
        {
            try
            {

                var video = await _videoService.GetById(id);

                video.Id = freshVideo.Id;
                video.Name = freshVideo.Name;
                video.Description = freshVideo.Description;
                video.TotalSeconds = freshVideo.TotalSeconds;
                video.StreamingUrl = freshVideo.StreamingUrl;
                video.GenreId = freshVideo.GenreId;
                video.ImageId = freshVideo.ImageId;
                
                /*
                var videoTagsToAdd = _mapper.Map<IEnumerable<BLVideoTag>>(freshVideo.VideoTags).Where(
                    vt => !video.VideoTags.Contains(vt));

                foreach (var videoTagToAdd in videoTagsToAdd)
                {
                    video.VideoTags.Add(videoTagToAdd);

                    if (!(await _tagService.GetAll()).Contains(videoTagToAdd.Tag))
                    {
                        await _tagService.Add(videoTagToAdd.Tag);   
                    }
                }
                */

                await _videoService.Update(video);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VideoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VideoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var dalVideo = await _videoService.GetById(id);
                if (dalVideo == null)
                    return NotFound();

                foreach (var videoTag in dalVideo.VideoTags.ToList())
                {
                    await _videoTagService.Delete(videoTag.Id);
                }

                await _videoService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
