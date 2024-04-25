using System;
namespace CarWorkshopManager.Models
{ 
// CalendarEvent.cs
public class CalendarEvent
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    // Other properties for the event
}
}