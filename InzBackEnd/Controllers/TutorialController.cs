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
    [Route("api/Tutorial")]
    [ApiController]
    [Authorize]
    public class TutorialController : ControllerBase
    {
        private readonly ITutorialService tutorialtSevice;

        public TutorialController(ITutorialService tutorialtSevice)
        {
            this.tutorialtSevice = tutorialtSevice;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Tutorial>> GetAll()
        {
            var tutorials = tutorialtSevice.GetAll();
            return Ok(tutorials);
        }

        [HttpGet("{id}")]
        public ActionResult<Tutorial> Get([FromRoute] int id)
        {
            var _tutorial = tutorialtSevice.GetById(id);

            return Ok(_tutorial);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Tutorial _tutorial)
        {

            var id = tutorialtSevice.Create(_tutorial);
            return Created($"/api/Tutorial/{id}", _tutorial);
        }
        
        [HttpPost("Comment")]
        public ActionResult CreateComment([FromBody] CommentDto comment)
        {

            var id = tutorialtSevice.CreateComment(comment);
            return Created($"Create Comment: {id}", null);
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            tutorialtSevice.Delete(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] Tutorial _tutorial, [FromRoute] int id)
        {
            tutorialtSevice.Update(id, _tutorial);

            return Ok();
        }
    }
}
