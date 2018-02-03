using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsInstructionParameters<T, TResult>
    {
        TResult IsReservationInFuture(Tuple<Reservation, Func<bool, T>> t);
        TResult ReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>> t);
        TResult Create(Tuple<Reservation, Func<int, T>> t);
    }
}
