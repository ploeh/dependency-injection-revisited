using System;
using System.Collections.Generic;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsRepository
    {
        // This method doesn't belong on a Repository interface. It ought to be
        // on some sort of ITimeProvider interface, or an extension method
        // thereof, but in order to keep the example refactorings as simple as
        // possible, it'll go here for demo purposes.
        bool IsReservationInFuture(Reservation reservation);

        IReadOnlyCollection<Reservation> ReadReservations(DateTimeOffset date);

        int Create(Reservation reservation);
    }
}