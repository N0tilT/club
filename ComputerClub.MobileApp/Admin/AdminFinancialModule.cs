using ComputerClub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerClub.Admin
{
    // ========== ФИНАНСОВАЯ АНАЛИТИКА ==========
    public class AdminFinancialModule
    {
        private List<Payment> payments = new List<Payment>();
        private List<Booking> bookings = new List<Booking>();
        private List<Tournament> tournaments = new List<Tournament>();

        public AdminFinancialModule()
        {
            // Инициализация тестовых платежей
            payments.Add(new Payment
            {
                Id = 1,
                UserId = 1,
                Amount = 450,
                PaymentDate = DateTime.Now.AddDays(-1),
                Type = PaymentType.Booking,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Paid
            });
            
            payments.Add(new Payment
            {
                Id = 2,
                UserId = 2,
                Amount = 300,
                PaymentDate = DateTime.Now.AddDays(-2),
                Type = PaymentType.TournamentEntry,
                Method = PaymentMethod.Cash,
                Status = PaymentStatus.Paid
            });
            
            payments.Add(new Payment
            {
                Id = 3,
                UserId = 1,
                Amount = 500,
                PaymentDate = DateTime.Now.AddDays(-3),
                Type = PaymentType.BalanceTopUp,
                Method = PaymentMethod.Online,
                Status = PaymentStatus.Paid
            });
        }

        // Функция 1: Полный финансовый отчет за период
        public FinancialReport GetFullReport(DateTime startDate, DateTime endDate)
        {
            var periodPayments = payments
                .Where(p => p.PaymentDate >= startDate && 
                       p.PaymentDate <= endDate && 
                       p.Status == PaymentStatus.Paid)
                .ToList();

            var report = new FinancialReport
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalRevenue = periodPayments.Sum(p => p.Amount),
                TotalPayments = periodPayments.Count
            };

            // Разбивка по типам платежей
            report.ByPaymentType = new Dictionary<string, decimal>
            {
                { "Бронирования", periodPayments.Where(p => p.Type == PaymentType.Booking).Sum(p => p.Amount) },
                { "Турниры", periodPayments.Where(p => p.Type == PaymentType.TournamentEntry).Sum(p => p.Amount) },
                { "Пополнения", periodPayments.Where(p => p.Type == PaymentType.BalanceTopUp).Sum(p => p.Amount) }
            };

            // Разбивка по методам оплаты
            report.ByPaymentMethod = new Dictionary<string, decimal>
            {
                { "Наличные", periodPayments.Where(p => p.Method == PaymentMethod.Cash).Sum(p => p.Amount) },
                { "Карта", periodPayments.Where(p => p.Method == PaymentMethod.Card).Sum(p => p.Amount) },
                { "Онлайн", periodPayments.Where(p => p.Method == PaymentMethod.Online).Sum(p => p.Amount) },
                { "СБП", periodPayments.Where(p => p.Method == PaymentMethod.SBP).Sum(p => p.Amount) }
            };

            // По дням
            report.DailyRevenue = periodPayments
                .GroupBy(p => p.PaymentDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            return report;
        }

        // Функция 2: Аналитика по бронированиям
        public BookingsAnalytics GetBookingsAnalytics(DateTime startDate, DateTime endDate)
        {
            var periodBookings = bookings
                .Where(b => b.StartTime >= startDate && b.StartTime <= endDate)
                .ToList();

            var analytics = new BookingsAnalytics
            {
                TotalBookings = periodBookings.Count,
                ConfirmedBookings = periodBookings.Count(b => b.Status == BookingStatus.Confirmed),
                CompletedBookings = periodBookings.Count(b => b.Status == BookingStatus.Completed),
                CancelledBookings = periodBookings.Count(b => b.Status == BookingStatus.Cancelled),
                TotalRevenue = periodBookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid)
                    .Sum(b => b.TotalPrice)
            };

            // Средняя длительность
            if (periodBookings.Any())
            {
                analytics.AverageDuration = periodBookings
                    .Average(b => (b.EndTime - b.StartTime).TotalHours);
            }

            return analytics;
        }

        // Функция 3: Аналитика по турнирам
        public TournamentsAnalytics GetTournamentsAnalytics(DateTime startDate, DateTime endDate)
        {
            var periodTournaments = tournaments
                .Where(t => t.StartDate >= startDate && t.StartDate <= endDate)
                .ToList();

            var analytics = new TournamentsAnalytics
            {
                TotalTournaments = periodTournaments.Count,
                TotalParticipants = periodTournaments.Sum(t => t.CurrentParticipants),
                TotalRevenue = periodTournaments.Sum(t => t.CurrentParticipants * t.EntryFee)
            };

            if (periodTournaments.Any())
            {
                analytics.AverageParticipants = periodTournaments.Average(t => t.CurrentParticipants);
            }

            return analytics;
        }

        // Функция 4: Детальная статистика за день
        public DailyStatistics GetDailyStatistics(DateTime date)
        {
            var dayPayments = payments
                .Where(p => p.PaymentDate.Date == date.Date && p.Status == PaymentStatus.Paid)
                .ToList();

            var stats = new DailyStatistics
            {
                Date = date,
                TotalRevenue = dayPayments.Sum(p => p.Amount),
                TransactionsCount = dayPayments.Count,
                AverageCheck = dayPayments.Any() ? dayPayments.Average(p => p.Amount) : 0
            };

            // По часам
            stats.ByHour = new Dictionary<int, decimal>();
            for (int hour = 10; hour <= 23; hour++)
            {
                stats.ByHour[hour] = dayPayments
                    .Where(p => p.PaymentDate.Hour == hour)
                    .Sum(p => p.Amount);
            }

            return stats;
        }
    }

    // Классы для отчетов
    public class FinancialReport
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalPayments { get; set; }
        public Dictionary<string, decimal> ByPaymentType { get; set; } = new();
        public Dictionary<string, decimal> ByPaymentMethod { get; set; } = new();
        public Dictionary<DateTime, decimal> DailyRevenue { get; set; } = new();
    }

    public class BookingsAnalytics
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageDuration { get; set; }
    }

    public class TournamentsAnalytics
    {
        public int TotalTournaments { get; set; }
        public int TotalParticipants { get; set; }
        public double AverageParticipants { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyStatistics
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TransactionsCount { get; set; }
        public decimal AverageCheck { get; set; }
        public Dictionary<int, decimal> ByHour { get; set; } = new();
    }
}