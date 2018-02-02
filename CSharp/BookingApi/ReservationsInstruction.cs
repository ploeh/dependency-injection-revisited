using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public static class ReservationsInstruction
    {
        public static IReservationsInstruction<TResult> Select<T, TResult>(
            this IReservationsInstruction<T> source,
            Func<T, TResult> selector)
        {
            return source.Match<IReservationsInstruction<TResult>>(
                isReservationInFuture: t =>
                    new IsReservationInFuture<TResult>(new Tuple<Reservation, Func<bool, TResult>>(
                        t.Item1,
                        b => selector(t.Item2(b)))),
                readReservations: t =>
                    new ReadReservations<TResult>(new Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, TResult>>(
                        t.Item1,
                        d => selector(t.Item2(d)))),
                create: t =>
                    new Create<TResult>(new Tuple<Reservation, Func<int, TResult>>(
                        t.Item1,
                        r => selector(t.Item2(r)))));
        }
    }
}
