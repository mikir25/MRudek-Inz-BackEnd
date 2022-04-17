using AutoMapper;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Exceptions;
using InzBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InzBackEnd.Services
{
    public interface IPostService
    {
        int Create(Post post);

        public int CreateComment(CommentDto commentDto);

        void Delete(int id);

        IEnumerable<Post> GetAll();

        Post GetById(int id);

        void Update(int id, Post newpost);
    }

    public class PostService : IPostService
    {
        private readonly PortalDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<PostService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public PostService(PortalDbContext dbContext, IMapper mapper, ILogger<PostService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public Post GetById(int id)
        {
            var post = dbContext.Posts
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(p => p.Id == id);

            if (post is null)
            {
                throw new NotFoundException("Post not found");
            }

            return post;
        }

        public IEnumerable<Post> GetAll()
        {
            var posts = dbContext.Posts
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .ToList().Reverse<Post>();

            return posts;
        }

        public int Create(Post post)
        {
            post.UserId = (int)userContextService.GetUserId;
            dbContext.Posts.Add(post);
            dbContext.SaveChanges();

            return post.Id;
        }

        public int CreateComment(CommentDto commentDto)
        {
            Comment comment = new Comment();
            comment.UserId = (int)userContextService.GetUserId;
            comment.Contents = commentDto.Contents;
            comment.PostId = commentDto.Id;
            comment.UserName = userContextService.GetUserName;

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            return comment.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Post with id: {id} DELETE action invoked");
            var post = dbContext.Posts
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(p => p.Id == id);

            if (post is null)
            {
                throw new NotFoundException("Post not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, post,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();
        }

        public void Update(int id, Post newpost)
        {
            var post = dbContext.Posts.FirstOrDefault(p => p.Id == id);

            if (post is null)
            {
                throw new NotFoundException("Post not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, post,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            post.Contents = newpost.Contents;
            post.Comments = newpost.Comments;
            post.Pictures = newpost.Pictures;

            dbContext.SaveChanges();
        }
    }
}