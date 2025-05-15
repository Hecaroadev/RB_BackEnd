using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityBooking.BookingRequests
{
    /// <summary>
    /// State machine to handle booking request transitions and enforce business rules
    /// </summary>
    public class BookingRequestStateMachine
    {
        // Define all possible transitions between states
        private static readonly Dictionary<BookingRequestStatus, HashSet<BookingRequestStatus>> _allowedTransitions = 
            new Dictionary<BookingRequestStatus, HashSet<BookingRequestStatus>>
            {
                { 
                    BookingRequestStatus.Pending, 
                    new HashSet<BookingRequestStatus> 
                    { 
                        BookingRequestStatus.Approved,
                        BookingRequestStatus.Rejected,
                        BookingRequestStatus.Cancelled
                    } 
                },
                { 
                    BookingRequestStatus.Approved, 
                    new HashSet<BookingRequestStatus> 
                    { 
                        BookingRequestStatus.Cancelled
                    } 
                },
                { 
                    BookingRequestStatus.Rejected, 
                    new HashSet<BookingRequestStatus>() // Cannot transition from rejected
                },
                { 
                    BookingRequestStatus.Cancelled, 
                    new HashSet<BookingRequestStatus>() // Cannot transition from cancelled
                }
            };
            
        // Check if a transition is valid
        public static bool CanTransitionTo(BookingRequestStatus currentStatus, BookingRequestStatus targetStatus)
        {
            // Same state is always allowed (no transition)
            if (currentStatus == targetStatus)
                return true;
                
            return _allowedTransitions.ContainsKey(currentStatus) && 
                   _allowedTransitions[currentStatus].Contains(targetStatus);
        }
        
        // Get all possible next states
        public static IEnumerable<BookingRequestStatus> GetAllowedTransitions(BookingRequestStatus currentStatus)
        {
            if (_allowedTransitions.ContainsKey(currentStatus))
                return _allowedTransitions[currentStatus];
                
            return Enumerable.Empty<BookingRequestStatus>();
        }
        
        // Try to transition and throw exception if not allowed
        public static void EnsureValidTransition(BookingRequestStatus currentStatus, BookingRequestStatus targetStatus)
        {
            if (!CanTransitionTo(currentStatus, targetStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot transition from {currentStatus} to {targetStatus}. " +
                    $"Allowed transitions are: {string.Join(", ", GetAllowedTransitions(currentStatus))}");
            }
        }
        
        // Check if a booking request is in a final state
        public static bool IsInFinalState(BookingRequestStatus status)
        {
            return status == BookingRequestStatus.Approved || 
                   status == BookingRequestStatus.Rejected ||
                   status == BookingRequestStatus.Cancelled;
        }
    }
}