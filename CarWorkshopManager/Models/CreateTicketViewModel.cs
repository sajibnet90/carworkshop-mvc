using System.ComponentModel.DataAnnotations;

namespace CarWorkshopManager.Models
{
    public class CreateTicketViewModel
    {
        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string RegistrationId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime RepairSchedule { get; set; }
    }
}
