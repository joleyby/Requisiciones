using Requisiciones.Entities;

namespace Requisiciones.Business.State
{
    /// <summary>STATE: contrato de los estados de la requisición.</summary>
    public interface IEstadoRequisicion
    {
        EstadoRequisicion Estado { get; }
        bool PuedeModificar { get; }
        void Enviar(Requisicion r);
        void Aprobar(Requisicion r);
        void Rechazar(Requisicion r);
        void Cancelar(Requisicion r);
    }
}
