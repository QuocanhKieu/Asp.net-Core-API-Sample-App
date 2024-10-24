using Microsoft.AspNetCore.Mvc;
using System.Net;
using T2305M_API.DTO.Culture;
using T2305M_API.DTO.Culture;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Services;
using T2305M_API.Services.Implements;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _cultureService;

        public CultureController(ICultureService cultureService)
        {
            _cultureService = cultureService;
        }

        [HttpGet]
        public async Task<ActionResult> GetBasicCultureDTOs([FromQuery] CultureQueryParameters queryParameters)
        {
            try
            {
                var paginatedResult = await _cultureService.GetBasicCultureDTOsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicCultureDTO>>(paginatedResult, "Retrieved paginated basic Cultures successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicCultureDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [HttpGet("{cultureId}")]
        public async Task<ActionResult<GetDetailCultureDTO>> GetDetailCultureDTOById(int cultureId)
        {
            try
            {
                var detailCultureDTO = await _cultureService.GetDetailCultureDTOByIdAsync(cultureId);
                if (detailCultureDTO == null)
                {
                    return NotFound(new APIResponse<GetDetailCultureDTO>(HttpStatusCode.NotFound, "DetailCulture not found.")); // Return 404 if not found
                }
                return Ok(detailCultureDTO); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<GetDetailCultureDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCulture([FromBody] CreateCultureDTO createCultureDTO)
        {
            try
            {
                var createCultureResponse = await _cultureService.CreateCultureAsync(createCultureDTO);

                if (createCultureResponse.CultureId > 0)
                {
                    return Ok(new APIResponse<CreateCultureResponseDTO>(createCultureResponse, createCultureResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<CreateCultureResponseDTO>(
                    HttpStatusCode.Conflict, "Can not create Culture due to internal problems contact the backend."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateCultureResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        //// PUT: api/Article/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutArticle(int id, Article article)
        //{
        //    if (id != article.ArticleId)
        //    {
        //        return BadRequest("ID mismatch");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Entry(article).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ArticleExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// DELETE: api/Article/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteArticle(int id)
        //{
        //    var article = await _context.Article.FindAsync(id);
        //    if (article == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Article.Remove(article);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ArticleExists(int id)
        //{
        //    return _context.Article.Any(e => e.ArticleId == id);
        //}
    }
}
