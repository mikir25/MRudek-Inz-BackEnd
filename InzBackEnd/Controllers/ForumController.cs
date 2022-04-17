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

namespace InzBackEnd.Controllers
{
    [Route("api/Forum")]
    [ApiController]
    [Authorize]
    public class ForumController : ControllerBase
    {
        private readonly IForumService forumService;

        public ForumController(IForumService forumService)
        {
            this.forumService = forumService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Forum>> GetAll()
        {
            var forums = forumService.GetAll();
            return Ok(forums);
        }

        [AllowAnonymous]
        [HttpGet("Categories")]
        public ActionResult<IEnumerable<Categorie>> GetAllCategorie()
        {
            var categories = forumService.GetAllCategorie();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public ActionResult<Forum> Get([FromRoute] int id)
        {
            var forum = forumService.GetById(id);
            return Ok(forum);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Forum forum)
        {
            var id = forumService.Create(forum);
            return Created($"/api/Forum/{id}", forum);
        }
        
        [HttpPost("Comment")]
        public ActionResult CreateComment([FromBody] CommentDto comment)
        {
            var id = forumService.CreateComment(comment);
            return Created($"Create Comment: {id}", null);
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            forumService.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] Forum forum, [FromRoute] int id)
        {
            forumService.Update(id, forum);
            return Ok();
        }
    }
}
