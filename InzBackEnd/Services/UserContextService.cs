using Microsoft.AspNetCore.Http;
using InzBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Services
{
    public interface IUserContextService
    {
        int? GetUserId { get; }
        string GetUserName { get; }
        string GetUserRole { get; }
        ClaimsPrincipal User { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;

        public int? GetUserId =>
            User is null ? null : (int?)int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public string GetUserName =>
            User is null ? null : User.FindFirst(c => c.Type == ClaimTypes.Name).Value;

        public string GetUserRole =>
            User is null ? null : User.FindFirst(c => c.Type == ClaimTypes.Role).Value;
    }
}
