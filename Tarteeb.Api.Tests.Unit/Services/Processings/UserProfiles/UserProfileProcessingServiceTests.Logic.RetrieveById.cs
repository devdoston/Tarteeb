//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveUserByIdAsync()
        {
            // given
            Guid randomUserProfileId = Guid.NewGuid();
            Guid inputUserProfileId = randomUserProfileId;
            User randomUser = CreateRandomUser();
            User storedUser = randomUser;
            UserProfile mappedUserProfile = PopulateUserProfile(storedUser);
            UserProfile expectedUserProfile = mappedUserProfile.DeepClone();

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileId))
                    .ReturnsAsync(storedUser);

            // when
            UserProfile actualUserProfile = 
                await this.userProfileProcessingService
                    .RetrieveUserProfileByIdAsync(inputUserProfileId);

            // then
            actualUserProfile.Should().BeEquivalentTo(expectedUserProfile);
            
            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileId), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
