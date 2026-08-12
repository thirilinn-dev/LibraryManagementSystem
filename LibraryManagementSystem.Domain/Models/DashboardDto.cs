namespace LibraryManagementSystem.Domain.Models;

public class DashboardDto
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int ActiveBorrowings { get; set; }
    public int OverdueBooks { get; set; }
    public int RegisteredBorrowers { get; set; }
}
