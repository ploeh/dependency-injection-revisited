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
            return
                from isInFuture in ReservationsProgram.IsReservationInFuture(reservation)
                from   _ in ReservationsProgram.Guard(isInFuture)

                from reservations in ReservationsProgram.ReadReservations(reservation.Date)
                let reservedSeats = reservations.Sum(r => r.Quantity)
                from  __ in ReservationsProgram.Guard(reservedSeats + reservation.Quantity <= Capacity)

                from ___ in ReservationsProgram.Do(() => { reservation.IsAccepted = true; })
                from id in ReservationsProgram.Create(reservation)
                select id;
        }

        public MaîtreD WithCapacity(int newCapacity)
        {
            return new MaîtreD(newCapacity);
        }
    }
}
