//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Orchestrations.UserTokens;

namespace Tarteeb.Api.Services.Orchestrations
{
    public interface IUserSecurityOrchestrationService
    {
        ValueTask<User> CreateUserAccountAsync(User user, string requestUrl);
        UserToken CreateUserToken(string email, string password);
    }
}
