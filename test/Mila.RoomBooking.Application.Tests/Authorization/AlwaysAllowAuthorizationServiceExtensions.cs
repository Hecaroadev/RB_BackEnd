using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mila.RoomBooking
{
    public static class AlwaysAllowAuthorizationServiceExtensions
    {
        public static void AddAlwaysAllowAuthorization(this IServiceCollection services)
        {
            var authorizationService = Substitute.For<IAuthorizationService>();

            authorizationService
                .AuthorizeAsync(
                    Arg.Any<ClaimsPrincipal>(),
                    Arg.Any<object>(),
                    Arg.Any<string>())
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            authorizationService
                .AuthorizeAsync(
                    Arg.Any<ClaimsPrincipal>(),
                    Arg.Any<object>(),
                    Arg.Any<IAuthorizationRequirement>())
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            services.AddSingleton(authorizationService);
        }
    }
}
