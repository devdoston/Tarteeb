//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Orchestrations.UserTokens;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationServiceTests
    {
        [Fact]
        public void ShoudCreateUserToken()
        {
            // given
            string randomString = GetRandomString();
            string token = randomString;
            string hashPassword = GetRandomString();
            User randomUser = CreateRandomUser();
            User existingUser = randomUser;
            User storageUser = existingUser.DeepClone();
            storageUser.Password = hashPassword;

            IQueryable<User> randomUsers =
                CreateRandomUsersIncluding(storageUser);

            IQueryable<User> retrievedUsers = randomUsers;

            UserToken expectedUserToken = new UserToken
            {
                UserId = existingUser.Id,
                Token = token
            };

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers())
                    .Returns(retrievedUsers);

            this.securityServiceMock.Setup(service =>
                service.HashPassword(existingUser.Password))
                    .Returns(hashPassword);

            this.securityServiceMock.Setup(service =>
                service.CreateToken(storageUser))
                    .Returns(token);

            // when
            UserToken actualUserToken = this.userSecurityOrchestrationService
                .CreateUserToken(existingUser.Email, existingUser.Password);

            // then
            actualUserToken.Should().BeEquivalentTo(expectedUserToken);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.securityServiceMock.Verify(service =>
                service.HashPassword(existingUser.Password), Times.Once);

            this.securityServiceMock.Verify(service =>
                service.CreateToken(storageUser), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.securityServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
