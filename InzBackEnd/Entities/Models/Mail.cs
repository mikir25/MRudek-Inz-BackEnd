using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities.Models
{
    public class Mail : IContent
    {
        public int Id { get; set; }        
        public string UserName { get; set; }
        [Required]
        public string Contents { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
