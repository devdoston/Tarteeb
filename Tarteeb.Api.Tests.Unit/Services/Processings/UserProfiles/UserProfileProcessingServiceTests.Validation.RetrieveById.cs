//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowVlidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidUserProfileId = Guid.Empty;
            var invalidUserProfileException = new InvalidUserProfileException();

            invalidUserProfileException.AddData(
                key: nameof(UserProfile.Id),
                values: "Id is required");

            var expectedUserProfileValidationException =
                new UserProfileValidationException(invalidUserProfileException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask = 
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(invalidUserProfileId);

            UserProfileValidationException actualUserProfileValidationException =
                await Assert.ThrowsAsync<UserProfileValidationException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileValidationException.Should().BeEquivalentTo(expectedUserProfileValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileValidationException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
