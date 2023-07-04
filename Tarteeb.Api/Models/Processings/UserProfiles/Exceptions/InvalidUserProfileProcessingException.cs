//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class InvalidUserProfileProcessingException : Xeption
    {
        public InvalidUserProfileProcessingException()
            : base(message: "User profile is invalid.")
        {
            
        }
    }
}
