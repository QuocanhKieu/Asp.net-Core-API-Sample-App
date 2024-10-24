using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using T2305M_API.DTO.User.Requirements;
using T2305M_API.Entities;
namespace T2305M_API.DTO.User.Handlers
{
    public class ValidYearOldHandler : AuthorizationHandler<YearOldRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, YearOldRequirement requirement)
        {
            // kiem tra tuổi của user để biết suceed hay fail
            if (IsValidYearOld(context.User, requirement))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }

        private bool IsValidYearOld(ClaimsPrincipal user, YearOldRequirement requirement)
        {
            if (user == null)
                return false;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var _context = new T2305mApiContext();
            var userData = _context.User.Find(Convert.ToInt32(userId));
            if (userData == null || userData.Age == null || userData.Age == 0)
                return false;
            if (userData.Age >= requirement.Min &&
                userData.Age <= requirement.Max)
                return true;
            return false;
        }
    }
}

