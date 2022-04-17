using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InzBackEnd.Entities;
using InzBackEnd.Entities.Models;
using InzBackEnd.Models;
using InzBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Controllers
{
    [Route("api/Account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet("User/{id}")]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            var user = accountService.GetUser(id);
            return Ok(user);
        }

        [HttpGet("UserByName/{name}")]
        public ActionResult<User> GetUserByName(string name)
        {
            var user = accountService.GetUserByName(name);
            return Ok(user);
        }

        [HttpGet("UserDate")]
        public ActionResult<UsersDate> GetUserDate()
        {
            var user = accountService.GetUserDate();
            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            accountService.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            Token token = accountService.GenerateJwt(dto);
            return Ok(token);
        }

        [HttpGet("Friends")]
        public ActionResult<IEnumerable<Friend>> GetFriend()
        {
            var friends = accountService.GetFriend();
            return Ok(friends);
        }

        [HttpGet("Groups")]
        public ActionResult<IEnumerable<GroupDto>> GetGroup()
        {
            var groups = accountService.GetGroup();
            return Ok(groups);
        }

        [HttpPost("Friend")]
        public ActionResult CreateFriend([FromBody] Friend friend)
        {
            accountService.CreateFriend(friend);
            return Created("", null);
        }

        [HttpDelete("Friend/{id}")]
        public ActionResult DeleteFriend([FromRoute] int id)
        {
            accountService.DeleteFriend(id);
            return NoContent();
        }

        [HttpGet("Mails")]
        public ActionResult<IEnumerable<Friend>> GetMail()
        {
            var mails = accountService.GetMail();
            return Ok(mails);
        }

        [HttpPost("Mail")]
        public ActionResult CreateMail([FromBody] Mail mail)
        {
            accountService.CreateMail(mail);
            return Created("", null);
        }

        [HttpDelete("Mail/{id}")]
        public ActionResult DeleteMail([FromRoute] int id)
        {
            accountService.DeleteMail(id);
            return NoContent();
        }

        [HttpPut("EditDateUser")]
        public ActionResult EditDateUser([FromBody] EditUser editUser)
        {
            accountService.EditDateUser(editUser);
            return Ok();
        }

        [HttpPut("EditPassword")]
        public ActionResult EditPassword([FromBody] EditPassword editUser)
        {
            accountService.EditPassword(editUser);

            return Ok();
        }
    }
}
