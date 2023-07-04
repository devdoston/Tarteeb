﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
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
            catch(InvalidUserProfileProcessingException invalidUserProfileException)
            {
                throw CreateAndLogValidationException(invalidUserProfileException);
            }
            catch(NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
        }

        private UserProfileProcessingValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userProfileValidationException = new UserProfileProcessingValidationException(exception);
            this.loggingBroker.LogError(userProfileValidationException);

            return userProfileValidationException;
        }
    }
}
