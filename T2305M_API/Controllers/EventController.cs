using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using T2305M_API.DTO.Book;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.Notification;
using T2305M_API.DTO.User;
using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;
using T2305M_API.Repositories.Implements;
using T2305M_API.Services;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventRepository _eventRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;


        public EventController(IEventService eventService, IUserNotificationRepository userNotificationRepository, IEventRepository eventRepository)
        {
            _eventService = eventService;
            _eventRepository = eventRepository;
            _userNotificationRepository = userNotificationRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetBasicEventDTOs([FromQuery] EventQueryParameters queryParameters)
        {
            try
            {
                var paginatedResult = await _eventService.GetBasicEventDTOsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicEventDTO>>(paginatedResult, "Retrieved paginated basic Events successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicEventDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<GetDetailEventDTO>> GetDetailEventDTOById(int eventId)
        {
            try
            {
                var detailEventDTO = await _eventService.GetDetailEventDTOByIdAsync(eventId);
                if (detailEventDTO == null)
                {
                    return NotFound(new APIResponse<GetDetailEventDTO>(HttpStatusCode.NotFound, "DetailEvent not found.")); // Return 404 if not found
                }
                //return Ok(new APIResponse<GetDetailEventDTO>(detailEventDTO, "Retrieved paginated basic Books successfully."));
                return Ok(detailEventDTO); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<GetDetailEventDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost("create-event")]
        public async Task<IActionResult> CreateEvent([FromForm] IFormFile formFile, [FromForm] string jsonEventData)
        {
            try
            {
                // Deserialize jsonData into CreateEventDTO, excluding the file.
                var createEventDTO = JsonConvert.DeserializeObject<CreateEventDTO>(jsonEventData);

                // Assign the formFile to the deserialized DTO.
                createEventDTO.formFile = formFile;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token or user not authenticated" });
                }

                int userId = int.Parse(userIdClaim);
                createEventDTO.UserId = userId;
                var validationErrors = await _eventService.ValidateCreateEventDTO(createEventDTO);

                if (validationErrors != null)
                {

                    return BadRequest(new APIResponse<Dictionary<string, List<string>>>(HttpStatusCode.BadRequest, "Validation failed", validationErrors));
                }

                var createEventResponse = await _eventService.CreateEventAsync(createEventDTO);
                //await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"You have tried to update the Story with the new Title:" }, userId);

                return Ok(new APIResponse<CreateEventResponseDTO>(createEventResponse, createEventResponse.Message));

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateEventResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("update-event")]
        public async Task<IActionResult> UpdateEvent([FromForm] IFormFile formFile, [FromForm] string jsonEventData)
        {
            try
            {
                // Deserialize jsonData into CreateEventDTO, excluding the file.
                var updateEventDTO = JsonConvert.DeserializeObject<UpdateEventDTO>(jsonEventData);

                // Assign the formFile to the deserialized DTO.
                updateEventDTO.formFile = formFile;
                var validationErrors = await _eventService.ValidateCreateEventDTO(updateEventDTO);

                if (validationErrors != null)
                {
                    return BadRequest(new APIResponse<Dictionary<string, List<string>>>(HttpStatusCode.BadRequest, "Validation failed", validationErrors));
                }

                var updateEventResponse = await _eventService.UpdateEventAsync(updateEventDTO);
                return Ok(new APIResponse<UpdateEventResponseDTO>(updateEventResponse, updateEventResponse.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateEventResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-event/{eventId}")]
        public async Task<IActionResult> DeleteUserArticle( int eventId)
        {
            try
            {
                await _eventRepository.SetEventInactive(eventId);
                return Ok("Delete Event Successfully");

            }
            catch (FileNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new APIResponse<UpdateAvatarResponseDTO>(
                       HttpStatusCode.NotFound, "Not found error: " + ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<UpdateUserResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
    }
}
