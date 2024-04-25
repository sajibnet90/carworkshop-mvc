using CarWorkshopManager.Models;

public class TicketCostViewModel
{
    public Ticket Ticket { get; set; }
    public decimal PartsTotal { get; set; }
    public decimal LaborTotal { get; set; }
    public decimal GrandTotal { get; set; }
}
