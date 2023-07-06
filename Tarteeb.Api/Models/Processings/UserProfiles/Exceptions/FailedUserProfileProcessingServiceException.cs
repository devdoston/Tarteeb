//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class FailedUserProfileProcessingServiceException : Xeption
    {
        public FailedUserProfileProcessingServiceException(Exception innerException)
            : base(message: "Failed user profile service error occured, please contact support.", innerException)
        { }
    }
}
