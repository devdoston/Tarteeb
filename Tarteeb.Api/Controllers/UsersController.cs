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
    public class UsersController : RESTFulController
    {
        private readonly IUserProcessingService userProcessingService;

        public UsersController(IUserProcessingService userProcessingService) =>
            this.userProcessingService = userProcessingService;

        [HttpGet]
        public async ValueTask<ActionResult<Guid>> VerifyUserByIdAsync(Guid userId)
        {
            Guid verifiedId = await this.userProcessingService.VerifyUserByIdAsync(userId);

            return Ok(verifiedId);
        }

        [HttpPost]
        public async ValueTask<ActionResult<Guid>> ActivateUserByIdAsync([FromBody] Guid userId)
        {
            Guid activatedId = await this.userProcessingService.ActivateUserByIdAsync(userId);

            return Ok(activatedId);
        }
    }
}
