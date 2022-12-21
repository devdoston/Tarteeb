﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Moq;
using Tarteeb.Api.Models.Teams;
using Tarteeb.Api.Models.Teams.Exceptions;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Teams
{
    public partial class TeamServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeamIsNullAndLogItAsync()
        {
            // given
            Team nullTeam = null;
            var nullTeamException = new NullTeamException();

            var expectedTeamValidationException =
                new TeamValidationException(nullTeamException);

            // when
            ValueTask<Team> modifyTeamTask =
                this.teamService.ModifyTeamAsync(nullTeam);

            TeamValidationException actualTeamValidationException =
                await Assert.ThrowsAsync<TeamValidationException>(
                    modifyTeamTask.AsTask);

            // then
            actualTeamValidationException.Should().BeEquivalentTo(expectedTeamValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeamAsync(It.IsAny<Team>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeamIsInvalidAndLogItAsync(string invalidString)
        {
            // given 
            var invalidTeam = new Team
            {
                TeamName = invalidString
            };

            var invalidTeamException =
                new InvalidTeamException();

            invalidTeamException.AddData(
                key: nameof(Team.Id),
                values: "Id is required");

            invalidTeamException.AddData(
                key: nameof(Team.TeamName),
                values: "Text is required");

            invalidTeamException.AddData(
                key: nameof(Team.CreatedDate),
                values: "Value is required");

            invalidTeamException.AddData(
                key: nameof(Team.UpdatedDate),
                    values: new[]
                    {
                        "Value is required",
                        "Date is not recent.",
                        $"Date is the same as {nameof(Team.CreatedDate)}"
                    }
                );

            var expectedTeamValidationException =
                new TeamValidationException(invalidTeamException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<Team> modifyTeamTask =
                this.teamService.ModifyTeamAsync(invalidTeam);

            TeamValidationException actualTeamValidationException =
                await Assert.ThrowsAsync<TeamValidationException>(
                    modifyTeamTask.AsTask);

            //then
            actualTeamValidationException.Should()
                .BeEquivalentTo(expectedTeamValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeamAsync(It.IsAny<Team>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Team randomTeam = CreateRandomTeam(randomDateTime);
            Team invalidTeam = randomTeam;
            var invalidTeamException = new InvalidTeamException();

            invalidTeamException.AddData(
            key: nameof(Team.UpdatedDate),
                values: $"Date is the same as {nameof(Team.CreatedDate)}");

            var expectedTeamValidationException =
                new TeamValidationException(invalidTeamException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTime);

            // when
            ValueTask<Team> modifyTeamTask =
                this.teamService.ModifyTeamAsync(invalidTeam);

            TeamValidationException actualTeamValidationException =
                await Assert.ThrowsAsync<TeamValidationException>(
                    modifyTeamTask.AsTask);

            // then
            actualTeamValidationException.Should()
                .BeEquivalentTo(expectedTeamValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeamByIdAsync(invalidTeam.Id), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Team randomTeam = CreateRandomTeam(dateTime);
            Team inputTeam = randomTeam;
            inputTeam.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidTeamException =
                new InvalidTeamException();
            invalidTeamException.AddData(
                key: nameof(Team.UpdatedDate),
                values: "Date is not recent.");

            var expectedTeamValidatonException =
                new TeamValidationException(invalidTeamException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                .Returns(dateTime);

            // when
            ValueTask<Team> modifyTeamTask =
                this.teamService.ModifyTeamAsync(inputTeam);

            TeamValidationException actualTeamValidationException =
                await Assert.ThrowsAsync<TeamValidationException>(
                    modifyTeamTask.AsTask);

            // then
            actualTeamValidationException.Should().BeEquivalentTo(expectedTeamValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeamByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeamDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            Team randomTeam = CreateRandomTeam(dateTime);
            Team nonExistTeam = randomTeam;
            nonExistTeam.CreatedDate = dateTime.AddMinutes(randomNegativeMinutes);
            Team nullTeam = null;

            var notFoundTeamException =
                new NotFoundTeamException(nonExistTeam.Id);

            var expectedTeamValidationException =
                new TeamValidationException(notFoundTeamException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeamByIdAsync(nonExistTeam.Id)).ReturnsAsync(nullTeam);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            // when 
            ValueTask<Team> modifyTeamTask =
                this.teamService.ModifyTeamAsync(nonExistTeam);

            TeamValidationException actualTeamValidationException =
               await Assert.ThrowsAsync<TeamValidationException>(modifyTeamTask.AsTask);

            // then
            actualTeamValidationException.Should().BeEquivalentTo(expectedTeamValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeamByIdAsync(nonExistTeam.Id),Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidationException))),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
