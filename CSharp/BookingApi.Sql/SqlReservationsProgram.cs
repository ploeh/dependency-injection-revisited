using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.Sql
{
    public static class SqlReservationsProgram
    {
        public static T Interpret<T>(
            this IReservationsProgram<T> program,
            string connectionString)
        {
            return program.Match(
                pure: x => x,
                free: i => i.Match(
                    isReservationInFuture: t =>
                        t.Item2(IsReservationInFuture(t.Item1))
                            .Interpret(connectionString),
                    readReservations: t =>
                        t.Item2(ReadReservations(t.Item1, connectionString))
                            .Interpret(connectionString),
                    create: t =>
                        t.Item2(Create(t.Item1, connectionString))
                            .Interpret(connectionString)));
        }

        public static bool IsReservationInFuture(Reservation reservation)
        {
            return DateTimeOffset.Now < reservation.Date;
        }

        public static IReadOnlyCollection<Reservation> ReadReservations(
            DateTimeOffset date,
            string connectionString)
        {
            return ReadReservations(
                date.Date,
                date.Date.AddDays(1).AddTicks(-1),
                connectionString);
        }

        private static IReadOnlyCollection<Reservation> ReadReservations(
            DateTimeOffset min,
            DateTimeOffset max,
            string connectionString)
        {
            var result = new List<Reservation>();

            using (var conn = new SqlConnection(connectionString))
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

        private static int Create(Reservation reservation, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
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
