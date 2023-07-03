//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Services.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public class UserProfileProcessingService : IUserProfileProcessingService
    {
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public UserProfileProcessingService(
            IUserService userService,
            ILoggingBroker loggingBroker)
        {
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserProfile> RetrieveUserProfileByIdAsync(Guid userProfileId)
        {
            throw new NotImplementedException();
        }
    }
}
