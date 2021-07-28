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
        public int? Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Bet Bet { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public Condition Condition { get; set; }
    }
}
