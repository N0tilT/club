using System;
using System.Collections.Generic;

namespace ComputerClub.Reports
{
    // Классы для финансовых отчетов
    public class FinancialReport
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public Dictionary<string, decimal> RevenueByType { get; set; }
        public Dictionary<string, decimal> RevenueByMethod { get; set; }
        public List<PaymentDetail> Transactions { get; set; }
    }

    public class PaymentDetail
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
    }

    public class DailyStat
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public int NewUsers { get; set; }
    }

    public class ComputerStat
    {
        public int ComputerId { get; set; }
        public string ComputerName { get; set; }
        public int SessionsCount { get; set; }
        public decimal TotalHours { get; set; }
        public decimal Revenue { get; set; }
        public double UtilizationPercent { get; set; }
    }
}