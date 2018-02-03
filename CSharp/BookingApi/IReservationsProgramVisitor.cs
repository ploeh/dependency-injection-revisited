using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public interface IReservationsProgramVisitor<T, TResult>
    {
        TResult VisitFree(IReservationsInstruction<IReservationsProgram<T>> i);
        TResult VisitPure(T x);
    }
}
