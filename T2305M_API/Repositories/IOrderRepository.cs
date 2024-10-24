using T2305M_API.DTO;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(int userId, OrderDTO orderDTO);
    }
}
