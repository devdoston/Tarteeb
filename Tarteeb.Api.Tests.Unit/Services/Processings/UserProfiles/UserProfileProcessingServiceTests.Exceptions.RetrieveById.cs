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
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(UserDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var expectedUserProfileValidationException =
                new UserProfileProcessingDependencyValidationException(dependencyException);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask = 
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileGuid);

            UserProfileProcessingDependencyValidationException actualUserProfileProcessingDependencyValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyValidationException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserProfileValidationException);

            this.userServiceMock.Verify(service =>
                service.RemoveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileValidationException))),Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
