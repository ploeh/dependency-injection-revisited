using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsProgramParameters<T, TResult>
    {
        public ReservationsProgramParameters(
            Func<IReservationsInstruction<IReservationsProgram<T>>, TResult> free,
            Func<T, TResult> pure)
        {
            this.Free = free;
            this.Pure = pure;
        }

        public Func<IReservationsInstruction<IReservationsProgram<T>>, TResult> Free { get; }
        public Func<T, TResult> Pure { get; }
    }
}
