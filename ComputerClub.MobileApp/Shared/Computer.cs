using System;

namespace ComputerClub.Shared
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Row { get; set; }
        public int Place { get; set; }
        public ComputerStatus Status { get; set; }
        public decimal PricePerHour { get; set; }
        public string Specifications { get; set; }
        public string Notes { get; set; }
    }

    public enum ComputerStatus
    {
        Available,
        Occupied,
        Reserved,
        Maintenance,
        Broken
    }
}