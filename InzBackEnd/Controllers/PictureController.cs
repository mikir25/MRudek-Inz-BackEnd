using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InzBackEnd.Models;
using InzBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Controllers
{
    [Route("api/Picture")]
    [ApiController]
    [Authorize]
    public class PictureController : ControllerBase
    {
        private readonly IPictureService pictureService;

        public PictureController(IPictureService pictureService)
        {
            this.pictureService = pictureService;
        }

        [HttpPost]
        public ActionResult Create([FromBody] Picture picture)
        {
            var id = pictureService.Create(picture);
            return Created($"Picture id: {id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            pictureService.Delete(id);
            return NoContent();
        }
    }
}
