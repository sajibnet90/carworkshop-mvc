using System.ComponentModel.DataAnnotations;

namespace CarWorkshopManager.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
