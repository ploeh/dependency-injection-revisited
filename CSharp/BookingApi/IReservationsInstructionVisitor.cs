using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsInstructionVisitor<T, TResult>
    {
        TResult VisitIsReservationInFuture(
            Reservation reservation,
            Func<bool, T> continuation);
        TResult VisitReadReservations(
            DateTimeOffset date,
            Func<IReadOnlyCollection<Reservation>, T> continuation);
        TResult VisitCreate(
            Reservation reservation,
            Func<int, T> continuation);
    }
}
