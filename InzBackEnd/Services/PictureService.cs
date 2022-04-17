using AutoMapper;
using Microsoft.Extensions.Logging;
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
    public interface IPictureService
    {
        int Create(Picture picture);
        void Delete(int id);
    }

    public class PictureService : IPictureService
    {
        private readonly PortalDbContext dbContext;
        private readonly ILogger<PictureService> logger;

        public PictureService(PortalDbContext dbContext, ILogger<PictureService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public int Create(Picture picture)
        {
            dbContext.Pictures.Add(picture);
            dbContext.SaveChanges();

            return picture.Id;
        }

        public void Delete(int id)
        {
            logger.LogWarning($"Picture with id: {id} DELETE action invoked");
            var picture = dbContext.Pictures.FirstOrDefault(p => p.Id == id);

            if (picture is null)
            {
                throw new NotFoundException("Picture not found");
            }

            dbContext.Pictures.Remove(picture);
            dbContext.SaveChanges();
        }
    }
}
