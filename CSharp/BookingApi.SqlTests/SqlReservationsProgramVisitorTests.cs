using Ploeh.Samples.BookingApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.BookingApi.SqlTests
{
    [UseDatabase]
    public class SqlReservationsProgramVisitorTests
    {
        [Fact]
        public void UsageExample()
        {
            var connectionString = ConnectionStrings.Reservations;
            var maîtreD = new MaîtreD(10);

            var reservation = new Reservation
            {
                Date = DateTimeOffset.Now.AddDays(27),
                Name = "Mark Seemann",
                Email = "mark@example.net",
                Quantity = 2
            };
            var p = maîtreD.TryAccept(reservation);
            var id = p.Accept(new SqlReservationsProgramVisitor<int?>(connectionString));

            Assert.NotNull(id);
            Assert.NotEqual(default(int), id);
        }

        [Fact]
        public void IsReservationInFutureReturnsTrue()
        {
            var now = DateTimeOffset.Now;
            var sut = new SqlReservationsProgramVisitor<bool>(
                ConnectionStrings.Reservations);

            var reservation = new Reservation
            {
                Date = now.AddDays(4),
                Name = "Sgryt Ler",
                Email = "sgryt@example.org",
                Quantity = 2
            };
            var p = ReservationsProgram.IsReservationInFuture(reservation);
            var actual = p.Accept(sut);

            Assert.True(actual);
        }

        [Fact]
        public void IsReservationInFutureReturnsFalse()
        {
            var now = DateTimeOffset.Now;
            var sut = new SqlReservationsProgramVisitor<bool>(
                ConnectionStrings.Reservations);

            var reservation = new Reservation
            {
                Date = now.AddDays(-2),
                Name = "Qux Corge",
                Email = "qux@example.com",
                Quantity = 1
            };
            var p = ReservationsProgram.IsReservationInFuture(reservation);
            var actual = p.Accept(sut);

            Assert.False(actual);
        }

        [Fact]
        public void ReadReservationsRetrievesRowsFromDatabase()
        {
            using (var conn = new SqlConnection(ConnectionStrings.Reservations))
            using (var cmd = new SqlCommand(@"
                INSERT INTO Reservations ([Date], [Name], [Email], [Quantity])
                VALUES ('2018-02-05 07:39:37 +01:00', 'Ploeh Fnaah', 'ploeh@example.org', 3)",
                conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            var sut = new SqlReservationsProgramVisitor<IReadOnlyCollection<Reservation>>(
                ConnectionStrings.Reservations);

            var p = ReservationsProgram.ReadReservations(
                new DateTimeOffset(
                    new DateTime(2018, 2, 5),
                    TimeSpan.FromHours(1)));
            var actual = p.Accept(sut);

            Assert.Equal(1, actual.Count);
            Assert.Equal("Ploeh Fnaah", actual.First().Name);
            Assert.Equal("ploeh@example.org", actual.First().Email);
            Assert.Equal(3, actual.First().Quantity);
        }

        [Fact]
        public void CreateAddsRowToDatabase()
        {
            var sut = new SqlReservationsProgramVisitor<int>(
                ConnectionStrings.Reservations);

            var p = ReservationsProgram.Create(
                new Reservation
                {
                    Date = new DateTimeOffset(2018, 2, 4, 16, 38, 51, TimeSpan.FromHours(1)),
                    Email = "foo@example.com",
                    Name = "Foo Bar",
                    IsAccepted = true,
                    Quantity = 4
                });
            var actual = p.Accept(sut);

            using (var conn = new SqlConnection(ConnectionStrings.Reservations))
            using (var cmd = new SqlCommand("SELECT * FROM Reservations", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    Assert.True(rdr.Read());
                    Assert.Equal(
                        new DateTimeOffset(2018, 2, 4, 16, 38, 51, TimeSpan.FromHours(1)),
                        rdr["Date"]);
                    Assert.Equal("foo@example.com", rdr["Email"]);
                    Assert.Equal("Foo Bar", rdr["Name"]);
                    Assert.Equal(4, rdr["Quantity"]);
                    Assert.Equal(actual, rdr["Id"]);
                }
            }
        }
    }
}
