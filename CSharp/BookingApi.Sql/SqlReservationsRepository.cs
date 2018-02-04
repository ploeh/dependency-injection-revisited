using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.Sql
{
    public class SqlReservationsRepository : IReservationsRepository
    {
        public SqlReservationsRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public bool IsReservationInFuture(Reservation reservation)
        {
            return DateTimeOffset.Now < reservation.Date;
        }

        public IReadOnlyCollection<Reservation> ReadReservations(
            DateTimeOffset date)
        {
            return this.ReadReservations(
                date.Date,
                date.Date.AddDays(1).AddTicks(-1));
        }

        private IReadOnlyCollection<Reservation> ReadReservations(
            DateTimeOffset min,
            DateTimeOffset max)
        {
            var result = new List<Reservation>();

            using (var conn = new SqlConnection(this.ConnectionString))
            using (var cmd = new SqlCommand(readByRangeSql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MinDate", min));
                cmd.Parameters.Add(new SqlParameter("@MaxDate", max));

                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        result.Add(
                            new Reservation
                            {
                                Date = (DateTimeOffset)rdr["Date"],
                                Name = (string)rdr["Name"],
                                Email = (string)rdr["Email"],
                                Quantity = (int)rdr["Quantity"]
                            });
                }
            }

            return result;
        }

        private const string readByRangeSql = @"
            SELECT [Date], [Name], [Email], [Quantity]
            FROM [dbo].[Reservations]
            WHERE YEAR(@MinDate) <= YEAR([Date])
            AND MONTH(@MinDate) <= MONTH([Date])
            AND DAY(@MinDate) <= DAY([Date])
            AND YEAR([Date]) <= YEAR(@MaxDate)
            AND MONTH([Date]) <= MONTH(@MaxDate)
            AND DAY([Date]) <= DAY(@MaxDate)";

        public int Create(Reservation reservation)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(createReservationSql, conn))
            {
                cmd.Parameters.Add(
                    new SqlParameter("@Date", reservation.Date));
                cmd.Parameters.Add(
                    new SqlParameter("@Name", reservation.Name));
                cmd.Parameters.Add(
                    new SqlParameter("@Email", reservation.Email));
                cmd.Parameters.Add(
                    new SqlParameter("@Quantity", reservation.Quantity));

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private const string createReservationSql = @"
            INSERT INTO [dbo].[Reservations] ([Date], [Name], [Email], [Quantity])
            VALUES (@Date, @Name, @Email, @Quantity)";
    }
}
