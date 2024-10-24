using Microsoft.AspNetCore.Mvc;
using System.Net;
using T2305M_API.DTO.History;
using T2305M_API.Models;
using T2305M_API.Services;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }
        [HttpGet]
        public async Task<ActionResult> GetBasicHistoryDTOs([FromQuery] HistoryQueryParameters queryParameters)
        {
            try
            {
                var paginatedResult = await _historyService.GetBasicHistoryDTOsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicHistoryDTO>>(paginatedResult, "Retrieved paginated basic Histories successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicHistoryDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [HttpGet("{historyId}")]
        public async Task<ActionResult<GetDetailHistoryDTO>> GetDetailHistoryDTOById(int historyId)
        {
            try
            {
                var detailHistoryDTO = await _historyService.GetDetailHistoryDTOByIdAsync(historyId);
                if (detailHistoryDTO == null)
                {
                    return NotFound(new APIResponse<GetDetailHistoryDTO>(HttpStatusCode.NotFound, "DetailHistory not found.")); // Return 404 if not found
                }
                return Ok(detailHistoryDTO); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<GetDetailHistoryDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateHistory([FromBody] CreateHistoryDTO createHistoryDTO)
        {
            var validationErrors = await _historyService.ValidateCreateHistoryDTO(createHistoryDTO);

            if (validationErrors != null)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                    HttpStatusCode.BadRequest, "Validation failed", validationErrors));
            }
            try
            {
                var createHistoryResponse = await _historyService.CreateHistoryAsync(createHistoryDTO);

                if (createHistoryResponse.HistoryId > 0)
                {
                    return Ok(new APIResponse<CreateHistoryResponseDTO>(createHistoryResponse, createHistoryResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<CreateHistoryResponseDTO>(
                    HttpStatusCode.Conflict, "Can not create History due to internal problems contact the backend."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateHistoryResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

    }
}
