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
                new ReservationsProgramParameters<T, T>(
                    pure: x => x,
                    free: i => i.Accept(
                        new InterpretReservationsInstructionParameters<T>(
                            connectionString))));
        }

        private class InterpretReservationsInstructionParameters<T> :
            IReservationsInstructionVisitor<IReservationsProgram<T>, T>
        {
            private readonly string connectionString;

            public InterpretReservationsInstructionParameters(
                string connectionString)
            {
                this.connectionString = connectionString;
            }

            public T VisitIsReservationInFuture(Tuple<Reservation, Func<bool, IReservationsProgram<T>>> t)
            {
                var isInFuture = DateTimeOffset.Now < t.Item1.Date;
                return t.Item2(isInFuture).Interpret(connectionString);
            }

            public T VisitReadReservations(Tuple<DateTimeOffset, Func<IReadOnlyCollection<Reservation>, IReservationsProgram<T>>> t)
            {
                var reservations = ReadReservations(
                    t.Item1.Date,
                    t.Item1.Date.AddDays(1).AddTicks(-1));
                return t.Item2(reservations).Interpret(connectionString);
            }

            private IReadOnlyCollection<Reservation> ReadReservations(
                DateTimeOffset min,
                DateTimeOffset max)
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

            public T VisitCreate(
                Tuple<Reservation, Func<int, IReservationsProgram<T>>> t)
            {
                return t.Item2(Create(t.Item1)).Interpret(connectionString);
            }

            private int Create(Reservation reservation)
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
}
