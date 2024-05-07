namespace CarWorkshopManager.Models
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegistrationId { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
        public DateTime RepairSchedule { get; set; }
    }
}
