using PrzyjaznyBot.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzyjaznyBot.Model
{
    public class UserBet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual int UserId { get; set; }

        [Required]
        public virtual int BetId { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public Condition Condition { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
