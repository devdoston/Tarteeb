//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Data;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService
    {
        private void ValidateUserProfileId(Guid userProfileId) =>
            Validate((Rule: IsInvalid(userProfileId), Parameter: nameof(UserProfile.Id)));

        private dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };


        private static void Validate(params(dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserProfileException = new InvalidUserProfileException();

            foreach((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserProfileException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserProfileException.ThrowIfContainsErrors();
        }
    }
}
