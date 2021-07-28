using System.ComponentModel.DataAnnotations;

namespace PrzyjaznyBot.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public ulong DiscordUserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Nickname { get; set; }

        [Required]
        public double Value { get; set; }


    }
}
