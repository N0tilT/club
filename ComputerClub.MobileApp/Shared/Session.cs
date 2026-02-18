using System;

namespace ComputerClub.Shared
{
    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public int? BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public SessionStatus Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public string StartedByAdmin { get; set; }
        public string Notes { get; set; }
    }

    public enum SessionStatus
    {
        Active,
        Completed,
        Interrupted
    }
}