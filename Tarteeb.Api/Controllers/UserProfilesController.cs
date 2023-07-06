//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Tarteeb.Api.Services.Processings.UserProfiles;

namespace Tarteeb.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : RESTFulController
    {
        private readonly IUserProfileProcessingService userProfileProcessingService;

        public UserProfilesController(IUserProfileProcessingService userProfileProcessingService) =>
            this.userProfileProcessingService = userProfileProcessingService;

        [HttpGet]
        public async ValueTask<ActionResult<UserProfile>> GetUserProfileByIdAsync(Guid userProfileId)
        {
            try
            {
                return await this.userProfileProcessingService.RetrieveUserProfileByIdAsync(userProfileId);
            }
            catch (UserProfileProcessingValidationException userProfileProcessingValidationException)
            {
                return BadRequest(userProfileProcessingValidationException.InnerException);
            }
            catch (UserProfileProcessingDependencyException userProfileProcessingDependencyException)
            {
                return InternalServerError(userProfileProcessingDependencyException.InnerException);
            }
            catch (UserProfileProcessingServiceException userProfileProcessingServiceException)
            {
                return InternalServerError(userProfileProcessingServiceException.InnerException);
            }
        }
    }
}
