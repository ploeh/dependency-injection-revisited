using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public static class ReservationsProgram
    {
        public static T Interpret<T>(
            this IReservationsProgram<T> program,
            bool isInFuture,
            IReadOnlyCollection<Reservation> reservations,
            int id)
        {
            return program.Match(
                pure: x => x,
                free: i => i.Accept(
                    new InterpretReservationsInstructionVisitor<T>(
                        isInFuture,
                        reservations,
                        id)));
        }

        private class InterpretReservationsInstructionVisitor<T> :
            IReservationsInstructionVisitor<IReservationsProgram<T>, T>
        {
            private readonly bool isInFuture;
            private readonly IReadOnlyCollection<Reservation> reservations;
            private readonly int id;

            public InterpretReservationsInstructionVisitor(
                bool isInFuture,
                IReadOnlyCollection<Reservation> reservations,
                int id)
            {
                this.isInFuture = isInFuture;
                this.reservations = reservations;
                this.id = id;
            }

            public T VisitIsReservationInFuture(Tuple<Reservation, Func<bool, IReservationsProgram<T>>> t)
            {
                return t.Item2(isInFuture)
                    .Interpret(isInFuture, reservations, id);
            }

            public T VisitReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, IReservationsProgram<T>>> t)
            {
                return t.Item2(reservations)
                    .Interpret(isInFuture, reservations, id);
            }

            public T VisitCreate(Tuple<Reservation, Func<int, IReservationsProgram<T>>> t)
            {
                return t.Item2(id).Interpret(isInFuture, reservations, id);
            }
        }
    }
}
