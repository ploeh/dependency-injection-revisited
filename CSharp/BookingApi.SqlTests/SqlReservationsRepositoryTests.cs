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
    public class SqlReservationsRepositoryTests
    {
        [Fact]
        public void UsageExample()
        {
            var connectionString = ConnectionStrings.Reservations;
            var sqlRepository = new SqlReservationsRepository(connectionString);
            var maîtreD = new MaîtreD(10, sqlRepository);

            var reservation = new Reservation
            {
                Date = DateTimeOffset.Now.AddDays(27),
                Name = "Mark Seemann",
                Email = "mark@example.net",
                Quantity = 2
            };
            var id = maîtreD.TryAccept(reservation);

            Assert.NotNull(id);
            Assert.NotEqual(default(int), id);
        }

        [Fact]
        public void IsReservationInFutureReturnsTrue()
        {
            var now = DateTimeOffset.Now;
            var sut = new SqlReservationsRepository(ConnectionStrings.Reservations);

            var reservation = new Reservation
            {
                Date = now.AddDays(4),
                Name = "Sgryt Ler",
                Email = "sgryt@example.org",
                Quantity = 2
            };
            var actual = sut.IsReservationInFuture(reservation);

            Assert.True(actual);
        }

        [Fact]
        public void IsReservationInFutureReturnsFalse()
        {
            var now = DateTimeOffset.Now;
            var sut = new SqlReservationsRepository(ConnectionStrings.Reservations);

            var reservation = new Reservation
            {
                Date = now.AddDays(-2),
                Name = "Qux Corge",
                Email = "qux@example.com",
                Quantity = 1
            };
            var actual = sut.IsReservationInFuture(reservation);

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
            var sut = new SqlReservationsRepository(ConnectionStrings.Reservations);

            var actual = sut.ReadReservations(
                new DateTimeOffset(
                    new DateTime(2018, 2, 5),
                    TimeSpan.FromHours(1)));

            Assert.Equal(1, actual.Count);
            Assert.Equal("Ploeh Fnaah", actual.First().Name);
            Assert.Equal("ploeh@example.org", actual.First().Email);
            Assert.Equal(3, actual.First().Quantity);
        }

        [Fact]
        public void CreateAddsRowToDatabase()
        {
            var sut = new SqlReservationsRepository(ConnectionStrings.Reservations);

            var actual = sut.Create(
                new Reservation
                {
                    Date = new DateTimeOffset(2018, 2, 4, 16, 38, 51, TimeSpan.FromHours(1)),
                    Email = "foo@example.com",
                    Name = "Foo Bar",
                    IsAccepted = true,
                    Quantity = 4
                });

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
