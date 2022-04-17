using InzBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities.Conversation
{
    public class UserGroup : IContent
    {
        public int Id { get; set; }       
        public int UserId { get; set; }      
        public int GroupConversationId { get; set; }

        public virtual User User { get; set; }
        public virtual GroupConversation GroupConversation { get; set; }
    }
}
