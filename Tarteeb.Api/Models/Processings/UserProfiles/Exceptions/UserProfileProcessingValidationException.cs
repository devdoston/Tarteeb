//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class UserProfileProcessingValidationException : Xeption
    {
        public UserProfileProcessingValidationException(Xeption innerException)
             : base(message: "User profile validation error occured, fix the errors and try again.")
        { }
    }
}
