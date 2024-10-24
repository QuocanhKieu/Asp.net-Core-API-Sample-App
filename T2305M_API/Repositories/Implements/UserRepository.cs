using Microsoft.EntityFrameworkCore;
using T2305M_API.CustomException;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.User;
using T2305M_API.DTO.User;
using T2305M_API.Entities;

namespace T2305M_API.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly T2305mApiContext _context;

        public UserRepository(T2305mApiContext context, IWebHostEnvironment env)
        {
            _context = context;
        }
        public async Task<User> GetUserByIdAsync(int userId,  bool includeUserEvents = false, bool includeUserArticles = false)
        {
            IQueryable<User> query = _context.User.AsQueryable();

            // Include UserEvents if specified
            if (includeUserEvents)
            {
                query = query.Include(u => u.UserEvents)
                             .ThenInclude(ue => ue.Event); // Include related Event if needed
                             
            }

            // Include UserArticles if specified
            if (includeUserArticles)
            {
                query = query.Include(u => u.UserArticles);
                            
            }

            // Fetch the user by ID
            return await query.FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<UpdateUserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Find the existing user by the UserId
                    var existingUser = await _context.User.FindAsync(userId);

                    if (existingUser == null)
                    {
                        return new UpdateUserResponseDTO
                        {
                            Message = "User not found"
                            // maybe need statusCode property for controller stausCode Return
                        };
                    }

                    // Update user properties
                    existingUser.FullName = updateUserDTO.FullName;
                    existingUser.Education = updateUserDTO.Education;
                    existingUser.ShortBiography = updateUserDTO.ShortBiography;
                    existingUser.LongBiography = updateUserDTO.LongBiography;
                    existingUser.Age = updateUserDTO.Age;
                    existingUser.Facebook = updateUserDTO.Facebook;
                    existingUser.LinkedIn = updateUserDTO.LinkedIn;
                    existingUser.Twitter = updateUserDTO.Twitter;
                    existingUser.PersonalWebsiteUrl = updateUserDTO.PersonalWebsiteUrl;
                    existingUser.PersonalWebsiteTitle = updateUserDTO.PersonalWebsiteTitle;
                    existingUser.ReceiveNotifications = updateUserDTO.ReceiveNotifications;
                    existingUser.UpdatedAt = DateTime.Now;

                    // Attach the original RowVersion to detect concurrency conflicts
                    //_context.Entry(existingUser).OriginalValues["RowVersion"] = updateUserDTO.RowVersion;

                    // Mark the user entity as modified
                    _context.Entry(existingUser).State = EntityState.Modified;

                    // Save the changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction if everything is successful
                    await transaction.CommitAsync();

                    // Return the success response
                    return new UpdateUserResponseDTO
                    {
                        UserId = existingUser.UserId,
                        IsActive = existingUser.IsActive,
                        Message = "User updated successfully"
                    };
                }
                catch (DbUpdateConcurrencyException dbCEx)
                {
                    // Rollback the transaction in case of concurrency exception
                    await transaction.RollbackAsync();
                    return new UpdateUserResponseDTO
                    {
                        Message = "Concurrency conflict detected: " + dbCEx.Message,
                    };
                }
                catch (DbUpdateException dbEx) // Catch database-specific errors
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    return new UpdateUserResponseDTO
                    {
                        Message = "Database update failed during event creation: " + dbEx.Message,
                    };
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of any other exceptions
                    await transaction.RollbackAsync();
                    throw new UserUpdateException(
                        new UpdateUserResponseDTO { Message = "Error occurred while updating the user" },
                        "Update failed",
                        ex
                    );
                    //return new UpdateUserResponseDTO
                    //{
                    //    Message = "Error occurred while updating the user: " + ex.Message,
                    //};
                }
            }
        }
        public async Task UpdateUserImageAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }
    }

}
