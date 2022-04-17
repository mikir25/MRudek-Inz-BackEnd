using Microsoft.EntityFrameworkCore;
using InzBackEnd.Controllers;
using InzBackEnd.Entities;
using InzBackEnd.Models;
using InzBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd
{
    public class PortalSeeder
    {
        
        private readonly PortalDbContext dbContext;
        private readonly IPostService postService;
        private readonly IAccountService accountService;
        private readonly IPictureService pictureService;

        public PortalSeeder(PortalDbContext dbContext, IPostService postService, IAccountService accountService, IPictureService pictureService)
        {
            this.dbContext = dbContext;
            this.postService = postService;
            this.accountService = accountService;
            this.pictureService = pictureService;
        }

        public void Seed()
        {
            
            if (dbContext.Database.CanConnect())
            {
                if (dbContext.Database.IsRelational())
                {
                    var pendingMigrations = dbContext.Database.GetPendingMigrations();
                    if (pendingMigrations != null && pendingMigrations.Any())
                    {
                        dbContext.Database.Migrate();
                    }
                }

                if (!dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    dbContext.Roles.AddRange(roles);
                    dbContext.SaveChanges();
                }

                if (!dbContext.Users.Any())
                {
                    var user = new RegisterUserDto()
                    {
                        Name = "User",
                        Email = "user@test.com",
                        Password = "test",
                        ConfirmPassword = "test",
                    };

                    accountService.RegisterUser(user);

                    if (dbContext.Database.IsRelational())
                    {
                        var admin = new RegisterUserDto()
                        {
                            Name = "Admin",
                            Email = "admin@test.com",
                            Password = "root",
                            ConfirmPassword = "root",
                        };

                        accountService.RegisterUser(admin);

                        var tmp = dbContext.Users.FirstOrDefault(p => p.Name == "Admin");
                        tmp.RoleId = 2;
                        dbContext.SaveChanges();
                    }

                }

                if (!dbContext.Categories.Any())
                {
                    var categories = new List<Categorie>() { 
                        new Categorie() { Name = "Kanoniczne" },
                        new Categorie() { Name = "Nie kanoniczne" },
                        new Categorie() { Name = "Inne" },
                    };

                    dbContext.Categories.AddRange(categories);
                    dbContext.SaveChanges();

                }
            }            
            
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Admin"
                },
            };

            return roles;
        }

    }
}
