﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd.Models
{
    public class CommentDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Contents { get; set; }

        [Range(1, 10)]
        public int? Rating { get; set; }
    }
}