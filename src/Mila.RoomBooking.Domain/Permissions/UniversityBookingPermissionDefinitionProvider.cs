using Mila.RoomBooking.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace UniversityBooking.Permissions
{
  public class UniversityBookingPermissionDefinitionProvider : PermissionDefinitionProvider
  {
    public override void Define(IPermissionDefinitionContext context)
    {
      var bookingGroup = context.AddGroup(UniversityBookingPermissions.GroupName, L("Permission:UniversityBooking"));

      // Room permissions
      var roomPermission = bookingGroup.AddPermission(UniversityBookingPermissions.Room.Default, L("Permission:Rooms"));
      roomPermission.AddChild(UniversityBookingPermissions.Room.Create, L("Permission:Rooms.Create"));
      roomPermission.AddChild(UniversityBookingPermissions.Room.Update, L("Permission:Rooms.Update"));
      roomPermission.AddChild(UniversityBookingPermissions.Room.Delete, L("Permission:Rooms.Delete"));
      roomPermission.AddChild(UniversityBookingPermissions.Room.ManageSettings, L("Permission:Rooms.ManageSettings"));

      // Booking Request permissions
      var bookingRequestPermission = bookingGroup.AddPermission(
        UniversityBookingPermissions.BookingRequest.Default, 
        L("Permission:BookingRequests")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Create, 
        L("Permission:BookingRequests.Create")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.View, 
        L("Permission:BookingRequests.View")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Manage, 
        L("Permission:BookingRequests.Manage")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Cancel, 
        L("Permission:BookingRequests.Cancel")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Review, 
        L("Permission:BookingRequests.Review")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Approve, 
        L("Permission:BookingRequests.Approve")
      );
      bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.Reject, 
        L("Permission:BookingRequests.Reject")
      );
      
      // Role-specific approval permissions
      var approveClassroomsPermission = bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.ApproveClassrooms, 
        L("Permission:BookingRequests.ApproveClassrooms")
      );
      var approveLabsPermission = bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.ApproveLabs, 
        L("Permission:BookingRequests.ApproveLabs")
      );
      var approveConferenceRoomsPermission = bookingRequestPermission.AddChild(
        UniversityBookingPermissions.BookingRequest.ApproveConferenceRooms, 
        L("Permission:BookingRequests.ApproveConferenceRooms")
      );
      
      // Booking permissions
      var bookingPermission = bookingGroup.AddPermission(
        UniversityBookingPermissions.Booking.Default, 
        L("Permission:Bookings")
      );
      bookingPermission.AddChild(
        UniversityBookingPermissions.Booking.View, 
        L("Permission:Bookings.View")
      );
      bookingPermission.AddChild(
        UniversityBookingPermissions.Booking.Create, 
        L("Permission:Bookings.Create")
      );
      bookingPermission.AddChild(
        UniversityBookingPermissions.Booking.Cancel, 
        L("Permission:Bookings.Cancel")
      );
      bookingPermission.AddChild(
        UniversityBookingPermissions.Booking.CancelOwn, 
        L("Permission:Bookings.CancelOwn")
      );
      bookingPermission.AddChild(
        UniversityBookingPermissions.Booking.CreateRecurring, 
        L("Permission:Bookings.CreateRecurring")
      );
      
      // Reports permissions
      var reportsPermission = bookingGroup.AddPermission(
        UniversityBookingPermissions.Reports.Default, 
        L("Permission:Reports")
      );
      reportsPermission.AddChild(
        UniversityBookingPermissions.Reports.ViewUsageReports, 
        L("Permission:Reports.ViewUsageReports")
      );
      reportsPermission.AddChild(
        UniversityBookingPermissions.Reports.ExportReports, 
        L("Permission:Reports.ExportReports")
      );
      reportsPermission.AddChild(
        UniversityBookingPermissions.Reports.ViewAnalytics, 
        L("Permission:Reports.ViewAnalytics")
      );
    }

    private static LocalizableString L(string name)
    {
      return LocalizableString.Create<RoomBookingResource>(name);
    }
  }
}
