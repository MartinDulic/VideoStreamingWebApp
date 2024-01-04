using BL.Model;
using BLL.Services;
using DAL.Requests;
using DAL.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;
        private readonly TagService _tagService;
        private readonly VideoTagService _videoTagService;
        private readonly GenreService _genreService;
        private readonly ImageService _imageService;
        
        public VideoController(VideoService videoService, TagService tagService, VideoTagService videoTagService, ImageService imageService, GenreService genreService)
        {
            _videoService = videoService;
            _tagService = tagService;
            _videoTagService = videoTagService;
            _imageService = imageService;
            _genreService = genreService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideos()
        {
            try
            {
                var allVideos = await _videoService.GetAll();
                var allTags = await _tagService.GetAll();

                return Ok(allVideos.Select(dalVideo =>

                    new VideoResponse
                    {
                        Name = dalVideo.Name,
                        Description = dalVideo.Description,
                        Image = dalVideo.Image.Content,
                        TotalTime = dalVideo.TotalSeconds,
                        StreamingUrl = dalVideo.StreamingUrl,
                        Genre = dalVideo.Genre.Name,
                        Tags = string.Join(",", dalVideo.VideoTags
                                     .Select(vt => vt.Tag)
                                     .Where(tag => tag != null)
                                     .Select(tag => tag!.Name))
                    }

                ));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<VideoResponse>> GetVideo(int id)
        {
            try
            {
                var allVideos = await _videoService.GetAll();
                var dalVideo = allVideos.FirstOrDefault(v => v.Id == id);
                var allTags = _tagService.GetAll();
                if (dalVideo == null) return NotFound();

                return Ok(new VideoResponse
                {
                    Name = dalVideo.Name,
                    Description = dalVideo.Description,
                    Image = dalVideo.Image.Content,
                    TotalTime = dalVideo.TotalSeconds,
                    StreamingUrl = dalVideo.StreamingUrl,
                    Genre = dalVideo.Genre.Name,
                    Tags = string.Join(",", dalVideo.VideoTags
                                     .Select(vt => vt.Tag)
                                     .Where(tag => tag != null)
                                     .Select(tag => tag!.Name))
                });

            } catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<VideoResponse>> AddVideo(VideoRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                string[] tags = request.Tags.Split(',');
                var alldalTags = await _tagService.GetAll();
                var dalTags = alldalTags.Where(x => tags.Contains(x.Name));

                var dalVideo = new BLVideo
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageId = request.ImageId,
                    TotalSeconds = request.TotalTime,
                    StreamingUrl = request.StreamingUrl,
                    GenreId = request.GenreId,
                    VideoTags = dalTags.Select(x => new BLVideoTag { Tag = x }).ToList()
                };

                foreach (var tag in tags)
                {
                    if (!dalTags.Any(t => t.Name == tag))
                    {
                        var newTag = new BLTag { Name = tag };
                        dalVideo.VideoTags.Add(new BLVideoTag { Tag = newTag });
                    }
                }

                await _videoService.Add(dalVideo);

                var responseVideo = new VideoResponse
                {
                    Name = dalVideo.Name,
                    Description = dalVideo.Description,
                    Image = dalVideo.ImageId.ToString(),
                    TotalTime = dalVideo.TotalSeconds,
                    StreamingUrl = dalVideo.StreamingUrl,
                    Genre = dalVideo.GenreId.ToString(),
                    Tags = string.Join(",", dalVideo.VideoTags
                                        .Select(vt => vt.Tag)
                                        .Where(tag => tag != null)
                                        .Select(tag => tag.Name))
                };

                return Ok(responseVideo);
            } catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while processing your request.");
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<BLVideo>>> Search(int page, int size, string searchValue, string orderBy, string orderingDirection)
        {
            try
            {
                var videos = from s in await _videoService.GetAll() select s;

                switch (orderBy)
                {
                    case "id":
                        videos = videos.OrderBy(v => v.Id);
                        break;
                    case "name":
                        videos = videos.OrderBy(v => v.Name);
                        break;
                    case "total time":
                        videos = videos.OrderBy(v => v.TotalSeconds);
                        break;
                    default:
                        videos = videos.OrderBy(v => v.Id);
                        break;
                }

                //Reverse list if order desc
                if (string.Compare(orderingDirection, "desc", true) == 0)
                {
                    videos = videos.Reverse();
                }

                //Filter request 
                if (!string.IsNullOrEmpty(searchValue)) videos = videos.Where(v => v.Name.Contains(searchValue));

                //Paging
                videos = videos.Skip((page - 1) * size).Take(size);

                return Ok(videos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPut("[action]")]
        public async Task<ActionResult<VideoResponse>> UpdateVideo(int id, VideoRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalVideos = await _videoService.GetAll();
                var dalVideo = dalVideos.FirstOrDefault(v => v.Id == id);
                if (dalVideo == null) return NotFound($"Could not find video with id {id}");

                dalVideo.Name = request.Name;
                dalVideo.Description = request.Description;
                dalVideo.GenreId = request.GenreId;
                dalVideo.TotalSeconds = request.TotalTime;
                dalVideo.StreamingUrl = request.StreamingUrl;
                dalVideo.ImageId = request.ImageId;

                var requestTags = request.Tags.Split(',');

                var toRemove = dalVideo.VideoTags.Where(vt => !requestTags.Contains(vt.Tag.Name));
                foreach (var vt in toRemove)
                {
                    await _videoTagService.Delete(vt.Id);
                }

                var existingDalTags = dalVideo.VideoTags.Select(vt => vt.Tag.Name);
                var newTags = requestTags.Except(existingDalTags);
                foreach (var newTag in newTags)
                {
                    var dalTags = await _tagService.GetAll();
                    var dalTag = dalTags.FirstOrDefault(t => newTag == t.Name);
                    if (dalTag == null) continue;

                    dalVideo.VideoTags.Add(new BLVideoTag
                    {
                        Video = dalVideo,
                        Tag = dalTag
                    });
                    await _tagService.SaveData();
                }

                await _videoService.Update(dalVideo);

                var dalGenre = await _genreService.GetById(dalVideo.GenreId);
                var dalImage = await _imageService.GetById(dalVideo.ImageId);

                return Ok(new VideoResponse
                {
                    Name = dalVideo.Name,
                    Description = dalVideo.Description,
                    Image = dalImage.Content,
                    TotalTime = dalVideo.TotalSeconds,
                    StreamingUrl = dalVideo.StreamingUrl,
                    Genre = dalGenre.Name,
                    Tags = string.Join(",", dalVideo.VideoTags
                                   .Select(vt => vt.Tag)
                                   .Where(tag => tag != null)
                                   .Select(tag => tag!.Name))
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while processing your request");
            }
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<VideoResponse>> DeleteVideo(int id)
        {
            try
            {
                var dalVideo = await _videoService.GetById(id);
                if (dalVideo == null) return NotFound();

                string deletedTags = string.Join(",", dalVideo.VideoTags
                                        .Select(vt => vt.Tag)
                                        .Where(tag => tag != null)
                                        .Select(tag => tag!.Name));

                foreach (var videoTag in dalVideo.VideoTags.ToList())
                {
                    await _videoTagService.Delete(videoTag.Id);
                }

                await _videoService.Delete(id);
                
                return Ok(new VideoResponse
                {
                    Name = dalVideo.Name,
                    Description = dalVideo.Description,
                    Image = dalVideo.Image.Content,
                    TotalTime = dalVideo.TotalSeconds,
                    StreamingUrl = dalVideo.StreamingUrl,
                    Genre = dalVideo.Genre.Name,
                    Tags = deletedTags
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
