using InzBackEnd.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models
{
    public class Comment : IContent
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }       
        [Required]
        public string Contents { get; set; }
        [Required]
        public int UserId { get; set; }
        [Range(1, 10)]
        public int? Rating { get; set; }
        public int? EventId { get; set; }
        public int? ForumId { get; set; }
        public int? GadgetId { get; set; }
        public int? PostId { get; set; }
        public int? TutorialId { get; set; }
    }
}
