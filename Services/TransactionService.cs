﻿using ExpenseTracker.Models;
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

        private const string CashInflowFileName = "CashInflows.json";
        private const string CashOutflowFileName = "CashOutflows.json";
        private const string DebtFileName = "Debts.json";

        private readonly string CashInflowFilePath = Path.Combine(BaseDirectory, CashInflowFileName);
        private readonly string CashOutflowFilePath = Path.Combine(BaseDirectory, CashOutflowFileName);
        private readonly string DebtFilePath = Path.Combine(BaseDirectory, DebtFileName);

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

        public string Category { get; set; } = string.Empty;

        public event Action OnChange;

        public decimal CurrentBalance =>
            GetTotalCashInflow() - GetTotalCashOutflow();

        // Add CashInflow transaction
        public void AddCashInflow(CashInflow transaction)
        {
            _cashInflows.Add(transaction);
            SaveData(CashInflowFilePath, _cashInflows);
            NotifyStateChanged();
        }

        // Add CashOutflow transaction
        public void AddCashOutflow(CashOutflow transaction)
        {
            _cashOutflows.Add(transaction);
            SaveData(CashOutflowFilePath, _cashOutflows);
            NotifyStateChanged();
        }

        // Add Debt transaction
        public void AddDebt(Debt transaction)
        {
            _debts.Add(transaction);
            SaveData(DebtFilePath, _debts);
            NotifyStateChanged();
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

        // Get all CashInflow transactions
        public List<CashInflow> GetAllCashInflows() => _cashInflows;

        // Get all CashOutflow transactions
        public List<CashOutflow> GetAllCashOutflows() => _cashOutflows;

        // Get all Debt transactions
        public List<Debt> GetAllDebts() => _debts;

        // Get Total Cash Inflow
        public decimal GetTotalCashInflow() => _cashInflows.Sum(t => t.Amount);

        // Get Total Cash Outflow
        public decimal GetTotalCashOutflow() => _cashOutflows.Sum(t => t.Amount);

        // Get Total Debt
        public decimal GetTotalDebt() => _debts.Sum(d => d.Amount);

        // Get Available Balance (Cash Inflow - Cash Outflow + Debt)
        public decimal GetAvailableBalance() =>
            GetTotalCashInflow() - GetTotalCashOutflow() + GetTotalDebt();

        // Method to notify UI updates
        private void NotifyStateChanged() => OnChange?.Invoke();

        // Save data to JSON file
        private void SaveData<T>(string filePath, List<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // Load data from JSON file
        private List<T> LoadData<T>(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }
}
