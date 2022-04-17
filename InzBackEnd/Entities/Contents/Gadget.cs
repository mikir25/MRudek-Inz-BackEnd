using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities
{
    public class Gadget : IContent
    {
        public int Id { get; set; }
        [Required]
        public string Contents { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Picture> Pictures { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
