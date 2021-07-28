using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrzyjaznyBot.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public ulong DiscordUserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        public double Value { get; set; }
    }
}
