using AutoMapper;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using T2305M_API.DTO.Event;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using T2305M_API.Services;
using T2305M_API.DTO.UserEvent;
using T2305M_API.Models;
using T2305M_API.DTO.Notification;
using T2305M_API.Repositories.Implements;
using T2305M_API.Services.Implements;

public class PaypalService : IPaypalService
{
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly T2305mApiContext _context;
    //private readonly IPaypalRepository _paypalRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventTicketRepository _eventTicketRepository;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly EmailService _emailService;



    public PaypalService(IWebHostEnvironment env, IMapper mapper, T2305mApiContext context, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IEventTicketRepository eventTicketRepository, IUserNotificationRepository userNotificationRepository, EmailService emailService)
    {
        _env = env;
        _mapper = mapper;
        _context = context;
        //_paypalRepository = paypalRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _eventTicketRepository = eventTicketRepository;
        _userNotificationRepository = userNotificationRepository;
        _userNotificationRepository = userNotificationRepository;
        _emailService = emailService;
        _emailService = emailService;
    }

    public async Task<PaypalHandleResponse> HandlePaypalInfo(int userId, OrderDTO orderDTO)
    {
        try
        {
            Order newOrder = await _orderRepository.CreateOrder(userId, orderDTO);
            Payment newPayment = await _paymentRepository.CreatePayment(userId, newOrder.OrderId, orderDTO);

            User user =   await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);


            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "EmailTemplate", "PaymentComplete.html");

            var placeholders = new Dictionary<string, string>
{
    { "{title}", "Congratulations! Your Payments are Received!" },
    { "{userName}",  user.FullName},
};

            await _emailService.SendEmailTemplateAsync(
                to: user.Email,
                subject: "Your Payments are Received!",
                templateFilePath: templateFilePath,
                placeholders: placeholders,
                ticketCodes: new List<string>()
            );

            await _userNotificationRepository.CreateNotificationAsync(new CreateBasicNotificationDTO { Message = $"Your payment has been successfully confirmed. Your tickets will be ready soon!" }, userId);
            return new PaypalHandleResponse
            {
                Message = "Successfully create Order, Payment, Tickets"
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UpdatePaymentStatusResonse> ChangePaymentStatusAsync(int paymentId, string newStatus)
    {
        try
        {
            newStatus = newStatus ?? "COMPLETE";
            // Step 1: Retrieve the payment record by ID
            var payment = await _context.Payment.FindAsync(paymentId);

            // Step 2: Check if the payment exists
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            // Step 3: Update the payment status
            payment.Status = newStatus;
            payment.CompletePaymentDate = DateTime.Now;

            // Step 4: Save changes to the database
            await _context.SaveChangesAsync();

            return new UpdatePaymentStatusResonse
            {
                IsSuccess = true,
                PaymentStatus = newStatus,
                Message = "Update Payment Status Successfully",
                UserEmail = payment.User.Email,
                UserId = payment.UserId ?? 0,
                UserName = payment.User.FullName

            };
        }
        catch (Exception ex)
        {
            // Log the exception here (optional)
            throw new ApplicationException("An error occurred while updating the payment status.", ex);
        }
    }

    public async Task<Payment> GetPaymentAsync(int paymentId)
    {
        try
        {
            // Step 1: Retrieve the payment record by ID
            var payment = await _context.Payment.FindAsync(paymentId);

            // Step 2: Check if the payment exists
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            // Return the retrieved payment record
            return payment;
        }
        catch (Exception ex)
        {
            // Log the exception here (optional)
            throw new ApplicationException("An error occurred while retrieving the payment.", ex);
        }
    }

    public async Task<Order> GetOrderAsync(int orderId)
    {
        try
        {
            // Step 1: Retrieve the order record by ID
            var order = await _context.Order.FindAsync(orderId);

            // Step 2: Check if the order exists
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // Return the retrieved order record
            return order;
        }
        catch (Exception ex)
        {
            // Log the exception here (optional)
            throw new ApplicationException("An error occurred while retrieving the order.", ex);
        }
    }

    public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        try
        {
            newStatus = newStatus ?? "COMPLETE";

            // Step 1: Retrieve the order record by ID
            var order = await _context.Order.FindAsync(orderId);

            // Step 2: Check if the order exists
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // Step 3: Retrieve the related payment
            var payment = await _context.Payment.FindAsync(order.Payment.PaymentId);

            // Step 4: Check if the payment exists
            if (payment == null)
            {
                throw new KeyNotFoundException("Related payment not found.");
            }

            // Step 5: Only update to COMPLETE if the related payment is COMPLETE
            if (newStatus == "COMPLETE" && payment.Status != "COMPLETE")
            {
                throw new InvalidOperationException("Cannot mark order as COMPLETE since the related payment is not COMPLETE.");
            }

            // Step 6: Allow cancellation if payment is not COMPLETE
            if (newStatus == "CANCELLED" && payment.Status == "COMPLETE")
            {
                throw new InvalidOperationException("Cannot cancel order as the payment is already COMPLETE.");
            }

            // Step 7: Update the order status
            order.Status = newStatus;

            // Step 8: Save changes to the database
            await _context.SaveChangesAsync();

            List<string> ticketCodes = await CreateEventTicketsAsync(orderId);
            // Step 1: Retrieve the related event by the EventId in the order
            Event relatedEvent = await _context.Event
                .FirstOrDefaultAsync(e => e.EventId == order.EventId);

            if (relatedEvent == null)
            {
                throw new KeyNotFoundException("Related event not found.");
            }

            // Step 2: Increment the CurrentTickets for the event based on the order quantity
            relatedEvent.CurrentTickets += order.Quantity;

            // Step 3: Save changes to the database
            await _context.SaveChangesAsync();
            return new UpdateOrderStatusResponse
            {
                IsSuccess = true,
                OrderStatus = newStatus,
                Message = "Order status updated successfully.",
                TicketCodes = ticketCodes,
                UserEmail = payment.User.Email,
                UserId = payment.UserId ?? 0,
                UserName = payment.User.FullName
            };
        }
        catch (Exception ex)
        {
            // Log the exception here (optional)
            throw;
        }
    }

