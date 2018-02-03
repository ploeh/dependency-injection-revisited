using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsInstructionVisitor<T, TResult>
    {
        TResult VisitIsReservationInFuture(Tuple<Reservation, Func<bool, T>> t);
        TResult VisitReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>> t);
        TResult VisitCreate(Tuple<Reservation, Func<int, T>> t);
    }
}
