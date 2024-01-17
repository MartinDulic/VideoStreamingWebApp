using AdministrationModule.Pager;
using AutoMapper;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PublicModule.Models;
using System.Globalization;

namespace PublicModule.Controllers
{
    public class VideoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly VideoService _videoService;
        private readonly TagService _tagService;
        private readonly GenreService _genreService;
        private readonly ImageService _imageService;
        private readonly VideoTagService _videoTagService;

        public VideoController(IMapper mapper, VideoService videoService, TagService tagService, GenreService genreService,
            ImageService imageService, VideoTagService videoTagService)
        {
            _mapper = mapper;
            _videoService = videoService;
            _tagService = tagService;
            _genreService = genreService;
            _imageService = imageService;
            _videoTagService = videoTagService;
        }

        // GET: VideoController
        [Authorize]

        public async Task<ActionResult> Index(string sortBy = null, string searchQuery = null, int page = 1)
        {
            if (string.IsNullOrEmpty(sortBy))
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

            if (!string.IsNullOrEmpty(searchQuery))
            {
                videos = videos.Where(v =>
                    v.Name.Trim().ToLower().Contains(searchQuery.Trim().ToLower())
                    || v.Description.Trim().ToLower().Contains(searchQuery.Trim().ToLower())
                    || v.Genre.Name.Trim().ToLower().Contains(searchQuery.Trim().ToLower())
                    || v.VideoTags.Any(t => t.Tag.Name.Trim().ToLower().Contains(searchQuery.Trim().ToLower())));
            }

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
            var pager = new MyPager(videos.Count(), page, pageSize);
            var data = videos.Skip((page - 1) * pageSize).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = false;
            return View(data);
        }

        // GET: VideoController/Details/5
        [Authorize]

        public async Task<ActionResult> Details(int id)
        {
            ViewBag.ShowLogin = false;
            ViewBag.ShowRegister = false;
            ViewBag.ShowLogout = true;

            return View(_mapper.Map<VMVideo>(await _videoService.GetById(id)));
        }

    }
}
