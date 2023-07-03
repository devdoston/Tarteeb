//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xeptions;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService
    {
        private delegate ValueTask<UserProfile> ReturningUserProfileFunction();

        private async ValueTask<UserProfile> TryCatch(ReturningUserProfileFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch(InvalidUserProfileException invalidUserProfileException)
            {
                throw CreateAndLogValidationException(invalidUserProfileException);
            }
        }

        private UserProfileValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userProfileValidationException = new UserProfileValidationException(exception);
            this.loggingBroker.LogError(userProfileValidationException);

            return userProfileValidationException;
        }
    }
}
