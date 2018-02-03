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
            ReservationsInstructionParameters<T, TResult> parameters)
        {
            return parameters.IsReservationInFuture(this.t);
        }
    }
}
