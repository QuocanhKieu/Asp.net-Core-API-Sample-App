using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using AutoMapper;
using T2305M_API.DTO.Event;


public class PaymentRepository : IPaymentRepository
{
    private readonly T2305mApiContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public PaymentRepository(T2305mApiContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }
    public async Task<Payment> CreatePayment(int userId, int orderId, OrderDTO orderDTO)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                Payment newPayment = _mapper.Map<Payment>(orderDTO);
                newPayment.UserId = userId;
                newPayment.OrderId = orderId;
                _context.Payment.Add(newPayment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newPayment;
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