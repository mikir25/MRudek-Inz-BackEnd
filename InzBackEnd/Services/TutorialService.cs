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
    public interface ITutorialService
    {
        int Create(Tutorial tutorial);
        public int CreateComment(CommentDto commentDto);
        void Delete(int id);
        IEnumerable<Tutorial> GetAll();
        Tutorial GetById(int id);
        void Update(int id, Tutorial newtutorial);
    }

    public class TutorialService : ITutorialService
    {
        private readonly PortalDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<TutorialService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public TutorialService(PortalDbContext dbContext, IMapper mapper, ILogger<TutorialService> logger
        , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }

        public Tutorial GetById(int id)
        {
            var tutorial = dbContext.Tutorials
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(t => t.Id == id);

            if (tutorial is null)
            {
                throw new NotFoundException("Tutorial not found");
            }

            var RatingValue = 0;
            var RatingCount = 0;

            for (int i=0; i < tutorial.Comments.Count; i++)
            {
                if( tutorial.Comments[i].Rating != null)
                {
                    RatingValue += (int)tutorial.Comments[i].Rating;
                    RatingCount++;
                }
            }
            if (RatingCount > 0)
            {
                var RatingAverage = RatingValue / RatingCount;
                tutorial.Rating = RatingAverage;
            }

            return tutorial;
        }

        public IEnumerable<Tutorial> GetAll()
        {
            var tutorials = dbContext.Tutorials
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .ToList().Reverse<Tutorial>();

            for(int j=0; j < tutorials.Count(); j++)
            {
                var RatingValue = 0;
                var RatingCount = 0;

                for (int i = 0; i < tutorials.ElementAt(j).Comments.Count; i++)
                {
                    if (tutorials.ElementAt(j).Comments[i].Rating != null)
                    {
                        RatingValue += (int)tutorials.ElementAt(j).Comments[i].Rating;
                        RatingCount++;
                    }
                }

                if(RatingCount > 0)
                {
                    var RatingAverage = RatingValue / RatingCount;
                    tutorials.ElementAt(j).Rating = RatingAverage;
                }

            }

            return tutorials;
        }

        public int Create(Tutorial tutorial)
        {
            tutorial.UserId = (int)userContextService.GetUserId;
            dbContext.Tutorials.Add(tutorial);
            dbContext.SaveChanges();

            return tutorial.Id;
        }

        public int CreateComment(CommentDto commentDto)
        {
            Comment comment = new Comment();
            comment.UserId = (int)userContextService.GetUserId;
            comment.Contents = commentDto.Contents;
            comment.TutorialId = commentDto.Id;
            comment.Rating = commentDto.Rating;
            comment.UserName = userContextService.GetUserName;

            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            return comment.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Tutorial with id: {id} DELETE action invoked");

            var tutorial = dbContext.Tutorials
                .Include(r => r.Comments)
                .Include(r => r.Pictures)
                .FirstOrDefault(t => t.Id == id);

            if (tutorial is null)
            {
                throw new NotFoundException("Tutorial not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, tutorial,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dbContext.Tutorials.Remove(tutorial);
            dbContext.SaveChanges();
        }

        public void Update(int id, Tutorial newtutorial)
        {
            var tutorial = dbContext.Tutorials.FirstOrDefault(t => t.Id == id);

            if (tutorial is null)
            {
                throw new NotFoundException("Tutorial not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, tutorial,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            tutorial.Contents = newtutorial.Contents;
            tutorial.Comments = newtutorial.Comments;
            tutorial.Pictures = newtutorial.Pictures;
            
            dbContext.SaveChanges();
        }
    }
}
