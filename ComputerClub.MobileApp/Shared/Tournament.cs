using System;
using System.Collections.Generic;

namespace ComputerClub.Shared
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Game { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public decimal EntryFee { get; set; }
        public decimal PrizePool { get; set; }
        public TournamentStatus Status { get; set; }
    }

    public class TournamentRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TournamentId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public RegistrationStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public enum TournamentStatus
    {
        Draft,
        Registration,
        InProgress,
        Completed,
        Cancelled
    }

    public enum RegistrationStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}