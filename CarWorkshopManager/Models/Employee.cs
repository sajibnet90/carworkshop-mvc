using System;
namespace CarWorkshopManager.Models
{ 
// Employee.cs
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal HourlyRate { get; set; }
    // Other properties

    // Navigation property for calendar events
    public ICollection<CalendarEvent> CalendarEvents { get; set; }
}
}