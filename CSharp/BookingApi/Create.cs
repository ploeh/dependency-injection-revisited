using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class Create<T> : IReservationsInstruction<T>
    {
        private readonly Tuple<Reservation, Func<int, T>> t;

        public Create(Tuple<Reservation, Func<int, T>> t)
        {
            this.t = t;
        }

        public TResult Match<TResult>(
            IReservationsInstructionParameters<T, TResult> parameters)
        {
            return parameters.Create(this.t);
        }
    }
}
