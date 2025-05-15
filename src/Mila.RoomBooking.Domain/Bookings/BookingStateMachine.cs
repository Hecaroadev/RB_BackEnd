using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityBooking.Bookings
{
    /// <summary>
    /// State machine to handle booking transitions and enforce business rules
    /// </summary>
    public class BookingStateMachine
    {
        // Define all possible transitions between states
        private static readonly Dictionary<BookingStatus, HashSet<BookingStatus>> _allowedTransitions = 
            new Dictionary<BookingStatus, HashSet<BookingStatus>>
            {
                { 
                    BookingStatus.Active, 
                    new HashSet<BookingStatus> 
                    { 
                        BookingStatus.Cancelled,
                        BookingStatus.Completed
                    } 
                },
                { 
                    BookingStatus.Cancelled, 
                    new HashSet<BookingStatus>() // Cannot transition from cancelled
                },
                { 
                    BookingStatus.Completed, 
                    new HashSet<BookingStatus>() // Cannot transition from completed
                }
            };
            
        // Check if a transition is valid
        public static bool CanTransitionTo(BookingStatus currentStatus, BookingStatus targetStatus)
        {
            // Same state is always allowed (no transition)
            if (currentStatus == targetStatus)
                return true;
                
            return _allowedTransitions.ContainsKey(currentStatus) && 
                   _allowedTransitions[currentStatus].Contains(targetStatus);
        }
        
        // Get all possible next states
        public static IEnumerable<BookingStatus> GetAllowedTransitions(BookingStatus currentStatus)
        {
            if (_allowedTransitions.ContainsKey(currentStatus))
                return _allowedTransitions[currentStatus];
                
            return Enumerable.Empty<BookingStatus>();
        }
        
        // Try to transition and throw exception if not allowed
        public static void EnsureValidTransition(BookingStatus currentStatus, BookingStatus targetStatus)
        {
            if (!CanTransitionTo(currentStatus, targetStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot transition from {currentStatus} to {targetStatus}. " +
                    $"Allowed transitions are: {string.Join(", ", GetAllowedTransitions(currentStatus))}");
            }
        }
        
        // Check if a booking is in a final state
        public static bool IsInFinalState(BookingStatus status)
        {
            return status == BookingStatus.Cancelled || 
                   status == BookingStatus.Completed;
        }
        
        // Check if a booking is active for a given date
        public static bool IsActiveOnDate(Booking booking, DateTime date)
        {
            if (booking.Status != BookingStatus.Active)
                return false;
                
            // If booking is for a specific date, check exact match
            if (booking.BookingDate.HasValue)
                return booking.BookingDate.Value.Date == date.Date;
                
            // If it's a recurring booking, check if the day of week matches
            return booking.Day?.DayOfWeek == date.DayOfWeek;
        }
    }
}