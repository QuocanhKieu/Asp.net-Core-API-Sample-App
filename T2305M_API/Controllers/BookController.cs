using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using T2305M_API.DTO.Book;
using T2305M_API.Models;
using T2305M_API.Services;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult> GetBasicBookDTOs([FromQuery] BookQueryParameters queryParameters)
        {
            try
            {
                var paginatedResult = await _bookService.GetBasicBookDTOsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicBookDTO>>(paginatedResult, "Retrieved paginated basic Books successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicBookDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDTO createBookDTO)
        {
            try
            {
                var createBookResponse = await _bookService.CreateBookAsync(createBookDTO);

                if (createBookResponse.BookId > 0)
                {
                    return Ok(new APIResponse<CreateBookResponseDTO>(createBookResponse, createBookResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<CreateBookResponseDTO>(
                    HttpStatusCode.Conflict, "Can not create Book due to internal problems contact the backend."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateBookResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
    }
}

