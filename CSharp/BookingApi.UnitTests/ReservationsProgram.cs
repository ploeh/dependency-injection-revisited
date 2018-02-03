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
                free: i => i.Match(
                    new InterpretReservationsInstructionParameter<T>(
                        isInFuture,
                        reservations,
                        id)));
        }

        private class InterpretReservationsInstructionParameter<T> :
            IReservationsInstructionParameters<IReservationsProgram<T>, T>
        {
            private readonly bool isInFuture;
            private readonly IReadOnlyCollection<Reservation> reservations;
            private readonly int id;

            public InterpretReservationsInstructionParameter(
                bool isInFuture,
                IReadOnlyCollection<Reservation> reservations,
                int id)
            {
                this.isInFuture = isInFuture;
                this.reservations = reservations;
                this.id = id;
            }

            public T IsReservationInFuture(Tuple<Reservation, Func<bool, IReservationsProgram<T>>> t)
            {
                return t.Item2(isInFuture)
                    .Interpret(isInFuture, reservations, id);
            }

            public T ReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, IReservationsProgram<T>>> t)
            {
                return t.Item2(reservations)
                    .Interpret(isInFuture, reservations, id);
            }

            public T Create(Tuple<Reservation, Func<int, IReservationsProgram<T>>> t)
            {
                return t.Item2(id).Interpret(isInFuture, reservations, id);
            }
        }
    }
}
