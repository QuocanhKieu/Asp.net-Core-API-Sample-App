using Microsoft.EntityFrameworkCore;
using T2305M_API.DTO.UserArticle;
using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;

namespace T2305M_API.Repositories.Implements
{
    public class UserArticleRepository : IUserArticleRepository
    {
        private readonly T2305mApiContext _context;

        public UserArticleRepository(T2305mApiContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<UserArticle> Data, int TotalItems)> GetUserArticlesAsync(UserArticleQueryParameters queryParameters)
        {
            IQueryable<UserArticle> query = _context.UserArticle;

            // Apply filters
            if (queryParameters.UserId > 0)
            {
                query = query.Where(a => a.UserId == queryParameters.UserId);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(a =>
                    a.Title.Contains(queryParameters.SearchTerm) ||
                    a.Description.Contains(queryParameters.SearchTerm) ||
                    a.Content.Contains(queryParameters.SearchTerm) ||
                    (a.User != null && a.User.FullName.Contains(queryParameters.SearchTerm))

                );
            }

            ////strictly only return UserArticle having the same tags id and the same number of tags as the tagIdList input by user
            //if (queryParameters.tagIdList != null && queryParameters.tagIdList.Count > 0)
            //{
            //    query = query.Where(a =>
            //        a.userArticleUserArticleTags.Count(e => queryParameters.tagIdList.Contains(e.UserArticleTagId)) == queryParameters.tagIdList.Count
            //        && a.userArticleUserArticleTags.Count() == queryParameters.tagIdList.Count);
            //}

            //// the UserArticle must include every tags from the tagIdList input by user but can have additonal UserArticleTagId 
            //if (queryParameters.tagIdList != null && queryParameters.tagIdList.Count > 0)
            //{
            //    query = query.Where(a =>
            //        a.userArticleUserArticleTags.Count(e => queryParameters.tagIdList.Contains(e.UserArticleTagId)) == queryParameters.tagIdList.Count);
            //}
            // the UserArticle must include every tags from the tagIdList input by user but can have additonal UserArticleTagId 
            // the same as above
            if (queryParameters.tagIdList != null && queryParameters.tagIdList.Count > 0)
            {
                query = query.Where(a =>
                    queryParameters.tagIdList.All(tagId =>
                        a.userArticleUserArticleTags.Any(e => e.UserArticleTagId == tagId)
                    ));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParameters.SortColumn))
            {
                bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
                switch (queryParameters.SortColumn.ToLower().Trim())
                {
                    case "title":
                        query = isDescending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title);
                        break;
                    case "dislikecount":
                        query = isDescending ? query.OrderByDescending(a => a.DislikeCount) : query.OrderBy(a => a.DislikeCount);
                        break;
                    case "likecount":
                        query = isDescending ? query.OrderByDescending(a => a.LikeCount) : query.OrderBy(a => a.LikeCount);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt);
                        break;
                }
            }

            // Pagination
            int totalItems = await query.CountAsync();
            var pagedData = await query
                .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return (pagedData, totalItems);
        }
        public async Task<UserArticle> GetUserArticleByIdAsync(int userArticleId)
        {
            //return await _context.UserArticle
            //    .FirstOrDefaultAsync(aa => aa.UserArticleId == userArticleId);
            return   await _context.UserArticle
                                .Include(ua => ua.User)
                                .FirstOrDefaultAsync(ua => ua.UserArticleId == userArticleId);
        }
        public async Task<CreateUserArticleResponseDTO> CreateUserArticleAsync(int userId, CreateUserArticleDTO createUserArticleDTO, string newPath)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var newUserArticle = new UserArticle
                    {
                        Title = createUserArticleDTO.Title,
                        Description = createUserArticleDTO.Description,
                        Content = createUserArticleDTO.Content,
                        ThumbnailImage = newPath ?? "/uploads/images/userArticleThumbnails/default-article-thumbnail.jpg",
                        UserId = userId, // Assuming UserId comes from DTO
                        CreatedAt = DateTime.Now,
                        Status = "PENDING",

                    };
                    _context.UserArticle.Add(newUserArticle);
                    await _context.SaveChangesAsync(); // Save the userArticle first to generate UserArticleId

                    if (createUserArticleDTO.UserArticleTagIds != null && createUserArticleDTO.UserArticleTagIds.Any(tagId => tagId != 0))
                    {
                        foreach (var tagId in createUserArticleDTO.UserArticleTagIds)
                        {
                            var userArticleUserArticleTag = new UserArticleUserArticleTag
                            {
                                UserArticleId = newUserArticle.UserArticleId, // Reference the newly created userArticle
                                UserArticleTagId = tagId
                            };
                            _context.UserArticleUserArticleTag.Add(userArticleUserArticleTag); // Add each UserArticleTag
                        }
                        await _context.SaveChangesAsync(); // Save all UserArticleTags
                    }
                    await transaction.CommitAsync();

                    return new CreateUserArticleResponseDTO
                    {
                        UserArticleId = newUserArticle.UserArticleId,
                        Status = newUserArticle.Status,
                        Message = "UserArticle created successfully"
                    };
                }
                catch (DbUpdateException dbEx) // Catch database-specific errors
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    return new CreateUserArticleResponseDTO
                    {
                        Message = "Database update failed during userArticle creation: " + dbEx.Message,
                    };
                }
                catch (Exception ex) // Catch other exceptions
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    throw ex; // Rethrow the exception after rollback
                }
            }
        }
        public async Task<UserArticle> UpdateUserArticleAsync(UpdateUserArticleDTO updateUserArticleDTO)
        {
            var existingUserArticle = await GetUserArticleByIdAsync(updateUserArticleDTO.UserArticleId);
            if (existingUserArticle == null)
            {
                return null;
            }
            existingUserArticle.Title = updateUserArticleDTO.Title;
            existingUserArticle.Description = updateUserArticleDTO.Description;
            existingUserArticle.Content = updateUserArticleDTO.Content;
            existingUserArticle.UpdatedAt = DateTime.Now;
            //_context.UserArticle.Update(existingUserArticle);
            // Remove existing tags
            var existingTags = _context.UserArticleUserArticleTag
                .Where(ut => ut.UserArticleId == updateUserArticleDTO.UserArticleId).ToList();
            _context.UserArticleUserArticleTag.RemoveRange(existingTags);

            // Add new tags
            if (updateUserArticleDTO.UserArticleTagIds != null && updateUserArticleDTO.UserArticleTagIds.Any())
            {
                foreach (var tagId in updateUserArticleDTO.UserArticleTagIds)
                {
                    var newTag = new UserArticleUserArticleTag
                    {
                        UserArticleId = updateUserArticleDTO.UserArticleId, // Make sure you're using UserArticleId here if needed
                        UserArticleTagId = tagId
                    };
                    _context.UserArticleUserArticleTag.Add(newTag);
                }
            }

            await _context.SaveChangesAsync();
            return existingUserArticle;
        }
        public async Task<ChangeUserArticleStatusResponseDTO> SetUserArticleInactive(UserArticle userArticle)
        {
            // Set the IsActive property to false
            userArticle.Status = "INACTIVE";

            // Optionally, save the changes to the database if you're using an ORM like Entity Framework
            // For example:
            _context.UserArticle.Update(userArticle);
            await _context.SaveChangesAsync();


            return new ChangeUserArticleStatusResponseDTO
            {
                Status = userArticle.Status,
                Message = "UserArticle's Status Set INACTIVE done",
                UserArticleId = userArticle.UserArticleId,
            };
        }
        public async Task<ChangeUserArticleStatusResponseDTO> SetUserArticleActive(UserArticle userArticle)
        {
            // Set the IsActive property to false
            userArticle.Status = "ACTIVE";

            // Optionally, save the changes to the database if you're using an ORM like Entity Framework
            // For example:
            _context.UserArticle.Update(userArticle);
            await _context.SaveChangesAsync();


            return new ChangeUserArticleStatusResponseDTO
            {
                Status = userArticle.Status,
                Message = "UserArticle's Status Set ACTIVE done",
                UserArticleId = userArticle.UserArticleId,
            };
        }

        public async Task<ChangeUserArticleStatusResponseDTO> SetUserArticleApproved(UserArticle userArticle)
        {
            // Set the IsActive property to false
            userArticle.Status = "ACTIVE";

            // Optionally, save the changes to the database if you're using an ORM like Entity Framework
            // For example:
            _context.UserArticle.Update(userArticle);
            await _context.SaveChangesAsync();


            return new ChangeUserArticleStatusResponseDTO
            {
                Status = userArticle.Status,
                Message = "UserArticle Approved, Status set to ACTIVE",
                UserArticleId = userArticle.UserArticleId,
            };
        }

    }
}
