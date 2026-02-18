using System;

namespace ComputerClub.Shared
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public int? BookingId { get; set; }
        public int? TournamentId { get; set; }
        public string TransactionId { get; set; }
    }

    public enum PaymentType
    {
        Booking,
        TournamentEntry,
        BalanceTopUp
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Online,
        Balance,
        SBP
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Refunded,
        Failed
    }
}