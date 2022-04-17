using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Entities.Conversation;
using InzBackEnd.Exceptions;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Services
{
    public interface IGroupConversationService
    {
        public int CreateMassage(Message message, int GroupId);
        int CreateGroup(GroupConversation group);
        public IEnumerable<GroupConversationDto> GetAllGroup();
        public void AddUserGroup(int id);
        void DeleteUserInGroup(int idGroup);
        public IEnumerable<UsersDate> GetUsersGroup(int id);
        public IEnumerable<Message> GetMassageGroup(int id);
    }

    public class GroupConversationService : IGroupConversationService
    {
        private readonly PortalDbContext context;
        private readonly ILogger<AccountService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;
        private readonly IMapper mapper;

        public GroupConversationService(PortalDbContext context, ILogger<AccountService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
            this.mapper = mapper;
        }

        public int CreateMassage(Message message , int GroupId)
        {
            message.UserId = (int)userContextService.GetUserId;
            message.GroupConversationId = GroupId;
            context.Messages.Add(message);
            context.SaveChanges();

            return message.Id;
        }

        public IEnumerable<GroupConversationDto> GetAllGroup()
        {

            var tmp = context.GroupConversations
                .Include(u => u.UsersGroup)
                .ToList();

            var groups = new List<GroupConversationDto>();
            for (int i=0; i < tmp.Count; i++)
            { 
                if(tmp[i].UsersGroup.All(e => e.UserId != (int)userContextService.GetUserId) == true)
                {
                    var GroupDto = mapper.Map<GroupConversationDto>(tmp[i]);
                    groups.Add(GroupDto);
                }               
            }

            return groups;
        }

        public void AddUserGroup(int id)
        {
            var result = context.UsersGrup
                .Include(r => r.User)
                .Where(e => e.GroupConversationId == id);

            if (result is null)
            {
                throw new NotFoundException("Group not found");
            }

            var usersGroup = new UserGroup()
            {
                UserId = (int)userContextService.GetUserId,
                GroupConversationId = id
            };
            context.UsersGrup.Add(usersGroup);
            context.SaveChanges();
        }

        public IEnumerable<UsersDate> GetUsersGroup(int id)
        {
            
            var result = context.UsersGrup
                .Include(r => r.User)
                .Where(e => e.GroupConversationId == id);

            if (result is null)
            {
                throw new NotFoundException("Users not found");
            }             

            var usersDto = mapper.Map<List<UsersDate>>(result);            
            return usersDto;
            
        }

        public IEnumerable<Message> GetMassageGroup(int id)
        {
            var group = context.GroupConversations
                .Include(r => r.Massages)
                .FirstOrDefault(e => e.Id == id);

            if (group is null)
            {
                throw new NotFoundException("Group not found");
            }

            var massages = group.Massages;

            return massages;
        }

        public int CreateGroup(GroupConversation group)
        {
            var groupConversation = new GroupConversation()
            {
                Name = group.Name
            };
            context.GroupConversations.Add(groupConversation);
            context.SaveChanges();

            var usersGroup = new UserGroup()
            {
                UserId = (int)userContextService.GetUserId,
                GroupConversationId = groupConversation.Id
            };          
            context.UsersGrup.Add(usersGroup);
            context.SaveChanges();  

            return usersGroup.Id;
        }

        public void DeleteUserInGroup(int idGroup)
        {          
            logger.LogWarning($"User with id: {userContextService.GetUserId} in Group with id: {idGroup} DELETE action invoked");

            var userInGroup = context.UsersGrup.FirstOrDefault(e => e.GroupConversationId == idGroup && e.UserId == (int)userContextService.GetUserId);

            if (userInGroup is null)
            {
                throw new NotFoundException("Friend not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, userInGroup,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            context.UsersGrup.Remove(userInGroup);
            context.SaveChanges();           
        }

    }
}
