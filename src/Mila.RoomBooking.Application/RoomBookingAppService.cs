using Mila.RoomBooking.Localization;
using Volo.Abp.Application.Services;

namespace Mila.RoomBooking;

/* Inherit your application services from this class.
 */
public abstract class RoomBookingAppService : ApplicationService
{
    protected RoomBookingAppService()
    {
        LocalizationResource = typeof(RoomBookingResource);
    }
}
