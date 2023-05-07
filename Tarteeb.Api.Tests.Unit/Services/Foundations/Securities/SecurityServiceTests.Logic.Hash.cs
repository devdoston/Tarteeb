//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Securities
{
    public partial class SecurityServiceTests
    {
        [Fact]
        public void ShouldHashPassword()
        {
            //given
            string randomPassword = CreateRandomPassword();
            string inputPassword = randomPassword;
            string hashedPassword = CreateRandomPassword();
            string expectedPassword = hashedPassword.DeepClone();

            this.tokenBrokerMock.Setup(broker =>
                broker.HashToken(inputPassword)).Returns(hashedPassword);

            //when
            string actualPassword = securityService.HashPassword(inputPassword);

            //then
            actualPassword.Should().BeEquivalentTo(expectedPassword);

            this.tokenBrokerMock.Verify(broker =>
                broker.HashToken(inputPassword), Times.Once);

            this.tokenBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
