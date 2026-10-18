using System;
using Requisiciones.Entities;

namespace Requisiciones.Business.State
{
    public abstract class EstadoBase : IEstadoRequisicion
    {
        public abstract EstadoRequisicion Estado { get; }
        public virtual bool PuedeModificar => false;

        public virtual void Enviar(Requisicion r) => Invalida("enviar a revisión");
        public virtual void Aprobar(Requisicion r) => Invalida("aprobar");
        public virtual void Rechazar(Requisicion r) => Invalida("rechazar");
        public virtual void Cancelar(Requisicion r) => Invalida("cancelar");

        private void Invalida(string accion) =>
            throw new InvalidOperationException($"No se puede {accion} una requisición en estado '{Estado}'.");
    }

    public class EstadoCreada : EstadoBase
    {
        public override EstadoRequisicion Estado => EstadoRequisicion.Creada;
        public override bool PuedeModificar => true;
        public override void Enviar(Requisicion r) => r.CambiarEstado(new EstadoEnRevision());
        public override void Cancelar(Requisicion r) => r.CambiarEstado(new EstadoCancelada());
    }

    public class EstadoEnRevision : EstadoBase
    {
        public override EstadoRequisicion Estado => EstadoRequisicion.EnRevision;
        public override void Aprobar(Requisicion r) => r.CambiarEstado(new EstadoAprobada());
        public override void Rechazar(Requisicion r) => r.CambiarEstado(new EstadoRechazada());
        public override void Cancelar(Requisicion r) => r.CambiarEstado(new EstadoCancelada());
    }

    public class EstadoAprobada : EstadoBase  { public override EstadoRequisicion Estado => EstadoRequisicion.Aprobada; }
    public class EstadoRechazada : EstadoBase { public override EstadoRequisicion Estado => EstadoRequisicion.Rechazada; }
    public class EstadoCancelada : EstadoBase { public override EstadoRequisicion Estado => EstadoRequisicion.Cancelada; }

    public static class EstadoFactory
    {
        public static IEstadoRequisicion Crear(EstadoRequisicion e)
        {
            switch (e)
            {
                case EstadoRequisicion.Creada: return new EstadoCreada();
                case EstadoRequisicion.EnRevision: return new EstadoEnRevision();
                case EstadoRequisicion.Aprobada: return new EstadoAprobada();
                case EstadoRequisicion.Rechazada: return new EstadoRechazada();
                case EstadoRequisicion.Cancelada: return new EstadoCancelada();
                default: throw new ArgumentOutOfRangeException(nameof(e));
            }
        }
    }
}
