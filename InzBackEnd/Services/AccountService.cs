using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Models;
using InzBackEnd.Entities.Users;
using InzBackEnd.Exceptions;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Services
{
    public interface IAccountService
    {
        public UserDto GetUser(int id);
        public UserDto GetUserByName(string name);
        public UsersDate GetUserDate();
        void RegisterUser(RegisterUserDto dto);
        Token GenerateJwt(LoginDto dto);
        public IEnumerable<Friend> GetFriend();
        public IEnumerable<GroupDto> GetGroup();
        public int CreateFriend(Friend friend);
        public void DeleteFriend(int id);
        public IEnumerable<Mail> GetMail();
        public int CreateMail(Mail mail);
        public void DeleteMail(int id);
        public void EditDateUser(EditUser editUser);
        public void EditPassword(EditPassword editUser);
    }
    public class AccountService : IAccountService
    {
        private readonly PortalDbContext context;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly AuthenticationSettings authenticationSettings;
        private readonly ILogger<AccountService> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;
        private readonly IMapper mapper;

        public AccountService(PortalDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings
            , ILogger<AccountService> logger, IAuthorizationService authorizationService, IUserContextService userContextService, IMapper mapper)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
            this.mapper = mapper;
        }
        public UserDto GetUserByName(string name)
        {
            var user = context.Users
                .Include(u => u.Role)
                .Include(u => u.Mails)
                .Include(u => u.Friends)
                .Include(u => u.UserGroup)
                .FirstOrDefault(u => u.Name.Equals(name));

            if (user is null)
            {
                throw new NotFoundException("Friend not found");
            }

            var usersGroup = mapper.Map<List<UsersGroupDto>>(user.UserGroup);

            var userDto = mapper.Map<UserDto>(user);
            userDto.UsersGroup = usersGroup;

            return userDto;
        }

        public UserDto GetUser(int id)
        {           
            var user = context.Users
                .Include(u => u.Role)               
                .Include(u => u.Mails)
                .Include(u => u.Friends)
                .Include(u => u.UserGroup)
                .FirstOrDefault(u => u.Id == id);

            var usersGroup = mapper.Map<List<UsersGroupDto>>(user.UserGroup);

            var userDto = mapper.Map<UserDto>(user);
            userDto.UsersGroup = usersGroup;

            return userDto;
        }

        public UsersDate GetUserDate()
        {

            var usersDate = new UsersDate();
            usersDate.Id = (int)userContextService.GetUserId;
            usersDate.Name = userContextService.GetUserName;
            usersDate.Role = userContextService.GetUserRole;
            return usersDate;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Name = dto.Name,
                Email = dto.Email,
                RoleId = dto.RoleId,               
            };
            var hashedPassword = passwordHasher.HashPassword(newUser, dto.Password);

            newUser.HashPassword = hashedPassword;
            context.Users.Add(newUser);
            context.SaveChanges();
        }

        public Token GenerateJwt(LoginDto dto)
        {
            var user = context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email.Equals(dto.Email));

            if (user is null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.HashPassword, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(authenticationSettings.JwtIssuer,
                authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            Token tokenValue = new Token() { Value = tokenHandler.WriteToken(token) };
            return tokenValue;

        }

        public IEnumerable<Friend> GetFriend()
        {
            var friends = context.Friends.Where(e => e.UserId == (int)userContextService.GetUserId);

            return friends;
        }

        public IEnumerable<GroupDto> GetGroup()
        {
            var user = context.Users
                .Include(u => u.UserGroup)
                .FirstOrDefault(u => u.Id == (int)userContextService.GetUserId);

            List<GroupDto> groupInUser = new List<GroupDto>();

            List<UserGroup> groupDto = user.UserGroup;

            foreach (UserGroup element in groupDto)
            {
                GroupConversation el = context.GroupConversations
                    .FirstOrDefault(u => u.Id == element.GroupConversationId);

                var date = mapper.Map<GroupDto>(el);

                groupInUser.Add(date);
            }

            return groupInUser;
        }

        public int CreateFriend(Friend friend)
        {

            friend.UserId = (int)userContextService.GetUserId;

            if (friend.Friend_Userid == (int)userContextService.GetUserId)
            {
                throw new NotFoundException("Something is wrong");
            }

            var tmp = context.Friends.FirstOrDefault(r => r.Friend_Userid == friend.Friend_Userid && r.UserId == (int)userContextService.GetUserId);
            if (tmp != null)
            {
                throw new NotFoundException("Something is wrong");
            }

            context.Friends.Add(friend);
            context.SaveChanges();
            return friend.Id;
        }

        public void DeleteFriend(int id)
        {
            logger.LogWarning($"Friend with id: {id} DELETE action invoked");
            var friend = context.Friends.FirstOrDefault(e => e.Id == id);

            if (friend is null)
            {
                throw new NotFoundException("Friend not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, friend,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            context.Friends.Remove(friend);
            context.SaveChanges();
        }

        public IEnumerable<Mail> GetMail()
        {
            var mails = context.Mails.Where(e => e.UserId == (int)userContextService.GetUserId);

            return mails;
        }

        public int CreateMail(Mail mail)
        {                     
            mail.UserName = userContextService.GetUserName;
            context.Mails.Add(mail);
            context.SaveChanges();
            return mail.Id;
        }

        public void DeleteMail(int id)
        {
            logger.LogWarning($"Mail with id: {id} DELETE action invoked");
            var mail = context.Mails.FirstOrDefault(e => e.Id == id);

            if (mail is null)
            {
                throw new NotFoundException("Mail not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, mail,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            context.Mails.Remove(mail);
            context.SaveChanges();
        }

        public void EditDateUser(EditUser editUser)
        {
            var user = context.Users.FirstOrDefault(e => e.Id == editUser.UserId);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, editUser,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            if(editUser.Name != null)
            {
                user.Name = editUser.Name;
            }

            if (editUser.Email != null)
            {
                user.Email = editUser.Email;
            }

            context.SaveChanges();
        }

        public void EditPassword(EditPassword editUser)
        {
            var user = context.Users.FirstOrDefault(e => e.Id == editUser.UserId);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, editUser,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.HashPassword, editUser.PasswordLast);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid password");
            }

            if (editUser.Password.Equals(editUser.ConfirmPassword) )
            {
               var hashedPassword = passwordHasher.HashPassword(user, editUser.Password);

                user.HashPassword = hashedPassword;
                context.SaveChanges();
            }else
            {
                throw new NotFoundException("Passwords are different");
            }
 
        }
    }
}
