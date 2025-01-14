namespace ExpenseTracker.Models
{
    public class Debt
    {
        public DateTime Date { get; set; } = DateTime.Now; // Default to current date
        public string Title { get; set; } = string.Empty; // Default to empty string
        public decimal Amount { get; set; } // Amount for the debt
        public string Source { get; set; } = string.Empty; // Source of the debt
    }
}