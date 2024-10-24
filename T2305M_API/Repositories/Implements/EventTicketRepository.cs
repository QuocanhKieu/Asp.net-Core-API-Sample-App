using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using AutoMapper;
using T2305M_API.DTO.Event;


public class EventTicketRepository : IEventTicketRepository
{
    private readonly T2305mApiContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public  EventTicketRepository(T2305mApiContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }
    public async Task<List<string>> CreateEventTickets(int orderId)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var order = await _context.Order.FindAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException("Order not found.");
                }

                // Create a list to store the event ticket codes
                List<string> eventTicketCodes = new List<string>();

                // Loop based on the quantity specified in the order
                for (int i = 0; i < order.Quantity; i++)
                {
                    EventTicket newEventTicket = new EventTicket
                    {
                        UserId = order.UserId,
                        OrderId = orderId,
                        EventId = order.EventId,
                        PaymentId = order.Payment.PaymentId,
                        TicketPrice = order.Event?.TicketPrice ?? 0.0m, // Use nullable decimal type
                    };

                    // Add the new ticket to the context (but not yet saved)
                    await _context.EventTicket.AddAsync(newEventTicket);

                    // Add the generated ticket code to the list
                    eventTicketCodes.Add(newEventTicket.EventTicketCode);
                }

                // Save all event tickets to the database
                await _context.SaveChangesAsync();

                // Commit the transaction after all tickets are successfully created
                await transaction.CommitAsync();

                return eventTicketCodes; // Return the list of ticket codes
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync(); // Rollback the transaction in case of error
                throw dbEx;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback the transaction for other exceptions
                throw ex;
            }
        }
    }

}