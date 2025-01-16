namespace ExpenseTracker.Models
{
    public class Debt
    {
        public int Id { get; set; } // Unique identifier for each debt
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public bool IsCleared { get; set; }
    }
}
