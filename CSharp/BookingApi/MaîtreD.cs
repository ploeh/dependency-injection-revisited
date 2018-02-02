using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class MaîtreD : IMaîtreD
    {
        public MaîtreD(int capacity, IReservationsRepository reservationsRepository)
        {
            Capacity = capacity;
            ReservationsRepository = reservationsRepository;
        }

        public int Capacity { get; }

        public IReservationsRepository ReservationsRepository { get; }

        public int? TryAccept(Reservation reservation)
        {
            if (ReservationsRepository.IsReservationInFuture(reservation))
            {
                var reservedSeats =
                    ReservationsRepository
                        .ReadReservations(reservation.Date)
                        .Sum(r => r.Quantity);
                if (reservedSeats + reservation.Quantity <= Capacity)
                {
                    reservation.IsAccepted = true;
                    return ReservationsRepository.Create(reservation);
                }
            }

            return null;
        }

        public MaîtreD WithCapacity(int newCapacity)
        {
            return new MaîtreD(newCapacity, ReservationsRepository);
        }
    }
}
