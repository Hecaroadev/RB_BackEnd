using Mila.RoomBooking.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Mila.RoomBooking.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class RoomBookingController : AbpControllerBase
{
    protected RoomBookingController()
    {
        LocalizationResource = typeof(RoomBookingResource);
    }
}
