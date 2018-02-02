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
            Func<IReservationsInstruction<IReservationsProgram<T>> ,TResult> free,
            Func<T, TResult> pure);
    }
}
