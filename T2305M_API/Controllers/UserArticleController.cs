using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using T2305M_API.CustomException;
using T2305M_API.DTO.User;
using T2305M_API.DTO.UserArticle;
using T2305M_API.DTO.UserEvent;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;
using T2305M_API.Services;
using T2305M_API.Services.Implements;
using static System.Net.WebRequestMethods;
using T2305M_API.DTO.Notification;
using T2305M_API.DTO;

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserArticleController : ControllerBase
    {
        private readonly IUserArticleService _userArticleService;
        private readonly IUserArticleRepository _userArticleRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly EmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;

        public UserArticleController(IUserArticleService userArticleService, IUserArticleRepository userArticleRepository, IAuthorizationService authorizationService, EmailService emailService, IUserRepository userRepository, IUserNotificationRepository userNotificationRepository)
        {
            _userArticleService = userArticleService;
            _userArticleRepository = userArticleRepository;
            _authorizationService = authorizationService;
            _emailService = emailService;
            _userRepository = userRepository;
            _userNotificationRepository = userNotificationRepository;

        }
        [HttpGet]
        public async Task<ActionResult> GetBasicUserArticleDTOs([FromQuery] UserArticleQueryParameters queryParameters)
        {
            if (queryParameters.invalidIds?.Any() ?? false)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                                    HttpStatusCode.BadRequest, "Invalid tag IDs provided."));
            }
            try
            {
                var paginatedResult = await _userArticleService.GetBasicUserArticleDTOsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicUserArticleDTO>>(paginatedResult, "Retrieved paginated basic Histories successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicUserArticleDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [HttpGet("{userArticleId}")]
        public async Task<ActionResult<GetDetailUserArticleDTO>> GetDetailUserArticleDTOById(int userArticleId)
        {
            try
            {
                var detailUserArticleDTO = await _userArticleService.GetDetailUserArticleDTOByIdAsync(userArticleId);
                if (detailUserArticleDTO == null)
                {
                    return NotFound(new APIResponse<GetDetailUserArticleDTO>(HttpStatusCode.NotFound, "DetailUserArticle not found.")); // Return 404 if not found
                }
                return Ok(detailUserArticleDTO); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<GetDetailUserArticleDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUserArticle([FromForm] CreateUserArticleDTO createUserArticleDTO)
        {
            //User global object will be available once user logged in
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token or user not authenticated" });
            }

            int userId = int.Parse(userIdClaim);

            var validationImageErrors = await _userArticleService.ValidateImageFile(createUserArticleDTO.file);

            var validationErrors = await _userArticleService.ValidateCreateUserArticleDTO(createUserArticleDTO);

            if (validationErrors != null)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                    HttpStatusCode.BadRequest, "Validation failed", validationErrors));
            }
            if (validationImageErrors != null)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                    HttpStatusCode.BadRequest, "Validation failed", validationImageErrors));
            }
            try
            {
                var createUserArticleResponse = await _userArticleService.CreateUserArticleAsync(userId, createUserArticleDTO, createUserArticleDTO.file);

                if (createUserArticleResponse.UserArticleId > 0)
                {
                    //string subject = "Notification Email";
                    //string body = $"<h1>New Article Submitted</h1><p>The Article  with Id: '{createUserArticleResponse.UserArticleId}' has been submitted for approval.</p><p>Thank you for contributing your story to our site, Have a great day!</p>";
                    //if (!string.IsNullOrEmpty(userEmailClaim))
                    //{
                    //    await _emailService.SendEmailAsync(userEmailClaim, subject, body);
                    //}
                    //else throw new Exception("userEmailClaim is empty.");

                    string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "EmailTemplate", "UserArticleSubmit.html");

                    var placeholders = new Dictionary<string, string>
                    {
                        { "{title}", "Congratulations! We received you Story, will take a review, and approve soon!" },
                        { "{userName}", userEmailClaim},
                    };

                    // List of dynamic ticket codes

                    // Send email
                    await _emailService.SendEmailTemplateAsync(
                        to: userEmailClaim,
                        subject: "Your Story has been submitted! Thank you!",
                        templateFilePath: templateFilePath,
                        placeholders: placeholders,
                        ticketCodes: new List<string>()
                    );


                    string subject_admin = "Notification Email";
                    string body_admin = $"<h1>New Article Submitted</h1><p>The Article  with Id: '{createUserArticleResponse.UserArticleId}' has been submitted for approval.</p><p><a href='{"adminReviewUrl"}'>Review the article here</a></p>";

                    await _emailService.SendEmailAsync("anhkqth2304001@fpt.edu.vn", subject_admin, body_admin);



                    await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"Thank you for submiting new Story with the Title: \"{createUserArticleDTO.Title}\", We'll review and let you know when it's approved!" }, userId);

                    return Ok(new APIResponse<CreateUserArticleResponseDTO>(createUserArticleResponse, createUserArticleResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.Conflict, "Can not create UserArticle due to internal problems contact the backend."));
            }
            catch (SmtpException ex)
            {
                //_logger.LogError(ex, "Error sending notification email.");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new APIResponse<string>(HttpStatusCode.InternalServerError, "Failed to send email notification"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUserArticle([FromForm] UpdateUserArticleDTO updateUserArticleDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token or user not authenticated" });
            }

            int userId = int.Parse(userIdClaim);

            var userArticle = await _userArticleRepository.GetUserArticleByIdAsync(updateUserArticleDTO.UserArticleId);
            if (userArticle == null)
            {
                return NotFound(new APIResponse<Object>(
                    HttpStatusCode.NotFound, "userArticle not found"));
            }

            // Check if the user is authorized to edit the post
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userArticle, "UpdateUserArticlePolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var validationImageErrors = await _userArticleService.ValidateImageFile(updateUserArticleDTO.file);

            var validationErrors = await _userArticleService.ValidateCreateUserArticleDTO(updateUserArticleDTO);

            if (validationErrors != null)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                    HttpStatusCode.BadRequest, "Validation failed", validationErrors));
            }
            if (validationImageErrors != null)
            {
                return BadRequest(new APIResponse<Dictionary<string, List<string>>>(
                    HttpStatusCode.BadRequest, "Validation failed", validationImageErrors));
            }
            try
            {
                var createUserArticleResponse = await _userArticleService.UpdateUserArticleAsync(updateUserArticleDTO, updateUserArticleDTO.file);

                if (createUserArticleResponse.UserArticleId > 0)
                {

                    string subject_admin = "Notification Email";
                    string body_admin = $"<h1>The Article </h1><p>Article '{userArticle.Title}' with Id: '{userArticle.UserArticleId}' has been updated for approval.</p><p><a href='{"adminReviewUrl"}'>Review the article here</a></p>";

                    await _emailService.SendEmailAsync("anhkqth2304001@fpt.edu.vn", subject_admin, body_admin);

                    string subject = "Notification Email";
                    string body = $"<h1>New Article Submitted</h1><p>The Article  with Id: '{createUserArticleResponse.UserArticleId}' has been updated for approval.</p><p>Thank you, Have a great day!</p>";
                    if (!string.IsNullOrEmpty(userEmailClaim))
                    {
                        await _emailService.SendEmailAsync(userEmailClaim, subject, body);
                    }
                    else throw new Exception("userEmailClaim is empty.");


                    await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"You have tried to update the Story with the new Title: \"{updateUserArticleDTO.Title}\", We'll review and let you know when it's approved!" }, userId);


                    return Ok(new APIResponse<CreateUserArticleResponseDTO>(createUserArticleResponse, createUserArticleResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.Conflict, "Can not create UserArticle due to internal problems contact the backend."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("delete/{userArticleId}")]
        public async Task<IActionResult> DeleteUserArticle(int userArticleId)
        {
            var userArticle = await _userArticleRepository.GetUserArticleByIdAsync(userArticleId);
            if (userArticle == null)
            {
                return NotFound(new APIResponse<Object>(
                    HttpStatusCode.NotFound, "userArticle not found"));

            }

            // Check if the user is authorized to edit the post
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userArticle, "UpdateUserArticlePolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            try
            {
                var setUserArticleInactiveRsponseDTO = await _userArticleRepository.SetUserArticleInactive(userArticle);

                return Ok(new APIResponse<ChangeUserArticleStatusResponseDTO>(setUserArticleInactiveRsponseDTO, setUserArticleInactiveRsponseDTO.Message));

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
        [Authorize(Roles = "ADMIN")]
        [HttpPost("restore/{userArticleId}")]
        public async Task<IActionResult> RestoreUserArticle(int userArticleId)
        {
            var userArticle = await _userArticleRepository.GetUserArticleByIdAsync(userArticleId);
            if (userArticle == null)
            {
                return NotFound(new APIResponse<Object>(
                    HttpStatusCode.NotFound, "userArticle not found"));

            }
            try
            {
                var setUserArticleInactiveRsponseDTO = await _userArticleRepository.SetUserArticleActive(userArticle);

                return Ok(new APIResponse<ChangeUserArticleStatusResponseDTO>(setUserArticleInactiveRsponseDTO, setUserArticleInactiveRsponseDTO.Message));

            }
            catch (FileNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new APIResponse<UpdateAvatarResponseDTO>(
                       HttpStatusCode.NotFound, "Not found error: " + ex.Message));
            }
            catch (SmtpException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new APIResponse<string>(HttpStatusCode.InternalServerError, "Failed to send email notification"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<UpdateUserResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost("approve/{userArticleId}")]
        public async Task<IActionResult> ApproveUserArticle(int userArticleId)
        {
            var userArticle = await _userArticleRepository.GetUserArticleByIdAsync(userArticleId);
            if (userArticle == null)
            {
                return NotFound(new APIResponse<Object>(
                    HttpStatusCode.NotFound, "userArticle not found"));
            }
            try
            {
                var setUserArticleInactiveRsponseDTO = await _userArticleRepository.SetUserArticleApproved(userArticle);
                ///need adjustment
                //var user = _userRepository.GetUserByIdAsync(userArticleId);

                if (string.IsNullOrEmpty(userArticle.User.Email))
                {
                    return BadRequest(new APIResponse<string>(HttpStatusCode.BadRequest, "User email is missing"));
                }

                string subject = "Notification Email";
                string body = $"<h1>Hello!</h1><p>Your Story with the Title: \"{userArticle.Title}\" has been approved. Congratulation !</p>";

                await _emailService.SendEmailAsync(userArticle.User.Email, subject, body);
                await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"Your Story with the Title: \"{userArticle.Title}\" has been approved. Congratulation !" }, userArticle.User.UserId);

                return Ok(new APIResponse<ChangeUserArticleStatusResponseDTO>(setUserArticleInactiveRsponseDTO, setUserArticleInactiveRsponseDTO.Message));

            }
            catch (FileNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new APIResponse<ChangeUserArticleStatusResponseDTO>(
                       HttpStatusCode.NotFound, "Not found error: " + ex.Message));
            }
            catch (SmtpException ex)
            {
                //_logger.LogError(ex, "Error sending notification email.");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new APIResponse<string>(HttpStatusCode.InternalServerError, "Failed to send email notification"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<ChangeUserArticleStatusResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

    }
}

