using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities
{
    public class Event: IContent
    {
        public int Id { get; set; }
        [Required]
        public string Contents { get; set; }
        [Required]
        public DateTime DateOfEvent { get; set; }
        [Required]
        public string PlaceOfEvent { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Picture> Pictures { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
