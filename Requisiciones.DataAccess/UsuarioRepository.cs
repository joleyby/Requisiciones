using System.Collections.Generic;
using System.Data.SqlClient;
using Requisiciones.Entities;

namespace Requisiciones.DataAccess
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private const string Columnas =
            "Id, NombreUsuario, NombreCompleto, Departamento, Rol, SuperiorId, LimiteAprobacion, Activo";

        public UsuarioDto ObtenerPorNombreUsuario(string nombreUsuario)
        {
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand($"SELECT {Columnas} FROM dbo.vw_Usuario WHERE NombreUsuario = @U", cn))
            {
                cmd.Parameters.AddWithValue("@U", nombreUsuario);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                    return rd.Read() ? Mapear(rd) : null;
            }
        }

        public CredencialDto ObtenerCredencial(int usuarioId)
        {
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand(
                "SELECT UsuarioId, PasswordHash, Salt, Iteraciones FROM dbo.vw_UsuarioCredencial WHERE UsuarioId = @Id", cn))
            {
                cmd.Parameters.AddWithValue("@Id", usuarioId);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;
                    return new CredencialDto
                    {
                        UsuarioId = rd.GetInt32(0),
                        PasswordHash = (byte[])rd["PasswordHash"],
                        Salt = (byte[])rd["Salt"],
                        Iteraciones = rd.GetInt32(3)
                    };
                }
            }
        }

        /// <summary>Superiores del usuario, del más cercano al más alto.</summary>
        public List<UsuarioDto> ObtenerCadenaSuperiores(int usuarioId)
        {
            const string sql =
                @"SELECT u.Id, u.NombreUsuario, u.NombreCompleto, u.Departamento, u.Rol,
                         u.SuperiorId, u.LimiteAprobacion, u.Activo
                  FROM dbo.vw_UsuarioJerarquia j
                  JOIN dbo.vw_Usuario u ON u.Id = j.SuperiorId
                  WHERE j.UsuarioId = @Id
                  ORDER BY j.Nivel";
            var lista = new List<UsuarioDto>();
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@Id", usuarioId);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read()) lista.Add(Mapear(rd));
            }
            return lista;
        }

        private static UsuarioDto Mapear(SqlDataReader rd)
        {
            int ordSup = rd.GetOrdinal("SuperiorId");
            int ordLim = rd.GetOrdinal("LimiteAprobacion");
            return new UsuarioDto
            {
                Id = rd.GetInt32(rd.GetOrdinal("Id")),
                NombreUsuario = rd.GetString(rd.GetOrdinal("NombreUsuario")),
                NombreCompleto = rd.GetString(rd.GetOrdinal("NombreCompleto")),
                Departamento = rd.GetString(rd.GetOrdinal("Departamento")),
                Rol = (RolUsuario)rd.GetByte(rd.GetOrdinal("Rol")),
                SuperiorId = rd.IsDBNull(ordSup) ? (int?)null : rd.GetInt32(ordSup),
                LimiteAprobacion = rd.IsDBNull(ordLim) ? (decimal?)null : rd.GetDecimal(ordLim),
                Activo = rd.GetBoolean(rd.GetOrdinal("Activo"))
            };
        }
    }
}
