using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Models;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string HashPassword { get; set; }
        [Required]
        public string Email { get; set; }

        public virtual List<UserGroup> UserGroup { get; set; }

        [Required]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual List<Mail> Mails { get; set; }

        public virtual List<Friend> Friends { get; set; }
    }
}
