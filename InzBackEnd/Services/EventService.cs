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
    public interface IEventService
    {
        int Create(Event _event);
        public int CreateComment(CommentDto commentDto);
        void Delete(int id);
        IEnumerable<Event> GetAll();
        Event GetById(int id);
        void Update(int id, Event newevent);
    }

    public class EventService : IEventService
    {
        private readonly PortalDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<EventService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public EventService(PortalDbContext dbContext, IMapper mapper, ILogger<EventService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public Event GetById(int id)
        {
            var _event = dbContext.Events
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(e => e.Id == id);

            if (_event is null)
            {
                throw new NotFoundException("Event not found");
            }

            return _event;
        }

        public IEnumerable<Event> GetAll()
        {
            var _events = dbContext.Events
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .ToList().Reverse<Event>();

            return _events;
        }

        public int Create(Event _event)
        {
            _event.UserId = (int)userContextService.GetUserId;
            dbContext.Events.Add(_event);
            dbContext.SaveChanges();

            return _event.Id;
        }

        public int CreateComment(CommentDto commentDto)
        {
            Comment comment = new Comment();
            comment.UserId = (int)userContextService.GetUserId;
            comment.Contents = commentDto.Contents;
            comment.EventId = commentDto.Id;
            comment.UserName = userContextService.GetUserName;

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            return comment.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Event with id: {id} DELETE action invoked");
            var _event = dbContext.Events
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(e => e.Id == id);

            if (_event is null)
            {
                throw new NotFoundException("Event not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, _event,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Events.Remove(_event);
            dbContext.SaveChanges();
        }

        public void Update(int id, Event newevent)
        {
            var _event = dbContext.Events.FirstOrDefault(e => e.Id == id);

            if (_event is null)
            {
                throw new NotFoundException("Event not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, _event,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _event.Contents = newevent.Contents;
            _event.DateOfEvent = newevent.DateOfEvent;
            _event.PlaceOfEvent = newevent.PlaceOfEvent;
            _event.Comments = newevent.Comments;
            _event.Pictures = newevent.Pictures;

            dbContext.SaveChanges();
        }
    }
}
