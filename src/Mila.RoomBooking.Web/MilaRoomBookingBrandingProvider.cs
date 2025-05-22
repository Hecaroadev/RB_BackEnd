using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace Mila.RoomBooking.Web
{
    public class MilaRoomBookingBrandingProvider : IBrandingProvider, ISingletonDependency
    {
        public string AppName => "Room Booking";
        public string LogoUrl => "/images/ksu-logo.png"; // Path to your custom logo
        public string LogoReverseUrl => "/images/custom-logo-reverse.png"; // Optional: for dark backgrounds
    }
}
