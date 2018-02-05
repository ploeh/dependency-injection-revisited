using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.Sql
{
    public class SqlReservationsProgramVisitor<T> :
        IReservationsProgramVisitor<T, T>,
        IReservationsInstructionVisitor<IReservationsProgram<T>, T>
    {
        private readonly string connectionString;

        public SqlReservationsProgramVisitor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public T VisitPure(T x)
        {
            return x;
        }

        public T VisitFree(IReservationsInstruction<IReservationsProgram<T>> i)
        {
            return i.Accept(this);
        }

        public T VisitIsReservationInFuture(
            Reservation reservation,
            Func<bool, IReservationsProgram<T>> continuation)
        {
            var isInFuture = DateTimeOffset.Now < reservation.Date;
            return continuation(isInFuture).Accept(this);
        }

        public T VisitReadReservations(
            DateTimeOffset date,
            Func<IReadOnlyCollection<Reservation>, IReservationsProgram<T>> continuation)
        {
            var reservations = ReadReservations(
                date.Date,
                date.Date.AddDays(1).AddTicks(-1));
            return continuation(reservations).Accept(this);
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
            Reservation reservation,
            Func<int, IReservationsProgram<T>> continuation)
        {
            return continuation(Create(reservation)).Accept(this);
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
