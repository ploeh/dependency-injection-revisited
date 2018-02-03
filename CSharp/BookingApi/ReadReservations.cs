using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class ReadReservations<T> : IReservationsInstruction<T>
    {
        private readonly DateTimeOffset date;
        private readonly Func<IReadOnlyCollection<Reservation>, T> continuation;

        public ReadReservations(
            DateTimeOffset date,
            Func<IReadOnlyCollection<Reservation>, T> continuation)
        {
            this.date = date;
            this.continuation = continuation;
        }

        public TResult Accept<TResult>(
            IReservationsInstructionVisitor<T, TResult> visitor)
        {
            return visitor.VisitReadReservations(date, continuation);
        }
    }
}
