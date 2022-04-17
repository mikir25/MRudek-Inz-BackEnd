using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InzBackEnd.Entities;
using InzBackEnd.Models;
using InzBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InzBackEnd.Entities.Conversation;

namespace InzBackEnd.Controllers
{
    [Route("api/Group")]
    [ApiController]
    [Authorize]
    public class GroupConversationController : ControllerBase
    {
        private readonly IGroupConversationService groupService;

        public GroupConversationController(IGroupConversationService groupService)
        {
            this.groupService = groupService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GroupConversationDto>> GetAllGroup()
        {
            var groups = groupService.GetAllGroup();
            return Ok(groups);
        }

        [HttpPost("AddUser/{GroupId}")]
        public ActionResult AddUserGroup([FromRoute] int groupId)
        {
            groupService.AddUserGroup(groupId);
            return Ok();
        }

        [HttpPost("Message/{GroupId}")]
        public ActionResult CreateMassage([FromBody] Message message, [FromRoute] int groupId)
        {
            groupService.CreateMassage(message, groupId);
            return Created("", null);
        }

        [HttpPost]
        public ActionResult CreateGroup([FromBody] GroupConversation group)
        {
            groupService.CreateGroup(group);
            return Created("", null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUserInGroup([FromRoute] int id)
        {
            groupService.DeleteUserInGroup(id);
            return NoContent();
        }

        [HttpGet("Users/{id}")]
        public ActionResult<IEnumerable<UsersDate>> GetUsersGroup(int id)
        {
            var friends = groupService.GetUsersGroup(id);
            return Ok(friends);
        }

        [HttpGet("Messages/{id}")]
        public ActionResult<IEnumerable<Message>> GetMassageGroup(int id)
        {
            var messages = groupService.GetMassageGroup(id);
            return Ok(messages);
        }

    }
}
