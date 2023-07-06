//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class UserProfileProcessingDependencyException : Xeption
    {
        public UserProfileProcessingDependencyException(Xeption innerException)
            : base(message: "User profile dependency error occurred, contact support.", innerException)
        { }
    }
}
