using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Requisiciones.DataAccess
{
    /// <summary>SINGLETON: única fábrica de conexiones de la aplicación.</summary>
    public sealed class DbConnectionFactory
    {
        private static readonly Lazy<DbConnectionFactory> _instancia =
            new Lazy<DbConnectionFactory>(() => new DbConnectionFactory());

        public static DbConnectionFactory Instancia => _instancia.Value;

        private readonly string _cadena;

        private DbConnectionFactory()
        {
            _cadena = ConfigurationManager.ConnectionStrings["RequisicionesDb"].ConnectionString;
        }

        public SqlConnection Crear() => new SqlConnection(_cadena);
    }
}
