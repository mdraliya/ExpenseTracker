namespace ExpenseTracker.Models;

public class CashInflow
{
    public DateTime Date { get; set; } = DateTime.Today; // Default to today's date
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}