using Microsoft.Extensions.Localization;
using Mila.RoomBooking.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Mila.RoomBooking;

[Dependency(ReplaceServices = true)]
public class RoomBookingBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<RoomBookingResource> _localizer;

    public RoomBookingBrandingProvider(IStringLocalizer<RoomBookingResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
