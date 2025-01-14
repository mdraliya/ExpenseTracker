namespace ExpenseTracker.Models
{
    public class CashOutflow
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}