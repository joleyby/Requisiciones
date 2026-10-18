using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Requisiciones.Entities;

namespace Requisiciones.DataAccess
{
    public class RequisicionRepository : IRequisicionRepository
    {
        // ======================= ESCRITURA: solo Procedimientos Almacenados =======================
        public int Crear(RequisicionDto r)
        {
            var tvp = new DataTable();
            tvp.Columns.Add("Codigo", typeof(string));
            tvp.Columns.Add("Descripcion", typeof(string));
            tvp.Columns.Add("Cantidad", typeof(decimal));
            tvp.Columns.Add("PrecioUnitario", typeof(decimal));
            foreach (var i in r.Items) tvp.Rows.Add(i.Codigo, i.Descripcion, i.Cantidad, i.PrecioUnitario);

            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand("dbo.sp_Requisicion_Crear", cn) { CommandType = CommandType.StoredProcedure })
            {
                // Solicitante y Departamento los toma el SP desde el usuario
                cmd.Parameters.AddWithValue("@UsuarioId", r.UsuarioId);
                cmd.Parameters.AddWithValue("@Descripcion", r.Descripcion);
                cmd.Parameters.AddWithValue("@Subtotal", r.Subtotal);
                cmd.Parameters.AddWithValue("@Impuesto", r.Impuesto);
                cmd.Parameters.AddWithValue("@Total", r.Total);
                cmd.Parameters.AddWithValue("@PoliticaImpuesto", r.PoliticaImpuesto);
                var pItems = cmd.Parameters.AddWithValue("@Items", tvp);
                pItems.SqlDbType = SqlDbType.Structured;
                pItems.TypeName = "dbo.ItemRequisicionType";
                var pId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void CambiarEstado(int id, EstadoRequisicion nuevoEstado, int usuarioId)
        {
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand("dbo.sp_Requisicion_CambiarEstado", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@NuevoEstado", (byte)nuevoEstado);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarDescripcion(int id, string descripcion, int usuarioId)
        {
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand("dbo.sp_Requisicion_ActualizarDescripcion", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ======================= LECTURA: solo Vistas =======================
        /// <summary>Requisiciones propias + las de quienes tienen al usuario como superior.</summary>
        public List<RequisicionDto> ListarVisibles(int usuarioId)
        {
            const string sql =
                @"SELECT r.* FROM dbo.vw_Requisicion r
                  WHERE r.UsuarioId = @U
                     OR EXISTS (SELECT 1 FROM dbo.vw_UsuarioJerarquia j
                                WHERE j.UsuarioId = r.UsuarioId AND j.SuperiorId = @U)
                  ORDER BY r.Id DESC";
            var lista = new List<RequisicionDto>();
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@U", usuarioId);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read()) lista.Add(MapearCabecera(rd));
            }
            return lista;
        }

        public RequisicionDto ObtenerPorId(int id)
        {
            RequisicionDto r = null;
            using (var cn = DbConnectionFactory.Instancia.Crear())
            {
                cn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM dbo.vw_Requisicion WHERE Id = @Id", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var rd = cmd.ExecuteReader())
                        if (rd.Read()) r = MapearCabecera(rd);
                }
                if (r == null) return null;
                using (var cmd = new SqlCommand("SELECT * FROM dbo.vw_RequisicionItem WHERE RequisicionId = @Id", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read())
                            r.Items.Add(new ItemDto
                            {
                                Codigo = rd.GetString(rd.GetOrdinal("Codigo")),
                                Descripcion = rd.GetString(rd.GetOrdinal("Descripcion")),
                                Cantidad = rd.GetDecimal(rd.GetOrdinal("Cantidad")),
                                PrecioUnitario = rd.GetDecimal(rd.GetOrdinal("PrecioUnitario")),
                                Subtotal = rd.GetDecimal(rd.GetOrdinal("Subtotal"))
                            });
                }
            }
            return r;
        }

        public List<ReporteRequisicionDto> ObtenerReporte(int id)
        {
            var lista = new List<ReporteRequisicionDto>();
            using (var cn = DbConnectionFactory.Instancia.Crear())
            using (var cmd = new SqlCommand("SELECT * FROM dbo.vw_ReporteRequisicion WHERE RequisicionId = @Id", cn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read())
                        lista.Add(new ReporteRequisicionDto
                        {
                            RequisicionId = rd.GetInt32(rd.GetOrdinal("RequisicionId")),
                            Solicitante = rd.GetString(rd.GetOrdinal("Solicitante")),
                            Departamento = rd.GetString(rd.GetOrdinal("Departamento")),
                            Descripcion = rd.GetString(rd.GetOrdinal("Descripcion")),
                            Estado = rd.GetString(rd.GetOrdinal("Estado")),
                            PoliticaImpuesto = rd.GetString(rd.GetOrdinal("PoliticaImpuesto")),
                            Codigo = rd.GetString(rd.GetOrdinal("Codigo")),
                            ItemDescripcion = rd.GetString(rd.GetOrdinal("ItemDescripcion")),
                            Cantidad = rd.GetDecimal(rd.GetOrdinal("Cantidad")),
                            PrecioUnitario = rd.GetDecimal(rd.GetOrdinal("PrecioUnitario")),
                            ItemSubtotal = rd.GetDecimal(rd.GetOrdinal("ItemSubtotal")),
                            Subtotal = rd.GetDecimal(rd.GetOrdinal("Subtotal")),
                            Impuesto = rd.GetDecimal(rd.GetOrdinal("Impuesto")),
                            Total = rd.GetDecimal(rd.GetOrdinal("Total"))
                        });
            }
            return lista;
        }

        private static RequisicionDto MapearCabecera(SqlDataReader rd)
        {
            int ordResuelto = rd.GetOrdinal("ResueltoPor");
            return new RequisicionDto
            {
                Id = rd.GetInt32(rd.GetOrdinal("Id")),
                UsuarioId = rd.GetInt32(rd.GetOrdinal("UsuarioId")),
                Solicitante = rd.GetString(rd.GetOrdinal("Solicitante")),
                Departamento = rd.GetString(rd.GetOrdinal("Departamento")),
                Descripcion = rd.GetString(rd.GetOrdinal("Descripcion")),
                Subtotal = rd.GetDecimal(rd.GetOrdinal("Subtotal")),
                Impuesto = rd.GetDecimal(rd.GetOrdinal("Impuesto")),
                Total = rd.GetDecimal(rd.GetOrdinal("Total")),
                Estado = (EstadoRequisicion)rd.GetByte(rd.GetOrdinal("Estado")),
                PoliticaImpuesto = rd.GetString(rd.GetOrdinal("PoliticaImpuesto")),
                ResueltoPor = rd.IsDBNull(ordResuelto) ? null : rd.GetString(ordResuelto)
            };
        }
    }
}
