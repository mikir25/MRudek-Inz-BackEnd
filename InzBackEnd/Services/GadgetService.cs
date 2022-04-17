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
    public interface IGadgetService
    {
        int Create(Gadget gadget);
        public int CreateComment(CommentDto commentDto);
        void Delete(int id);
        IEnumerable<Gadget> GetAll();
        Gadget GetById(int id);
        void Update(int id, Gadget newgadget);
    }

    public class GadgetService : IGadgetService
    {
        private readonly PortalDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<GadgetService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public GadgetService(PortalDbContext dbContext, IMapper mapper, ILogger<GadgetService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public Gadget GetById(int id)
        {
            var gadget = dbContext.Gadgets
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(g => g.Id == id);

            if (gadget is null)
            {
                throw new NotFoundException("Gadget not found");
            }

            return gadget;
        }

        public IEnumerable<Gadget> GetAll()
        {
            var gadgets = dbContext.Gadgets
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .ToList().Reverse<Gadget>();

            return gadgets;
        }

        public int Create(Gadget gadget)
        {
            gadget.UserId = (int)userContextService.GetUserId;
            dbContext.Gadgets.Add(gadget);
            dbContext.SaveChanges();

            return gadget.Id;
        }

        public int CreateComment(CommentDto commentDto)
        {
            Comment comment = new Comment();
            comment.UserId = (int)userContextService.GetUserId;
            comment.Contents = commentDto.Contents;
            comment.GadgetId = commentDto.Id;
            comment.UserName = userContextService.GetUserName;

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            return comment.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Gadget with id: {id} DELETE action invoked");
            var gadget = dbContext.Gadgets
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(g => g.Id == id);

            if (gadget is null)
            {
                throw new NotFoundException("Gadget not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, gadget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Gadgets.Remove(gadget);
            dbContext.SaveChanges();
        }

        public void Update(int id, Gadget newgadget)
        {
            var gadget = dbContext.Gadgets.FirstOrDefault(g => g.Id == id);

            if (gadget is null)
            {
                throw new NotFoundException("Gadget not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, gadget,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            gadget.Contents = newgadget.Contents;
            gadget.Comments = newgadget.Comments;
            gadget.Pictures = newgadget.Pictures;

            dbContext.SaveChanges();
        }
    }
}
