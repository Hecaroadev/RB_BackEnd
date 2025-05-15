using System;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Volo.Abp.Users;

namespace Mila.RoomBooking
{
    public static class MockCurrentUserExtensions
    {
        public static void AddCurrentUser(this IServiceCollection services)
        {
            var currentUser = Substitute.For<ICurrentUser>();

            // Setup a default test user
            var userId = Guid.NewGuid();
            currentUser.Id.Returns(userId);
            currentUser.IsAuthenticated.Returns(true);
            currentUser.UserName.Returns("testuser");
            currentUser.Email.Returns("test@example.com");

            // Return null for FindClaim
            currentUser.FindClaim(Arg.Any<string>()).Returns((Claim)null);

            // Return empty array for FindClaims
            currentUser.FindClaims(Arg.Any<string>()).Returns(new Claim[0]);

            services.AddSingleton(currentUser);
        }
    }
}
