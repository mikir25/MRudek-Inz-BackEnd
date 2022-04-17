using Microsoft.EntityFrameworkCore;
using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Models;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities
{
    public class PortalDbContext : DbContext
    {
        //Db Users
        public DbSet<User> Users { get; set; }       

        public DbSet<Mail> Mails { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Friend> Friends { get; set; }

        //Db Contents       
        public DbSet<Event> Events { get; set; }

        public DbSet<Forum> Forums { get; set; }

        public DbSet<Gadget> Gadgets { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Tutorial> Tutorials { get; set; }

        public DbSet<Categorie> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        //Db Conversation
        public DbSet<GroupConversation> GroupConversations { get; set; }

        public DbSet<UserGroup> UsersGrup { get; set; }

        public DbSet<Message> Messages { get; set; }


        public PortalDbContext(DbContextOptions<PortalDbContext> options): base(options)
        {}

        /*
        add-migration Init
        update-database
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Users
            modelBuilder.Entity<User>();
          
            modelBuilder.Entity<Mail>();

            modelBuilder.Entity<Role>();

            modelBuilder.Entity<Friend>();

            //Contents
            modelBuilder.Entity<Event>();

            modelBuilder.Entity<Forum>();

            modelBuilder.Entity<Gadget>();

            modelBuilder.Entity<Post>();

            modelBuilder.Entity<Tutorial>();

            modelBuilder.Entity<Categorie>();

            modelBuilder.Entity<Comment>();

            modelBuilder.Entity<Picture>();

            //Conversation
            modelBuilder.Entity<GroupConversation>();

            modelBuilder.Entity<UserGroup>();

            modelBuilder.Entity<Message>();

        }

    }
}
