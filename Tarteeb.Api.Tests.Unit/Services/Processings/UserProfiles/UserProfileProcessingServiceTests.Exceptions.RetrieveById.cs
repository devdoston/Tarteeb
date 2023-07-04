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
            Xeption dependencyValidationException)
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var expectedUserProfileValidationException =
                new UserProfileProcessingDependencyValidationException(dependencyValidationException);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyValidationException);

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
                service.RetrieveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileValidationException))),Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var userProfileProcessingDependencyException =
                new UserProfileProcessingDependencyException(dependencyException);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask =
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileGuid);

            UserProfileProcessingDependencyException actualUserProfileProcessingDependencyException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcessingDependencyException.Should()
                .BeEquivalentTo(userProfileProcessingDependencyException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    userProfileProcessingDependencyException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
