using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Exceptions;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Services
{
    public interface IForumService
    {
        int Create(Forum forum);
        public int CreateComment(CommentDto commentDto);
        void Delete(int id);
        IEnumerable<Forum> GetAll();
        IEnumerable<Categorie> GetAllCategorie();
        Forum GetById(int id);
        void Update(int id, Forum newforum);
    }

    public class ForumService : IForumService
    {
        private readonly PortalDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ForumService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public ForumService(PortalDbContext dbContext, IMapper mapper, ILogger<ForumService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public Forum GetById(int id)
        {
            var forum = dbContext.Forums
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(f => f.Id == id);

            if (forum is null)
            {
                throw new NotFoundException("Forum not found");
            }

            return forum;
        }

        public IEnumerable<Forum> GetAll()
        {
            var forums = dbContext.Forums
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .Include(r => r.Categorie)
                .ToList().Reverse<Forum>();

            return forums;
        }

        public IEnumerable<Categorie> GetAllCategorie()
        {
            var categories = dbContext.Categories.ToList();

            return categories;
        }

        public int Create(Forum forum)
        {
            forum.UserId = (int)userContextService.GetUserId;
            dbContext.Forums.Add(forum);
            dbContext.SaveChanges();

            return forum.Id;
        }

        public int CreateComment(CommentDto commentDto)
        {
            Comment comment = new Comment();
            comment.UserId = (int)userContextService.GetUserId;
            comment.Contents = commentDto.Contents;
            comment.ForumId = commentDto.Id;
            comment.UserName = userContextService.GetUserName;

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            return comment.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Forum with id: {id} DELETE action invoked");
            var forum = dbContext.Forums
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(f => f.Id == id);

            if (forum is null)
            {
                throw new NotFoundException("Forum not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, forum,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Forums.Remove(forum);
            dbContext.SaveChanges();
        }

        public void Update(int id, Forum newforum)
        {
            var forum = dbContext.Forums.FirstOrDefault(f => f.Id == id);

            if (forum is null)
            {
                throw new NotFoundException("Forum not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, forum,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            forum.Contents = newforum.Contents;
            forum.Comments = newforum.Comments;
            forum.Pictures = newforum.Pictures;
            forum.CategorieId = newforum.CategorieId;

            dbContext.SaveChanges();
        }
    }
}
