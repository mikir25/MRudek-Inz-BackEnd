using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models
{
    public class Picture
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string picture { get; set; }

        public int? EventId { get; set; }
        public int? ForumId { get; set; }
        public int? GadgetId { get; set; }
        public int? PostId { get; set; }
        public int? TutorialId { get; set; }
    }
}
