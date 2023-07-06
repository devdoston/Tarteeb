//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Orchestrations.UserTokens;
using Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnCreateIfEmailOrPasswordAreInvalidAndLogItAsync()
        {
            //given
            string invalidEmail = string.Empty;
            string invalidPassword = string.Empty;
            var invalidUserCreadentialOrchestrationException = new InvalidUserCredentialOrchestrationException();

            invalidUserCreadentialOrchestrationException.AddData(
                key: nameof(User.Email),
                values: "Value is required");

            invalidUserCreadentialOrchestrationException.AddData(
                key: nameof(User.Password),
                values: "Value is required");

            var expectedUserOrchestrationValidationException =
                new UserTokenOrchestrationValidationException(invalidUserCreadentialOrchestrationException);

            //when
            Action createUserTokenAction = () =>
                this.userSecurityOrchestrationService.CreateUserToken(invalidEmail, invalidPassword);

            UserTokenOrchestrationValidationException actualUserTokenOrchestrationValidationException =
                 Assert.Throws<UserTokenOrchestrationValidationException>(createUserTokenAction);

            //then
            actualUserTokenOrchestrationValidationException.Should().BeEquivalentTo(
               expectedUserOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationValidationException))), Times.Once);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Never);

            this.securityServiceMock.Verify(service =>
                service.CreateToken(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.securityServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowValidationExceptionOnCreateIfUserDoesntExistsAndLogItAsync()
        {
            //given
            string email = GetRandomString();
            string password = GetRandomString();
            string hashedPassword = GetRandomString();
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> retrievedUsers = randomUsers;
            var notFoundUserException = new NotFoundUserException();

            var expectedUserOrchestrationValidationException =
                new UserTokenOrchestrationValidationException(notFoundUserException);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers())
                    .Returns(retrievedUsers);

            this.securityServiceMock.Setup(service =>
                service.HashPassword(password))
                    .Returns(hashedPassword);

            //when
            Action createUserTokenAction = () =>
                this.userSecurityOrchestrationService.CreateUserToken(email, password);

            UserTokenOrchestrationValidationException actualUserTokenOrchestrationValidationException =
                 Assert.Throws<UserTokenOrchestrationValidationException>(createUserTokenAction);

            //then
            actualUserTokenOrchestrationValidationException.Should().BeEquivalentTo(
               expectedUserOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationValidationException))), Times.Once);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.securityServiceMock.Verify(service =>
                service.HashPassword(password), Times.Once);

            this.securityServiceMock.Verify(service =>
                service.CreateToken(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.securityServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowValidationExceptionOnCreateIfUserIsNotActiveOrVerifiedAndLogItAsync()
        {
            // given
            string hashedPassword = GetRandomString();
            User randomUser = CreateRandomUser();
            User existingUser = randomUser;
            existingUser.IsVerified = false;
            existingUser.IsActive = false;
            var storageUser = existingUser.DeepClone();
            storageUser.Password = hashedPassword;
            IQueryable<User> storageUsers = CreateRandomUsersIncluding(storageUser);

            var invalidUserCredentialOrchestrationException =
                new InvalidUserCredentialOrchestrationException();

            invalidUserCredentialOrchestrationException.AddData(
                key: nameof(User.IsActive),
                values: "Status is not true");

            invalidUserCredentialOrchestrationException.AddData(
                key: nameof(User.IsVerified),
                values: "Status is not true");

            var expectedUserTokenOrchestrationValidationException =
                new UserTokenOrchestrationValidationException(invalidUserCredentialOrchestrationException);

            this.userServiceMock.Setup(broker =>
                broker.RetrieveAllUsers()).Returns(storageUsers);

            this.securityServiceMock.Setup(broker =>
                broker.HashPassword(existingUser.Password)).Returns(hashedPassword);

            // when
            Action createUserTokenTask = () =>
                this.userSecurityOrchestrationService.CreateUserToken(existingUser.Email, existingUser.Password);

            UserTokenOrchestrationValidationException actualUserTokenValidationException =
                Assert.Throws<UserTokenOrchestrationValidationException>(createUserTokenTask);

            // then
            actualUserTokenValidationException.Should().BeEquivalentTo(
                expectedUserTokenOrchestrationValidationException);

            this.userServiceMock.Verify(broker => broker.RetrieveAllUsers(), Times.Once);

            this.userServiceMock.Verify(broker =>
                broker.RetrieveAllUsers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserTokenOrchestrationValidationException))),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
