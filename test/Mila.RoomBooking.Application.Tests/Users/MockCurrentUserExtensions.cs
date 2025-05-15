using System;
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
            currentUser.FindClaim(Arg.Any<string>()).Returns((string)null);
            currentUser.FindClaims(Arg.Any<string>()).Returns(Array.Empty<string>());

            services.AddSingleton(currentUser);
        }
    }
}
