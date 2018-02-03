using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class Free<T> : IReservationsProgram<T>
    {
        private readonly IReservationsInstruction<IReservationsProgram<T>> i;

        public Free(IReservationsInstruction<IReservationsProgram<T>> i)
        {
            this.i = i;
        }

        public TResult Match<TResult>(
            ReservationsProgramParameters<T, TResult> parameters)
        {
            return parameters.Free(this.i);
        }
    }
}
