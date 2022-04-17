using AutoMapper;
using InzBackEnd.Entities;
using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Users;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd
{
    public class MappingProfile : Profile
    {
        private readonly PortalDbContext context;

        public MappingProfile(PortalDbContext context)
        {
            this.context = context;
        }
        public MappingProfile()
        {
            CreateMap<UserGroup, UsersDate>()
                .ForMember(m => m.Id, c => c.MapFrom(s => s.UserId))
                .ForMember(m => m.Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(m => m.Role, c => c.MapFrom(s => s.User.Role));

            CreateMap<User, UserDto>()                
                .ForMember(m => m.Role, c => c.MapFrom(s => s.Role.Name));

            CreateMap<UserGroup, UsersGroupDto>();

            CreateMap<GroupConversation, GroupDto>();

            CreateMap<GroupConversation, GroupConversationDto>();

        }
    }
}