    public async Task<List<string>> CreateEventTicketsAsync(int orderId)
    {
        try
        {
            var order = await _context.Order.FindAsync(orderId);

            // Step 1: Retrieve the payment record by ID
            var payment = await _context.Payment.FindAsync(order.Payment.PaymentId);

            // Step 2: Check if the payment exists
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            // Return the retrieved payment record
            // Admin duyệt thì mới tạo ticket
            // tạo tickets xong thì nhớ tăng currentTickets lên trong Event table

            return await _eventTicketRepository.CreateEventTickets(orderId);
        }
        catch (Exception ex)
        {
            // Log the exception here (optional)
            throw;
        }
    }





    public async Task<PaginatedResult<GetBasicOrderDTO>> GetBasicOrdersAsync(OrderQueryParameters queryParameters)
    {
        // Step 1: Query the Orders
        IQueryable<Order> query = _context.Order
                                           .Include(o => o.Event); // Include the related Event
                                                                   //.Include(o => o.User)
                                                                   //.Include(o => o.Payment); // Include the related Event

        // Step 2: Apply filtering based on Status
        if (!string.IsNullOrEmpty(queryParameters.Status))
        {
            query = query.Where(o => o.Status == queryParameters.Status);
        }

        // Step 3: Apply filters based on Event properties
        if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
        {
            query = query.Where(o =>
                o.Event.Title.Contains(queryParameters.SearchTerm) ||
                o.Event.Description.Contains(queryParameters.SearchTerm) ||
                o.Event.Organizer.Contains(queryParameters.SearchTerm) ||
                o.Event.Location.Contains(queryParameters.SearchTerm) ||
                o.Event.Address.Contains(queryParameters.SearchTerm));
        }

        // Step 4: Apply sorting
        if (!string.IsNullOrEmpty(queryParameters.SortColumn))
        {
            bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
            switch (queryParameters.SortColumn.ToLower())
            {
                case "title":
                    query = isDescending ? query.OrderByDescending(o => o.Event.Title) : query.OrderBy(o => o.Event.Title);
                    break;
                case "quantity":
                    query = isDescending ? query.OrderByDescending(o => o.Quantity) : query.OrderBy(o => o.Quantity);
                    break;
                default:
                    query = isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);
                    break;
            }
        }

        // Step 5: Pagination
        int totalItems = await query.CountAsync();
        var pagedData = await query
            .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();

        // Step 6: Manually map the result to GetBasicOrderDTO
        var orderDTOs = pagedData.Select(o => new GetBasicOrderDTO
        {
            OrderId = o.OrderId,
            EventId = o.Event.EventId,
            UserName = o.User?.FullName, // Assuming User has a Name property
            EventThumbnail = o.Event?.Thumbnail, // Assuming User has a Name property
            OrderDate = o.OrderDate,
            TicketQuantity = o.Quantity,
            TotalAmount = o.TotalAmount, // Cast to int as per your DTO
            //PaymentId = o.paymentId ?? 0, // Use 0 if paymentId is null
            PaymentMethod = o.Payment?.PaymentMethod, // Assuming payment method is int
            OrderStatus = o.Status, // Assuming status is an int; change accordingly
            EventTitle = o.Event.Title
        }).ToList();

        // Step 7: Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

        // Return the paginated result
        return new PaginatedResult<GetBasicOrderDTO>
        {
            TotalItems = totalItems,
            PageSize = queryParameters.PageSize,
            CurrentPage = queryParameters.Page,
            TotalPages = totalPages,
            HasNextPage = queryParameters.Page < totalPages,
            HasPreviousPage = queryParameters.Page > 1,
            Data = orderDTOs
        };
    }

}