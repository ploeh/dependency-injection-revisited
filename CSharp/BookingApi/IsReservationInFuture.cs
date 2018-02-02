using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class IsReservationInFuture<T> : IReservationsInstruction<T>
    {
        private readonly Tuple<Reservation, Func<bool, T>> t;

        public IsReservationInFuture(Tuple<Reservation, Func<bool, T>> t)
        {
            this.t = t;
        }

        public TResult Match<TResult>(
            Func<Tuple<Reservation, Func<bool, T>>, TResult> isReservationInFuture,
            Func<Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>>, TResult> readReservations,
            Func<Tuple<Reservation, Func<int, T>>, TResult> create)
        {
            return isReservationInFuture(this.t);
        }
    }
}
