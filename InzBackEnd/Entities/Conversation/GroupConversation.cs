using InzBackEnd.Entities.Conversation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities
{
    public class GroupConversation
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Message> Massages { get; set; }
       
        public virtual List<UserGroup> UsersGroup { get; set; }        
    }
}
