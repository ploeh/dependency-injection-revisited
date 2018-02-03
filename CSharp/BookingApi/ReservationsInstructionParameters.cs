using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsInstructionParameters<T, TResult>
    {
        public ReservationsInstructionParameters(
            Func<Tuple<Reservation, Func<bool, T>>, TResult> isReservationInFuture,
            Func<Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>>, TResult> readReservations,
            Func<Tuple<Reservation, Func<int, T>>, TResult> create)
        {
            this.IsReservationInFuture = isReservationInFuture;
            this.ReadReservations = readReservations;
            this.Create = create;
        }

        public Func<Tuple<Reservation, Func<bool, T>>, TResult> IsReservationInFuture { get; }
        public Func<Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>>, TResult> ReadReservations { get; }
        public Func<Tuple<Reservation, Func<int, T>>, TResult> Create { get; }
    }
}
