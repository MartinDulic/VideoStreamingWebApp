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
    public class GenresController : Controller
    {
        private readonly GenreService _genreService;

        public GenresController(GenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GenreResponse>>> GetGenres()
        {
            try
            {
                var genres = await _genreService.GetAll();

                return Ok(genres.Select(dalGenre => new GenreResponse
                {
                    Id = dalGenre.Id,
                    Name = dalGenre.Name,
                    Description = dalGenre.Description
                }));

            } catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GenreResponse>> GetGenre(int id)
        {
            try
            {
                var dalGenre = await _genreService.GetById(id);

                if (dalGenre == null)
                {
                    return NotFound();
                }

                return Ok(new GenreResponse
                {
                    Id= dalGenre.Id,
                    Name = dalGenre.Name,
                    Description = dalGenre.Description
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<GenreResponse>> AddGenre(GenreRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalGenres = await _genreService.GetAll();
                var genre = dalGenres.FirstOrDefault(g => g.Name == request.Name);

                if (genre == null)
                {
                    var dbGenre = new BLGenre
                    {
                        Name = request.Name,
                        Description = request.Description
                    };

                    await _genreService.Add(dbGenre);

                    var newlyAddedGenre = await _genreService.GetByName(request.Name);

                    return Ok(new GenreResponse
                    {
                        Id = newlyAddedGenre.Id,
                        Name = dbGenre.Name,
                        Description = dbGenre.Description,

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
        public async Task<ActionResult<GenreResponse>> UpdateGenre(int id, [FromBody] GenreRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalGenre = await _genreService.GetById(id);

                if (dalGenre == null) return NotFound();

                dalGenre.Name = request.Name;
                dalGenre.Description = request.Description;


                await _genreService.Update(dalGenre);

                return Ok(new GenreResponse
                {
                    Id = dalGenre.Id,
                    Name = dalGenre.Name,
                    Description = dalGenre.Description

                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<GenreResponse>> RemoveGenre(int id)
        {
            try
            {
                var dalGenre = await _genreService.GetById(id);
                if (dalGenre == null) return NotFound();

                await _genreService.Delete(id);

                return Ok(new GenreResponse
                {
                    Id = dalGenre.Id,
                    Name = dalGenre.Name,
                    Description = dalGenre.Description

                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
