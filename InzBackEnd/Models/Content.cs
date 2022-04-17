using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models
{
    public interface IContent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
