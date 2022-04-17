using InzBackEnd.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InzBackEnd.Entities
{
    public class Post : IContent
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