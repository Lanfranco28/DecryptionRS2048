using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    public static class Conexion
    {
        private static IDbConnection Connection { get; set; }
        public static void DbSession(string connectionString)
        {
            try
            {
                Connection = new SqlConnection(connectionString);

                Connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
