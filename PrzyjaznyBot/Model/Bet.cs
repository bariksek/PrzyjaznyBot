﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzyjaznyBot.Model
{
    public class Bet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual int AuthorId { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(256)]
        public string Message { get; set; }
    }
}