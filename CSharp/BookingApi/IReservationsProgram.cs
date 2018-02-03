using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsProgram<T>
    {
        TResult Match<TResult>(
            ReservationsProgramParameters<T, TResult> parameters);
    }
}
