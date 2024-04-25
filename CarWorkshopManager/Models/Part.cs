using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWorkshopManager.Models
{
    // Part.cs
    public class Part
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }


        // Foreign key for Ticket
        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        // Navigation property for the Ticket this Part belongs to
        public virtual Ticket Ticket { get; set; }
    }

}

