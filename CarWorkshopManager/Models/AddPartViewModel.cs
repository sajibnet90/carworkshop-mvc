namespace CarWorkshopManager.Models
{
    public class AddPartViewModel
    {
        public int TicketId { get; set; }
        public Part Part { get; set; }
        public List<Part> Parts { get; set; }
    }
}
