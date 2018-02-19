using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class MaîtreD : IMaîtreD
    {
        public MaîtreD(int capacity)
        {
            Capacity = capacity;
        }

        public int Capacity { get; }

        public IReservationsProgram<IMaybe<int>> TryAccept(Reservation reservation)
        {
            return ReservationsProgram.IsReservationInFuture(reservation)
                .SelectMany(isInFuture => ReservationsProgram.Guard(isInFuture))

                .SelectMany((Unit _) => ReservationsProgram.ReadReservations(reservation.Date))
                .Select(reservations => reservations.Sum(r => r.Quantity))
                .SelectMany(reservedSeats =>
                    ReservationsProgram.Guard(reservedSeats + reservation.Quantity <= Capacity))

                .SelectMany((Unit _) => ReservationsProgram.Do(() => { reservation.IsAccepted = true; }))
                .SelectMany((Unit _) => ReservationsProgram.Create(reservation));
        }

        public MaîtreD WithCapacity(int newCapacity)
        {
            return new MaîtreD(newCapacity);
        }
    }
}
