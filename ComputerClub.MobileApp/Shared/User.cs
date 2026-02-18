using System;
using System.Collections.Generic;

namespace ComputerClub.Shared
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public decimal Balance { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
    }

    public enum UserRole
    {
        Client,
        Admin
    }
}