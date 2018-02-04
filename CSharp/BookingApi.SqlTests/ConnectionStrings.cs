using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.SqlTests
{
    public class ConnectionStrings
    {
        public const string Reservations =
            @"Server=(LocalDB)\MSSQLLocalDB;Database=Booking;Integrated Security=true;Pooling=false";
    }
}
