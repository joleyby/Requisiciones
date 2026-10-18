using System;
using Requisiciones.Business.Singleton;
using Requisiciones.Business.Strategy;
using Requisiciones.Entities;

namespace Requisiciones.Business.Builder
{
    /// <summary>BUILDER: única vía para crear una requisición nueva.</summary>
    public class RequisicionBuilder
    {
        private readonly Requisicion _r = new Requisicion();
        private IImpuestoStrategy _estrategia = PoliticaImpuestos.Instancia.Actual;

        /// <summary>El solicitante y su departamento salen del usuario autenticado.</summary>
        public RequisicionBuilder ConUsuario(UsuarioDto usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            _r.UsuarioId = usuario.Id;
            _r.Solicitante = usuario.NombreCompleto;
            _r.Departamento = usuario.Departamento;
            return this;
        }

        public RequisicionBuilder ConDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("La descripción es obligatoria.");
            _r.SetDescripcionInicial(descripcion.Trim());
            return this;
        }

        public RequisicionBuilder ConPolitica(IImpuestoStrategy estrategia)
        {
            _estrategia = estrategia ?? throw new ArgumentNullException(nameof(estrategia));
            return this;
        }

        public RequisicionBuilder AgregarItem(string codigo, string descripcion, decimal cantidad, decimal precio)
        {
            _r.AgregarItemInterno(new ItemRequisicion(codigo, descripcion, cantidad, precio)); // valida >0 y duplicados
            return this;
        }

        public Requisicion Build()
        {
            if (string.IsNullOrEmpty(_r.Solicitante)) throw new InvalidOperationException("Falta el solicitante.");
            if (string.IsNullOrEmpty(_r.Descripcion)) throw new InvalidOperationException("Falta la descripción.");
            if (_r.Items.Count == 0) throw new InvalidOperationException("La requisición debe contener al menos un ítem válido.");
            _r.Recalcular(_estrategia);   // montos automáticos
            return _r;
        }
    }
}
