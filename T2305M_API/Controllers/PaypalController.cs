using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Security.Claims;
using T2305M_API.CustomException;
using T2305M_API.DTO;
using T2305M_API.DTO.History;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Services;
using T2305M_API.Services.Implements;
using T2305M_API.DTO.UserEvent;
using T2305M_API.DTO.Notification;
using T2305M_API.DTO.UserArticle;
using T2305M_API.Repositories.Implements;
using T2305M_API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace T2305M_API.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly IPaypalService _paypalService;
        private readonly EmailService _emailService;
        private readonly IUserNotificationRepository _userNotificationRepository;


        public PaypalController(IPaypalService paypalService, EmailService emailService, IUserNotificationRepository userNotificationRepository)
        {
            _paypalService = paypalService;
            _emailService = emailService;
            _userNotificationRepository = userNotificationRepository;

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ConfirmPaypalCapture([FromForm] OrderDTO orderDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token or user not authenticated" });
                }

                int userId = int.Parse(userIdClaim);

                var paypalResponse = await _paypalService.HandlePaypalInfo(userId, orderDTO);
                if (paypalResponse == null)
                {
                    return NotFound(new APIResponse<String>(HttpStatusCode.NotFound, "DetailPaypal not found."));
                }
                return Ok(paypalResponse); // Return the DTO
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<String>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-orders")]
        public async Task<ActionResult> GetBasicUserEventDTOs([FromQuery] OrderQueryParameters orderQueryParameters)
        {
            try
            {
                var paginatedResult = await _paypalService.GetBasicOrdersAsync(orderQueryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicOrderDTO>>(paginatedResult, "Retrieved paginated basic Events successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<String>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update-payment-status/{paymentId}")]
        public async Task<IActionResult> ChangePaymentStatus(int paymentId)
        {
            try
            {
                UpdatePaymentStatusDTO updatePaymentStatusDTO = new UpdatePaymentStatusDTO
                {
                    PaymentId = paymentId,
                    Status = "COMPLETE"
                };

                var updatePaymentStatusResponse = await _paypalService.ChangePaymentStatusAsync(updatePaymentStatusDTO.PaymentId, updatePaymentStatusDTO?.Status);

                if (updatePaymentStatusResponse.IsSuccess)
                {

                    string subject_admin = "Notification Email";
                    string body_admin = $"Update Payment Successfully";
                    await _emailService.SendEmailAsync("anhkqth2304001@fpt.edu.vn", subject_admin, body_admin);


                    string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "EmailTemplate", "PaymentComplete.html");

                    var placeholders = new Dictionary<string, string>
{
    { "{title}", "Congratulations! Your Payments are Received!" },
    { "{userName}",  updatePaymentStatusResponse.UserName},
};

                    await _emailService.SendEmailTemplateAsync(
                        to: updatePaymentStatusResponse.UserEmail,
                        subject: "Your Payments are Received!",
                        templateFilePath: templateFilePath,
                        placeholders: placeholders,
                        ticketCodes: new List<string>()
                    );

                    await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"Your payment has been successfully confirmed. Your tickets will be ready soon!" }, updatePaymentStatusResponse.UserId);


                    return Ok(new APIResponse<string>("Successfully Update the Payment Status", updatePaymentStatusResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<string>(
                    HttpStatusCode.Conflict, "Can not Update the payment status, contact the backend."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-payment-status/{paymentId}")]
        public async Task<IActionResult> GetPaymentStatus(int paymentId)
        {
            try
            {
                Payment payment = await _paypalService.GetPaymentAsync(paymentId);
                return Ok(new APIResponse<object>(new { paymentStatus = payment.Status }, "Get Payment Status Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<CreateUserArticleResponseDTO>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update-order-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            try
            {
                UpdateOrderStatusDTO updateOrderStatusDTO = new UpdateOrderStatusDTO
                {
                    OrderId = orderId,
                    Status = "COMPLETE"
                };
                var updateOrderStatusResponse = await _paypalService.UpdateOrderStatusAsync(updateOrderStatusDTO.OrderId, updateOrderStatusDTO?.Status);

                if (updateOrderStatusResponse.IsSuccess)
                {
                    // Email to admin
                    string subjectAdmin = "Notification Email";
                    string bodyAdmin = "Update Order Successfully";
                    await _emailService.SendEmailAsync("anhkqth2304001@fpt.edu.vn", subjectAdmin, bodyAdmin);

                    string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "EmailTemplate", "SendTicketsOrderComplete.html");

                    var placeholders = new Dictionary<string, string>
                    {
                        { "{title}", "Congratulations! Your order is complete. Check out your tickets. " },
                        { "{userName}",  updateOrderStatusResponse.UserName},
                    };

                    // List of dynamic ticket codes
                    var ticketCodes = updateOrderStatusResponse.TicketCodes;

                    // Send email
                    await _emailService.SendEmailTemplateAsync(
                        to: updateOrderStatusResponse.UserEmail,
                        subject: "Your Event Tickets Are Ready!",
                        templateFilePath: templateFilePath,
                        placeholders: placeholders,
                        ticketCodes: ticketCodes
                    );

                    // Create user notification
                    await _userNotificationRepository.CreateNotificationAsync(
                        new CreateBasicNotificationDTO { Message = $"Your order has been successfully confirmed. Ticket codes: {ticketCodes}" },
                        updateOrderStatusResponse.UserId);

                    return Ok(new APIResponse<string>("Successfully updated the payment status", updateOrderStatusResponse.Message));
                }

                return StatusCode((int)HttpStatusCode.Conflict, new APIResponse<string>(
                    HttpStatusCode.Conflict, "Cannot update the payment status, please contact support."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<string>(
                    HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }

    }
}
