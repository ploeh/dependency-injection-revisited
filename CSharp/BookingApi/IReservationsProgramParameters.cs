using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsProgramParameters<T, TResult>
    {
        TResult Free(IReservationsInstruction<IReservationsProgram<T>> i);
        TResult Pure(T x);
    }
}
