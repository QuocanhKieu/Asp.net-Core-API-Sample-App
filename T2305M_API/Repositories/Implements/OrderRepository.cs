using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using AutoMapper;
using T2305M_API.DTO.Event;


public class OrderRepository : IOrderRepository
{
    private readonly T2305mApiContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public OrderRepository(T2305mApiContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }
    public async Task<Order> CreateOrder(int userId, OrderDTO orderDTO)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                Order newOrder = _mapper.Map<Order>(orderDTO);
                newOrder.UserId = userId;
                _context.Order.Add(newOrder);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newOrder;
            }
            catch (DbUpdateException dbEx) // Catch database-specific errors
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                throw dbEx;

            }
            catch (Exception ex) // Catch other exceptions
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                throw ex;
            }
        }
    }
    





}