using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using T2305M_API.Entities;
using T2305M_API.DTO.UserArticle.Requirements;
namespace T2305M_API.DTO.UserArticle.Handlers
{
    public class UpdateUserArticleHandler : AuthorizationHandler<UpdateUserArticleRequirement, Entities.UserArticle>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UpdateUserArticleRequirement requirement,
            Entities.UserArticle userArticle)
        {
            // Assuming we want to check if the user is the author of the post
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // If the user is the author of the post, succeed
            if (userId != null && userArticle.UserId.ToString() == userId)
            {
                context.Succeed(requirement);
            }

            // Otherwise, don't mark the requirement as succeeded
            return Task.CompletedTask;
        }
    }

}
