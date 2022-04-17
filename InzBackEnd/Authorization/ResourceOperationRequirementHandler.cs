using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Models;

namespace InzBackEnd.Authorization
{
    public class ResourceOperationRequirementHandler<T>: AuthorizationHandler<ResourceOperationRequirement, T> where T: IContent
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, T subject)
        {
            if (requirement.ResourceOperation == ResourceOperation.Read || requirement.ResourceOperation == ResourceOperation.Create)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var userRole = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;

            if (subject.UserId == int.Parse(userId) || userRole == "Admin")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
