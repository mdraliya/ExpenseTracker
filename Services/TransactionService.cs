using ExpenseTracker.Models;
using System.Linq;
using System.Collections.Generic;


namespace ExpenseTracker.Services
{
    public class TransactionService
    {
        private readonly List<CashInflow> _cashInflows = new();
        private readonly List<CashOutflow> _cashOutflows = new();
        private readonly List<Debt> _debts = new(); // List to store debts

        // Event to notify UI of updates
        public event Action OnChange;

        // Add CashInflow transaction
        public void AddCashInflow(CashInflow transaction)
        {
            _cashInflows.Add(transaction);
            NotifyStateChanged();  // Notify when a new CashInflow is added
        }

        // Add CashOutflow transaction
        public void AddCashOutflow(CashOutflow transaction)
        {
            _cashOutflows.Add(transaction);
            NotifyStateChanged();  // Notify when a new CashOutflow is added
        }

        // Add Debt transaction
        public void AddDebt(Debt transaction)
        {
            _debts.Add(transaction);
            NotifyStateChanged();  // Notify when a new Debt is added
        }

        // Get all CashInflow transactions
        public List<CashInflow> GetAllCashInflows()
        {
            return _cashInflows;
        }

        // Get all CashOutflow transactions
        public List<CashOutflow> GetAllCashOutflows()
        {
            return _cashOutflows;
        }

        // Get all Debt transactions
        public List<Debt> GetAllDebts()
        {
            return _debts;
        }

        // Get all transactions (CashInflow, CashOutflow, and Debt combined)
        public List<object> GetAllTransactions()
        {
            // Combine all transactions into a single list
            return _cashInflows.Cast<object>()
                .Concat(_cashOutflows.Cast<object>())
                .Concat(_debts.Cast<object>())
                .ToList();
        }

        // Get Total Cash Inflow
        public decimal GetTotalCashInflow()
        {
            return _cashInflows.Sum(t => t.Amount);
        }

        // Get Total Cash Outflow
        public decimal GetTotalCashOutflow()
        {
            return _cashOutflows.Sum(t => t.Amount);
        }

        // Get Debt (Add logic to calculate debt if needed)
        public decimal GetTotalDebt()
        {
            return _debts.Sum(d => d.Amount); // Example: sum of all debts
        }

        // Get Available Balance (Cash Inflow - Cash Outflow + Debt)
        public decimal GetAvailableBalance()
        {
            return GetTotalCashInflow() - GetTotalCashOutflow() + GetTotalDebt(); // Add debt to available balance
        }

        // Method to trigger UI updates
        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}
