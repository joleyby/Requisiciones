using System.Collections.Generic;
using Requisiciones.Entities;

namespace Requisiciones.DataAccess
{
    public interface IRequisicionRepository
    {
        int Crear(RequisicionDto requisicion);                                      // sp_Requisicion_Crear
        void CambiarEstado(int id, EstadoRequisicion nuevoEstado, int usuarioId);   // sp_Requisicion_CambiarEstado
        void ActualizarDescripcion(int id, string descripcion, int usuarioId);      // sp_Requisicion_ActualizarDescripcion
        RequisicionDto ObtenerPorId(int id);                                        // vw_Requisicion + vw_RequisicionItem
        List<RequisicionDto> ListarVisibles(int usuarioId);                         // propias + de su equipo
        List<ReporteRequisicionDto> ObtenerReporte(int id);                         // vw_ReporteRequisicion
    }
}
