using Microsoft.AspNetCore.Mvc;
using System.Net;
using T2305M_API.DTO.History;
using T2305M_API.DTO.Search;
using T2305M_API.Models;
using T2305M_API.Services;
using T2305M_API.Services.Implements;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Searchcontroller : ControllerBase
    {
        private readonly SearchServiceImpl _searchservice;
        public Searchcontroller(SearchServiceImpl searchservice)
        {
            _searchservice = searchservice;
        }

        [HttpGet]
        public async Task<ActionResult> get([FromQuery] SearchParameters searchParameters)
        {
            try
            {
                var paginatedResult = await _searchservice.SearchArticlesAsync(searchParameters);
                //return Ok(paginatedResult);
                return Ok(new APIResponse<PaginatedResult<SearchResultDTO>>(paginatedResult, "Retrieved paginated basic Histories and Cultures successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicHistoryDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
    }
}
