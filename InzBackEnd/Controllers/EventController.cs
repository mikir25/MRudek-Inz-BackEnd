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
    [Route("api/Event")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService eventSevice;

        public EventController(IEventService eventSevice)
        {
            this.eventSevice = eventSevice;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Event>> GetAll()
        {
            var events = eventSevice.GetAll();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public ActionResult<Event> Get([FromRoute] int id)
        {
            var _event = eventSevice.GetById(id);
            return Ok(_event);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Event _event)
        {
            var id = eventSevice.Create(_event);
            return Created($"/api/Event/{id}", _event);
        }
        
        [HttpPost("Comment")]
        public ActionResult CreateComment([FromBody] CommentDto comment)
        {
            var id = eventSevice.CreateComment(comment);
            return Created($"Create Comment: {id}", null);
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            eventSevice.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] Event _event, [FromRoute] int id)
        {
            eventSevice.Update(id, _event);
            return Ok();
        }
    }
}
