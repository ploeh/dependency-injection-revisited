using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class Create<T> : IReservationsInstruction<T>
    {
        private readonly Reservation reservation;
        private readonly Func<int, T> continuation;

        public Create(
            Reservation reservation,
            Func<int, T> continuation)
        {
            this.reservation = reservation;
            this.continuation = continuation;
        }

        public TResult Accept<TResult>(
            IReservationsInstructionVisitor<T, TResult> visitor)
        {
            return visitor.VisitCreate(reservation, continuation);
        }
    }
}
