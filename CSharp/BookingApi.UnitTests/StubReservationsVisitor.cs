using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class StubReservationsVisitor<T> :
        IReservationsProgramVisitor<T, T>,
        IReservationsInstructionVisitor<IReservationsProgram<T>, T>
    {
        private readonly bool isInFuture;
        private readonly IReadOnlyCollection<Reservation> reservations;
        private readonly int id;

        public StubReservationsVisitor(
            bool isInFuture,
            IReadOnlyCollection<Reservation> reservations,
            int id)
        {
            this.isInFuture = isInFuture;
            this.reservations = reservations;
            this.id = id;
        }

        public T VisitPure(T x)
        {
            return x;
        }

        public T VisitFree(IReservationsInstruction<IReservationsProgram<T>> i)
        {
            return i.Accept(this);
        }

        public T VisitIsReservationInFuture(
            Reservation reservation,
            Func<bool, IReservationsProgram<T>> continuation)
        {
            return continuation(isInFuture).Accept(this);
        }

        public T VisitReadReservations(
            DateTimeOffset date,
            Func<IReadOnlyCollection<Reservation>, IReservationsProgram<T>> continuation)
        {
            return continuation(reservations).Accept(this);
        }

        public T VisitCreate(
            Reservation reservation,
            Func<int, IReservationsProgram<T>> continuation)
        {
            return continuation(id).Accept(this);
        }
    }
}
