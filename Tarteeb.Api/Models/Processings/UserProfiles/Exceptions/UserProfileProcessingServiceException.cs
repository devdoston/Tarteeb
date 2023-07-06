//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class UserProfileProcessingServiceException : Xeption
    {
        public UserProfileProcessingServiceException(Xeption innerException)
            : base(message: "User profile service error occured, contact support.",innerException)
        { }
    }
}
