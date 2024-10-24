using T2305M_API.DTO;
using T2305M_API.DTO.Event;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePayment(int userId, int orderId, OrderDTO orderDTO);

    }
}
