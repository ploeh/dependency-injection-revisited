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

        public IReservationsProgram<int?> TryAccept(Reservation reservation)
        {
            return ReservationsProgram
                .IsReservationInFuture(reservation)
                .SelectMany(isInFuture =>
                {
                    if (!isInFuture)
                        return new Pure<int?>(null);

                    return ReservationsProgram
                        .ReadReservations(reservation.Date)
                        .SelectMany(reservations =>
                        {
                            var reservedSeats = reservations.Sum(r => r.Quantity);
                            if (Capacity < reservedSeats + reservation.Quantity)
                                return new Pure<int?>(null);

                            reservation.IsAccepted = true;
                            return ReservationsProgram
                                .Create(reservation)
                                .Select(x => new int?(x));
                        });
                });
        }

        public MaîtreD WithCapacity(int newCapacity)
        {
            return new MaîtreD(newCapacity);
        }
    }
}
