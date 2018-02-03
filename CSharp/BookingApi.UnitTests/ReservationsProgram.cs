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
                    new ReservationsInstructionParameters<IReservationsProgram<T>, T>(
                        isReservationInFuture: t => t.Item2(isInFuture).Interpret(isInFuture, reservations, id),
                        readReservations: t => t.Item2(reservations).Interpret(isInFuture, reservations, id),
                        create: t => t.Item2(id).Interpret(isInFuture, reservations, id))));
        }
    }
}
