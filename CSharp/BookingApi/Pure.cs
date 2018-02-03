using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class Pure<T> : IReservationsProgram<T>
    {
        private readonly T x;

        public Pure(T x)
        {
            this.x = x;
        }

        public TResult Accept<TResult>(
            IReservationsProgramVisitor<T, TResult> visitor)
        {
            return visitor.VisitPure(this.x);
        }
    }
}
