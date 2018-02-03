using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class ReadReservations<T> : IReservationsInstruction<T>
    {
        private readonly Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>> t;

        public ReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, T>> t)
        {
            this.t = t;
        }

        public TResult Accept<TResult>(
            IReservationsInstructionVisitor<T, TResult> visitor)
        {
            return visitor.VisitReadReservations(this.t);
        }
    }
}
