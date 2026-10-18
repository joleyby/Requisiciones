using System;
using System.Collections.Generic;
using System.Linq;
using Requisiciones.Business.State;
using Requisiciones.Business.Strategy;
using Requisiciones.Entities;

namespace Requisiciones.Business
{
    public class ItemRequisicion
    {
        public string Codigo { get; }
        public string Descripcion { get; }
        public decimal Cantidad { get; }
        public decimal PrecioUnitario { get; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public ItemRequisicion(string codigo, string descripcion, decimal cantidad, decimal precioUnitario)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("El código del ítem es obligatorio.");
            if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("La descripción del ítem es obligatoria.");
            if (cantidad <= 0) throw new ArgumentException($"Ítem '{codigo}': la cantidad debe ser mayor a cero.");
            if (precioUnitario <= 0) throw new ArgumentException($"Ítem '{codigo}': el precio unitario debe ser mayor a cero.");
            Codigo = codigo.Trim(); Descripcion = descripcion.Trim();
            Cantidad = cantidad; PrecioUnitario = precioUnitario;
        }
    }

    /// <summary>
    /// Requisición de dominio. Los montos tienen setter privado (no editables manualmente).
    /// El constructor es interno: solo se crea con RequisicionBuilder o se rehidrata desde BD.
    /// </summary>
    public class Requisicion
    {
        private readonly List<ItemRequisicion> _items = new List<ItemRequisicion>();
        private IEstadoRequisicion _estado = new EstadoCreada();

        public int Id { get; internal set; }
        public int UsuarioId { get; internal set; }   // quien la creó
        public string Solicitante { get; internal set; }
        public string Departamento { get; internal set; }
        public string Descripcion { get; private set; }
        public string PoliticaImpuesto { get; internal set; }
        public decimal Subtotal { get; private set; }
        public decimal Impuesto { get; private set; }
        public decimal Total { get; private set; }
        public IReadOnlyList<ItemRequisicion> Items => _items;
        public EstadoRequisicion Estado => _estado.Estado;
        public bool PuedeModificar => _estado.PuedeModificar;

        internal Requisicion() { }

        // ---- State ----
        internal void CambiarEstado(IEstadoRequisicion nuevo) => _estado = nuevo;
        internal void RestaurarEstado(EstadoRequisicion e) => _estado = EstadoFactory.Crear(e);
        public void Enviar() => _estado.Enviar(this);
        public void Aprobar() => _estado.Aprobar(this);
        public void Rechazar() => _estado.Rechazar(this);
        public void Cancelar() => _estado.Cancelar(this);

        // ---- Modificaciones (bloqueadas por el State) ----
        public void ActualizarDescripcion(string descripcion)
        {
            if (!PuedeModificar)
                throw new InvalidOperationException($"No se puede modificar una requisición en estado '{Estado}'.");
            if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("La descripción es obligatoria.");
            Descripcion = descripcion.Trim();
        }

        // ---- Items / montos (solo los usa el Builder o la rehidratación) ----
        internal void SetDescripcionInicial(string d) => Descripcion = d;

        internal void AgregarItemInterno(ItemRequisicion item)
        {
            if (_items.Any(i => string.Equals(i.Codigo, item.Codigo, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"El ítem '{item.Codigo}' ya existe en la requisición.");
            _items.Add(item);
        }

        internal void Recalcular(IImpuestoStrategy estrategia)
        {
            Subtotal = _items.Sum(i => i.Subtotal);
            Impuesto = estrategia.Calcular(Subtotal);
            Total = Subtotal + Impuesto;
            PoliticaImpuesto = estrategia.Nombre;
        }

        internal void RestaurarMontos(decimal subtotal, decimal impuesto, decimal total)
        { Subtotal = subtotal; Impuesto = impuesto; Total = total; }
    }
}
