using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class IsReservationInFuture<T> : IReservationsInstruction<T>
    {
        private readonly Reservation reservation;
        private readonly Func<bool, T> continuation;

        public IsReservationInFuture(
            Reservation reservation,
            Func<bool, T> continuation)
        {
            this.reservation = reservation;
            this.continuation = continuation;
        }

        public TResult Accept<TResult>(
            IReservationsInstructionVisitor<T, TResult> visitor)
        {
            return
                visitor.VisitIsReservationInFuture(reservation, continuation);
        }
    }
}
