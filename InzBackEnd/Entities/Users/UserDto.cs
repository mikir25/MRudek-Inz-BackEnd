using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Models;
using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities.Users
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HashPassword { get; set; }
        
        public string Email { get; set; }

        public virtual List<UsersGroupDto> UsersGroup { get; set; }
      
        public string Role { get; set; }

        public List<Mail> Mails { get; set; }

        public List<Friend> Friends { get; set; }

    }
}
