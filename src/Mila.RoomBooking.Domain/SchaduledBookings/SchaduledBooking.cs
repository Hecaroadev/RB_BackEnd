using System;
using System.Collections.Generic;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Mila.RoomBooking.SchaduledBookings
{
  /// <summary>
  /// Represents a scheduled booking that repeats weekly for a specific room and day
  /// </summary>
  public class SchaduledBooking : FullAuditedAggregateRoot<Guid>
  {
    public Guid RoomId { get; protected set; }
    public Guid DayId { get; protected set; }
    public string Purpose { get; set; }
    public string InstructorName { get; set; }
    public string Subject { get; set; }
    public BookingStatus Status { get; set; }

    // Navigation properties
    public virtual Room Room { get; set; }
    public virtual Day Day { get; set; }
    public virtual IdentityUser ReservedByUser { get; set; }

    // Many-to-many relationship with TimeSlots
    public virtual ICollection<TimeSlot> TimeRange { get; set; }

    protected SchaduledBooking()
    {
      // Required for EF Core
      TimeRange = new List<TimeSlot>();
    }

    public SchaduledBooking(
      Guid id,
      Guid roomId,
      Guid dayId,
      string purpose,
      string instructorName,
      string subject,
      BookingStatus status,
      List<TimeSlot> timeSlots) : base(id)
    {
      RoomId = roomId;
      DayId = dayId;
      Purpose = purpose;
      InstructorName = instructorName;
      Subject = subject;
      Status = status;
      TimeRange = timeSlots ?? new List<TimeSlot>();
    }

    public void UpdateTimeRange(List<TimeSlot> newTimeSlots)
    {
      TimeRange.Clear();
      foreach (var slot in newTimeSlots)
      {
        TimeRange.Add(slot);
      }
    }

    public void Cancel()
    {
      Status = BookingStatus.Cancelled;
    }

    public void Confirm()
    {
      Status = BookingStatus.Confirmed;
    }
  }
}
