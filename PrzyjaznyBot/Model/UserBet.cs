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
        public virtual User User { get; set; }

        [Required]
        public virtual Bet Bet { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public Condition Condition { get; set; }
    }
}
