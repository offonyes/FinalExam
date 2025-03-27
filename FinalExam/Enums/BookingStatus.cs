namespace FinalExam.Enums
{
    public enum BookingStatus
    {
        Pending = 0,    // Waiting for confirmation (not required, but convenient)
        Confirmed = 1,  // Confirmed, ready for check-in
        //CheckedIn = 2,  // Guest checked in
        //CheckedOut = 3, // Guest checked out
        Cancelled = 4   // Cancelled
    }
}
