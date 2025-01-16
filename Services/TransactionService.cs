using ExpenseTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ExpenseTracker.Services
{
    public class TransactionService
    {
        private static readonly string BaseDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExpenseTracker");

        private readonly string CashInflowFilePath = Path.Combine(BaseDirectory, "CashInflows.json");
        private readonly string CashOutflowFilePath = Path.Combine(BaseDirectory, "CashOutflows.json");
        private readonly string DebtFilePath = Path.Combine(BaseDirectory, "Debts.json");

        private readonly List<CashInflow> _cashInflows;
        private readonly List<CashOutflow> _cashOutflows;
        private readonly List<Debt> _debts;

        public TransactionService()
        {
            // Ensure the base directory exists
            Directory.CreateDirectory(BaseDirectory);

            // Load data from JSON files
            _cashInflows = LoadData<CashInflow>(CashInflowFilePath) ?? new List<CashInflow>();
            _cashOutflows = LoadData<CashOutflow>(CashOutflowFilePath) ?? new List<CashOutflow>();
            _debts = LoadData<Debt>(DebtFilePath) ?? new List<Debt>();
        }

        public event Action OnChange;

        public decimal CurrentBalance => GetTotalCashInflow() - GetTotalCashOutflow();

        // Fetch Top 5 Highest and Lowest Transactions
        public List<CashInflow> GetTop5HighestCashInflows() => _cashInflows.OrderByDescending(t => t.Amount).Take(5).ToList();
        public List<CashInflow> GetTop5LowestCashInflows() => _cashInflows.OrderBy(t => t.Amount).Take(5).ToList();

        public List<CashOutflow> GetTop5HighestCashOutflows() => _cashOutflows.OrderByDescending(t => t.Amount).Take(5).ToList();
        public List<CashOutflow> GetTop5LowestCashOutflows() => _cashOutflows.OrderBy(t => t.Amount).Take(5).ToList();

        public List<Debt> GetTop5HighestDebts() => _debts.OrderByDescending(d => d.Amount).Take(5).ToList();
        public List<Debt> GetTop5LowestDebts() => _debts.OrderBy(d => d.Amount).Take(5).ToList();

        // Add methods for Cash Inflow, Outflow, and Debt
        public void AddCashInflow(CashInflow transaction)
        {
            _cashInflows.Add(transaction);
            SaveData(CashInflowFilePath, _cashInflows);
            NotifyStateChanged();
        }

        public void AddCashOutflow(CashOutflow transaction)
        {
            if (CurrentBalance >= transaction.Amount)
            {
                _cashOutflows.Add(transaction);
                SaveData(CashOutflowFilePath, _cashOutflows);
                NotifyStateChanged();
            }
            else
            {
                throw new InvalidOperationException("Insufficient balance for this transaction.");
            }
        }

        public void AddDebt(Debt transaction)
        {
            _debts.Add(transaction);
            SaveData(DebtFilePath, _debts);
            NotifyStateChanged();
        }

        public void MarkDebtAsCleared(int debtId)
        {
            var debtToUpdate = _debts.FirstOrDefault(d => d.Id == debtId);
            if (debtToUpdate != null && !debtToUpdate.IsCleared)
            {
                debtToUpdate.IsCleared = true;
                SaveData(DebtFilePath, _debts);
                NotifyStateChanged();
            }
        }

        public void DeleteDebt(int debtId)
        {
            var debtToDelete = _debts.FirstOrDefault(d => d.Id == debtId);
            if (debtToDelete != null)
            {
                _debts.Remove(debtToDelete);
                SaveData(DebtFilePath, _debts);
                NotifyStateChanged();
            }
        }

        // Get all transactions
        public List<CashInflow> GetAllCashInflows() => _cashInflows;
        public List<CashOutflow> GetAllCashOutflows() => _cashOutflows;
        public List<Debt> GetAllDebts() => _debts;

        // Get Available Balance (Cash Inflow - Cash Outflow + Debt)
        public decimal GetAvailableBalance() => GetTotalCashInflow() - GetTotalCashOutflow() + GetTotalDebt();



        // Get totals
        public decimal GetTotalCashInflow() => _cashInflows.Sum(t => t.Amount);
        public decimal GetTotalCashOutflow() => _cashOutflows.Sum(t => t.Amount);
        public decimal GetTotalDebt() => _debts.Sum(d => d.Amount);
        public decimal GetClearedDebt() => _debts.Where(d => d.IsCleared).Sum(d => d.Amount);
        public decimal GetRemainingDebt() => _debts.Where(d => !d.IsCleared).Sum(d => d.Amount);

        // Update available balance when debt is cleared or added
        public void UpdateAvailableBalance(decimal debtAmount)
        {
            var updatedBalance = CurrentBalance - debtAmount;
            NotifyStateChanged();
        }

        // Notify UI updates
        private void NotifyStateChanged() => OnChange?.Invoke();

        // Save data to JSON file
        private void SaveData<T>(string filePath, List<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // Load data from JSON file
        private List<T>? LoadData<T>(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }
}
