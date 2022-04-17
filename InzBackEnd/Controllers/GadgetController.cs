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
    [Route("api/Gadget")]
    [ApiController]
    [Authorize]
    public class GadgetController : ControllerBase
    {
        private readonly IGadgetService gadgetSevice;

        public GadgetController(IGadgetService gadgetSevice)
        {
            this.gadgetSevice = gadgetSevice;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Gadget>> GetAll()
        {
            var gadgets = gadgetSevice.GetAll();
            return Ok(gadgets);
        }

        [HttpGet("{id}")]
        public ActionResult<Gadget> Get([FromRoute] int id)
        {
            var gadget = gadgetSevice.GetById(id);
            return Ok(gadget);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Gadget gadget)
        {
            var id = gadgetSevice.Create(gadget);
            return Created($"/api/Gadget/{id}", gadget);
        }
        
        [HttpPost("Comment")]
        public ActionResult CreateComment([FromBody] CommentDto comment)
        {
            var id = gadgetSevice.CreateComment(comment);
            return Created($"Create Comment: {id}", null);
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            gadgetSevice.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] Gadget gadget, [FromRoute] int id)
        {
            gadgetSevice.Update(id, gadget);
            return Ok();
        }
    }
}
