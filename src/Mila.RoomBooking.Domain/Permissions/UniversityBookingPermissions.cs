using Volo.Abp.Reflection;

namespace UniversityBooking.Permissions
{
  public static class UniversityBookingPermissions
  {
    public const string GroupName = "UniversityBooking";

    public static class BookingRequest
    {
      public const string Default = GroupName + ".BookingRequest";
      public const string Create = Default + ".Create"; // Submit a booking request
      public const string View = Default + ".View"; // View all requests or just own
      public const string Manage = Default + ".Manage"; // General management permission
      public const string Cancel = Default + ".Cancel"; // Cancel own request
      public const string Review = Default + ".Review"; // Review all pending requests
      public const string Approve = Default + ".Approve"; // Approve a booking request
      public const string Reject = Default + ".Reject"; // Reject a booking request

      // Role-based approvals
      public const string ApproveClassrooms = Default + ".ApproveClassrooms";
      public const string ApproveLabs = Default + ".ApproveLabs";
      public const string ApproveConferenceRooms = Default + ".ApproveConferenceRooms";
    }

    public static class Room
    {
      public const string Default = GroupName + ".Room";
      public const string Create = Default + ".Create";
      public const string Update = Default + ".Update";
      public const string Delete = Default + ".Delete";
      public const string ManageSettings = Default + ".ManageSettings";
    }

    public static class Booking
    {
      public const string Default = GroupName + ".Booking";
      public const string View = Default + ".View"; // View all bookings
      public const string Create = Default + ".Create"; // Create direct booking (bypass request)
      public const string Cancel = Default + ".Cancel"; // Cancel any booking
      public const string CancelOwn = Default + ".CancelOwn"; // Cancel own booking
      public const string CreateRecurring = Default + ".CreateRecurring"; // Create recurring bookings
    }

    public static class Reports
    {
      public const string Default = GroupName + ".Report";
      public const string ViewUsageReports = Default + ".ViewUsageReports";
      public const string ExportReports = Default + ".ExportReports";
      public const string ViewAnalytics = Default + ".ViewAnalytics";
    }
    public static class SchaduledBookings
    {
      public const string Default = GroupName + ".SchaduledBookings";
      public const string Create = Default + ".Create";
      public const string Edit = Default + ".Edit";
      public const string Delete = Default + ".Delete";
      public const string ManageAll = Default + ".ManageAll";
    }
    public static string[] GetAll()
    {
      return ReflectionHelper.GetPublicConstantsRecursively(typeof(UniversityBookingPermissions));
    }
  }
}
