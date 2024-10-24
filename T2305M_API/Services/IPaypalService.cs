using T2305M_API.DTO.Event;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IPaypalService
    {
        Task<PaypalHandleResponse> HandlePaypalInfo(int userId, OrderDTO orderDTO);
        Task<UpdatePaymentStatusResonse> ChangePaymentStatusAsync(int paymentId, string newStatus = "COMPLETE");
        Task<Payment> GetPaymentAsync(int paymentId);
        Task<Order> GetOrderAsync(int orderId);
        Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(int orderId, string newStatus = "COMPLETE");
        Task<List<string>> CreateEventTicketsAsync(int orderId);
        Task<PaginatedResult<GetBasicOrderDTO>> GetBasicOrdersAsync(OrderQueryParameters queryParameters);
    }
}
