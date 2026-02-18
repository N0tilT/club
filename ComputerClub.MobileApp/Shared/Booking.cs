using System;

namespace ComputerClub.Shared
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Active,
        Completed,
        Cancelled
    }
}