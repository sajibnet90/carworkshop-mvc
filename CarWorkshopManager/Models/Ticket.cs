using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWorkshopManager.Models
{
    // Ticket.cs
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegistrationId { get; set; }
        public string Description { get; set; }
        public int Hours { get; set; } // Total hours of labor on the ticket

        // Collection of Parts associated with the Ticket
        public virtual ICollection<Part> Parts { get; set; } = new List<Part>();

    }
}
