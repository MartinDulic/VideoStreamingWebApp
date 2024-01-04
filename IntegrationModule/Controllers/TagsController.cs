using BL.Model;
using BLL.Services;
using DAL.Requests;
using DAL.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagsController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetTags()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var tags = await _tagService.GetAll();

                return Ok(tags.Select(dalTags =>
                    new TagResponse
                    {
                        Id = dalTags.Id,
                        Name = dalTags.Name
                    }
                ));

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TagResponse>> GetTag(int id)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var dalTag = await _tagService.GetById(id);

                if (dalTag == null) return NotFound();

                return Ok(new TagResponse
                {
                    Id = dalTag.Id,
                    Name = dalTag.Name
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TagResponse>> AddTag(TagRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalTags = await _tagService.GetAll();
                var tag = dalTags.FirstOrDefault(g => g.Name == request.Name);

                if (tag == null)
                {
                    var dalTag = new BLTag
                    {
                        Name = request.Name
                    };

                    await _tagService.Add(dalTag);
                    var newlyAddedTag = await _tagService.GetByName(request.Name);

                    return Ok(new TagResponse
                    {
                        Id = newlyAddedTag.Id,
                        Name = dalTag.Name
                    });
                }
                else return Conflict("The Genre you are trying to add already exists");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<TagResponse>> ModifyTag(int id, [FromBody] TagRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalTag = await _tagService.GetById(id);

                if (dalTag == null)
                    return NotFound();

                dalTag.Name = request.Name;

                await _tagService.Update(dalTag);

                return Ok(new TagResponse
                {
                    Id = dalTag.Id,
                    Name = dalTag.Name
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<TagResponse>> RemoveTag(int id)
        {
            try
            {

                var dalTag = await _tagService.GetById(id);
                if (dalTag == null)
                    return NotFound();

                await _tagService.Delete(id);

                return Ok(new TagResponse
                {
                    Id = dalTag.Id,
                    Name = dalTag.Name
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
