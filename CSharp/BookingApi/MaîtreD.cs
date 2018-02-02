using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class MaîtreD : IMaîtreD
    {
        public int? TryAccept(Reservation reservation)
        {
            throw new NotImplementedException();
        }
    }
}
