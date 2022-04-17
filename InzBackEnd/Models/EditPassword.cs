using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models
{
    public class EditPassword : IContent
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string PasswordLast { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
