//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;

namespace Tarteeb.Api.Models.Processings.UserProfiles
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string GitHubUsername { get; set; }
        public string TelegramUsername { get; set; }
        public Guid TeamId { get; set; }
    }
}
