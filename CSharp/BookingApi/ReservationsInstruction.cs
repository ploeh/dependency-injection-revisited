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
            return source.Accept(
                new SelectReservationsInstructionVisitor<T, TResult>(
                    selector));
        }

        private class SelectReservationsInstructionVisitor<T, TResult> :
            IReservationsInstructionVisitor<T, IReservationsInstruction<TResult>>
        {
            private readonly Func<T, TResult> selector;

            public SelectReservationsInstructionVisitor(
                Func<T, TResult> selector)
            {
                this.selector = selector;
            }

            public IReservationsInstruction<TResult> VisitIsReservationInFuture(
                Reservation reservation,
                Func<bool, T> continuation)
            {
                return new IsReservationInFuture<TResult>(
                    reservation,
                    b => selector(continuation(b)));
            }

            public IReservationsInstruction<TResult> VisitReadReservations(
                DateTimeOffset date,
                Func<IReadOnlyCollection<Reservation>, T> continuation)
            {
                return new ReadReservations<TResult>(
                    date,
                    d => selector(continuation(d)));
            }

            public IReservationsInstruction<TResult> VisitCreate(
                Reservation reservation,
                Func<int, T> continuation)
            {
                return new Create<TResult>(
                    reservation,
                    r => selector(continuation(r)));
            }
        }
    }
}
