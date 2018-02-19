using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class MaîtreDTests
    {
        [Theory, BookingApiTestConventions]
        public void TryAcceptReturnsReservationIdInHappyPathScenario(
            Reservation reservation,
            IReadOnlyCollection<Reservation> reservations,
            MaîtreD sut,
            int excessCapacity,
            int expected)
        {
            var reservedSeats = reservations.Sum(r => r.Quantity);
            reservation.IsAccepted = false;
            sut = sut.WithCapacity(
                reservedSeats + reservation.Quantity + excessCapacity);

            var actual = sut.TryAccept(reservation);

            Assert.Equal(
                new Just<int>(expected),
                actual.Accept(new StubReservationsVisitor<IMaybe<int>>(true, reservations, expected)));
            Assert.True(reservation.IsAccepted);
        }

        [Theory, BookingApiTestConventions]
        public void TryAcceptReturnsNullOnReservationInThePast(
            Reservation reservation,
            IReadOnlyCollection<Reservation> reservations,
            int id,
            MaîtreD sut)
        {
            reservation.IsAccepted = false;

            var actual = sut.TryAccept(reservation);

            Assert.True(
                actual.Accept(new StubReservationsVisitor<IMaybe<int>>(false, reservations, id)).IsNothing());
            Assert.False(reservation.IsAccepted);
        }

        [Theory, BookingApiTestConventions]
        public void TryAcceptReturnsNullOnInsufficientCapacity(
            Reservation reservation,
            IReadOnlyCollection<Reservation> reservations,
            int id,
            MaîtreD sut)
        {
            reservation.IsAccepted = false;
            var reservedSeats = reservations.Sum(r => r.Quantity);
            sut = sut.WithCapacity(reservedSeats + reservation.Quantity - 1);

            var actual = sut.TryAccept(reservation);

            Assert.True(
                actual.Accept(new StubReservationsVisitor<IMaybe<int>>(true, reservations, id)).IsNothing());
            Assert.False(reservation.IsAccepted);
        }
    }
}
