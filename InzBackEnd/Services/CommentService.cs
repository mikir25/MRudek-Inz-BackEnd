using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Exceptions;
using InzBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace InzBackEnd.Services
{
    public interface ICommentService
    {
        void Delete(int id);

        void Update(int id, CommentDto newevent);
    }

    public class CommentService : ICommentService
    {
        private readonly PortalDbContext dbContext;
        private readonly IAuthorizationService authorizationService;
        private readonly ILogger<CommentService> logger;
        private readonly IUserContextService userContextService;

        public CommentService(PortalDbContext dbContext, IAuthorizationService authorizationService, ILogger<CommentService> logger
            , IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.authorizationService = authorizationService;
            this.logger = logger;
            this.userContextService = userContextService;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Comments with id: {id} DELETE action invoked");
            var comment = dbContext.Comments.FirstOrDefault(e => e.Id == id);

            if (comment is null)
            {
                throw new NotFoundException("Comment not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, comment,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Comments.Remove(comment);
            dbContext.SaveChanges();
        }

        public void Update(int id, CommentDto newcomment)
        {
            var comment = dbContext.Comments.FirstOrDefault(e => e.Id == id);

            if (comment is null)
            {
                throw new NotFoundException("Comments not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, comment,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            if (newcomment != null)
                comment.Rating = newcomment.Rating;

            comment.Contents = newcomment.Contents;

            dbContext.SaveChanges();
        }
    }
}