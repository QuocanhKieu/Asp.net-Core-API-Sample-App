using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.User;
using T2305M_API.DTO.UserEvent;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;
using T2305M_API.Services;

namespace T2305M_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventController : ControllerBase
    {
        private readonly IUserEventRepository _iUserEventRepository;
        public UserEventController(IUserEventRepository iUserEventRepository)
        {
            _iUserEventRepository = iUserEventRepository;
        }

        //// POST: api/UserUserEvents/bookmark
        //[HttpPost("bookmark/{eventId}")]
        //public async Task<IActionResult> BookmarkUserEvent(int eventId)
        //{
        //    try
        //    {
        //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if (userIdClaim == null)
        //        {
        //            return Unauthorized(new { message = "Invalid token or user not authenticated" });
        //        }


        //        int userId = int.Parse(userIdClaim);


        //        var bookmarkResponseDTO = await _iUserEventRepository.CreateBookmark(userId, eventId);

        //        if (bookmarkResponseDTO.EventId > 0)
        //        {
        //            return Ok(new APIResponse<UserEventResponseDTO>(bookmarkResponseDTO, bookmarkResponseDTO.Message));
        //        }

        //        return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<UserEventResponseDTO>(
        //            HttpStatusCode.Conflict, "Can not create UserEvent due to internal problems contact the backend."));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return StatusCode((int)HttpStatusCode.BadRequest, new APIResponse<UserEventResponseDTO>(
        //            HttpStatusCode.BadRequest, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<UserEventResponseDTO>(
        //            HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
        //    }
        //}

        //// DELETE: api/UserUserEvents/unbookmark
        //[HttpDelete("unbookmark/{eventId}")]
        //public async Task<IActionResult> UnbookmarkUserEvent( int eventId)
        //{
        //    {
        //        try
        //        {
        //            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //            if (userIdClaim == null)
        //            {
        //                return Unauthorized(new { message = "Invalid token or user not authenticated" });
        //            }


        //            int userId = int.Parse(userIdClaim);

        //            var bookmarkResponseDTO = await _iUserEventRepository.RemoveBookmark( userId,  eventId);

        //            if (bookmarkResponseDTO.EventId > 0)
        //            {
        //                return Ok(new APIResponse<UserEventResponseDTO>(bookmarkResponseDTO, bookmarkResponseDTO.Message));
        //            }

        //            return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<UserEventResponseDTO>(
        //                HttpStatusCode.Conflict, "Can not delete UserEvent due to internal problems contact the backend."));
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<UserEventResponseDTO>(
        //                HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
        //        }
        //    }
        //}

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetBasicUserEventDTOs([FromQuery] UserEventQueryParameters queryParameters)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token or user not authenticated" });
                }


                int userId = int.Parse(userIdClaim);


                var paginatedResult = await _iUserEventRepository.GetBasicUserEventDTOsAsync(userId, queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetUserEventDTO>>(paginatedResult, "Retrieved paginated basic Events successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<String>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize]
        [HttpGet("getDetailOrderInfo/{orderId}")]
        public async Task<ActionResult> GetDetailUserEventDTO(int orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token or user not authenticated" });
                }


                int userId = int.Parse(userIdClaim);

                GetDetailUserEventDTO getDetailUserEventDTO = await _iUserEventRepository.GetDetailUserEventDTO(userId, orderId);
                if (getDetailUserEventDTO == null)
                {
                    return NotFound(new APIResponse<String>(HttpStatusCode.NotFound, "not found.")); // Return 404 if not found
                }
                //return Ok(new APIResponse<GetDetailEventDTO>(detailEventDTO, "Retrieved paginated basic Books successfully."));
                return Ok(getDetailUserEventDTO); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<String>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }



        }
    }
}

