//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tarteeb.Api.Services.Processings.Users;

namespace Tarteeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmailsController : RESTFulController
    {
        private readonly IUserProcessingService userProcessingService;

        public EmailsController(IUserProcessingService userProcessingService) =>
            this.userProcessingService = userProcessingService;

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<Guid>> VerifyUserEmailById(Guid userId)
        {
            Guid verifiedId = await this.userProcessingService.VerifyUserByIdAsync(userId);

            return Ok(verifiedId);
        }
    }
}
