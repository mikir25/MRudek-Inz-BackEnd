using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Entities.Conversation
{
    public class UsersGroupDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupConversationId { get; set; }

    }
}
