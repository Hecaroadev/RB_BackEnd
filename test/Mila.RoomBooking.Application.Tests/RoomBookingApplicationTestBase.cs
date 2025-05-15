using Volo.Abp.Modularity;
using System;
using Microsoft.Extensions.DependencyInjection;
using Mila.RoomBooking;
using Volo.Abp;

namespace Mila.RoomBooking
{
    public abstract class RoomBookingApplicationTestBase : AbpTestBaseWithServiceProvider

    {
        protected T GetRequiredService<T>()
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
    }
}

public abstract class RoomBookingApplicationTestBase<TStartupModule> : RoomBookingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
