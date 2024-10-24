using Microsoft.AspNetCore.SignalR;
using T2305M_API.DTO;
using T2305M_API.DTO.Event;

namespace T2305M_API.Repositories
{
    public interface IEventTicketRepository
    {
        Task<List<String>>CreateEventTickets(int orderId);
    }
}
