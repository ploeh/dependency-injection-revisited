using Ploeh.Samples.BookingApi.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Ploeh.Samples.BookingApi.SqlTests
{
    public class UseDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            using (var schemaStream = ReadSchema())
            using (var rdr = new StreamReader(schemaStream))
            {
                var schemaSql = rdr.ReadToEnd();

                var builder = new SqlConnectionStringBuilder(
                    ConnectionStrings.Reservations);
                builder.InitialCatalog = "Master";
                using (var conn = new SqlConnection(builder.ConnectionString))
                using (var cmd = new SqlCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;

                    foreach (var sql in SeperateStatements(schemaSql))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            base.Before(methodUnderTest);
        }

        private Stream ReadSchema()
        {
            return typeof(SqlReservationsProgramVisitor<>)
                .Assembly
                .GetManifestResourceStream(
                    "Ploeh.Samples.BookingApi.Sql.BookingDbSchema.sql");
        }

        private static IEnumerable<string> SeperateStatements(string schemaSql)
        {
            return schemaSql.Split(
                new[] { "GO" },
                StringSplitOptions.RemoveEmptyEntries);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            base.After(methodUnderTest);

            var dropCmd = @"
                IF EXISTS (SELECT name
                    FROM master.dbo.sysdatabases
                    WHERE name = N'Booking')
                DROP DATABASE[Booking];";

            var builder = new SqlConnectionStringBuilder(
                ConnectionStrings.Reservations);
            builder.InitialCatalog = "Master";
            using (var conn = new SqlConnection(builder.ConnectionString))
            using (var cmd = new SqlCommand(dropCmd, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
