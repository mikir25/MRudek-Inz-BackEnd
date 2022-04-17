using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities.Models
{
    public class Friend: IContent
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public int Friend_Userid { get; set; }
       
        public int UserId { get; set; }
    }
}
