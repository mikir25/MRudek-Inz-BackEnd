using InzBackEnd.Entities;
using InzBackEnd.Models;
using InzBackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace InzBackEnd.Controllers
{
    [Route("api/Post")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService postSevice;

        public PostController(IPostService postSevice)
        {
            this.postSevice = postSevice;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetAll()
        {
            var posts = postSevice.GetAll();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public ActionResult<Post> Get([FromRoute] int id)
        {
            var post = postSevice.GetById(id);
            return Ok(post);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Post post)
        {
            var id = postSevice.Create(post);
            return Created($"/api/Post/{id}", post);
        }

        [HttpPost("Comment")]
        public ActionResult CreateComment([FromBody] CommentDto comment)
        {
            var id = postSevice.CreateComment(comment);
            return Created($"Create Comment: {id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            postSevice.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] Post post, [FromRoute] int id)
        {
            postSevice.Update(id, post);
            return Ok();
        }
    }
}