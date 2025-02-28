﻿namespace back_end.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Username { get; set; }
        public bool UserType { get; set; }
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public string? CPF { get; set; } 
        public string? DateOfBirth { get; set; } 
        public string? Email { get; set; } 
        public string? Address { get; set; } 
        public string? Number { get; set; }
        public string? Neighborhood { get; set; } 
        public string? City { get; set; } 
        public string? Password { get; set; } 
    }
}
