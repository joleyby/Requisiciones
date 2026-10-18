using System;
using System.Collections.Generic;
using System.Linq;
using Requisiciones.Business.Builder;
using Requisiciones.Business.Security;
using Requisiciones.Business.Singleton;
using Requisiciones.Business.Strategy;
using Requisiciones.DataAccess;
using Requisiciones.Entities;

namespace Requisiciones.Business.Services
{
    public class ItemInput
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class RequisicionService
    {
        private readonly IRequisicionRepository _repo;
        private readonly IUsuarioRepository _usuarios;

        public RequisicionService() : this(new RequisicionRepository(), new UsuarioRepository()) { }
        public RequisicionService(IRequisicionRepository repo, IUsuarioRepository usuarios)
        {
            _repo = repo; _usuarios = usuarios;
        }

        private static UsuarioDto Actual =>
            Sesion.Instancia.Usuario ?? throw new InvalidOperationException("No hay una sesión iniciada.");

        // ---------------- creación ----------------
        /// <summary>Construye (sin guardar) para previsualizar montos.</summary>
        public Requisicion Construir(string descripcion, IEnumerable<ItemInput> items, IImpuestoStrategy politica)
        {
            var b = new RequisicionBuilder()
                .ConUsuario(Actual)
                .ConDescripcion(descripcion)
                .ConPolitica(politica);
            foreach (var i in items) b.AgregarItem(i.Codigo, i.Descripcion, i.Cantidad, i.PrecioUnitario);
            return b.Build();
        }

        public int Crear(string descripcion, IEnumerable<ItemInput> items, IImpuestoStrategy politica)
        {
            var r = Construir(descripcion, items, politica);
            return _repo.Crear(ADto(r));
        }

        // ---------------- consulta ----------------
        /// <summary>Requisiciones propias y las de los usuarios a su cargo.</summary>
        public List<RequisicionDto> Listar() => _repo.ListarVisibles(Actual.Id);

        // ---------------- transiciones (State + autorización) ----------------
        public void Enviar(int id) => Transicionar(id, r =>
        {
            ReglasAutorizacion.ValidarCreador(Actual, r, "enviar");
            r.Enviar();
        });

        public void Cancelar(int id) => Transicionar(id, r =>
        {
            ReglasAutorizacion.ValidarCreador(Actual, r, "cancelar");
            r.Cancelar();
        });

        public void Aprobar(int id) => Transicionar(id, r =>
        {
            ReglasAutorizacion.ValidarAprobacion(Actual, r, _usuarios.ObtenerCadenaSuperiores(r.UsuarioId));
            r.Aprobar();
        });

        public void Rechazar(int id) => Transicionar(id, r =>
        {
            ReglasAutorizacion.ValidarRechazo(Actual, r, _usuarios.ObtenerCadenaSuperiores(r.UsuarioId));
            r.Rechazar();
        });

        public void ActualizarDescripcion(int id, string descripcion)
        {
            var r = Cargar(id);
            ReglasAutorizacion.ValidarCreador(Actual, r, "modificar");
            r.ActualizarDescripcion(descripcion);                       // State valida
            _repo.ActualizarDescripcion(id, r.Descripcion, Actual.Id);  // SP
        }

        // ---------------- reporte ----------------
        /// <summary>Solo con la requisición Aprobada, y solo para el solicitante o sus superiores.</summary>
        public List<ReporteRequisicionDto> ObtenerReporte(int id)
        {
            var r = Cargar(id);
            ReglasAutorizacion.ValidarVisibilidad(Actual, r, _usuarios.ObtenerCadenaSuperiores(r.UsuarioId));
            if (r.Estado != EstadoRequisicion.Aprobada)
                throw new InvalidOperationException("El reporte solo se puede generar para requisiciones Aprobadas.");
            return _repo.ObtenerReporte(id);
        }

        // ---------------- helpers ----------------
        private void Transicionar(int id, Action<Requisicion> accion)
        {
            var r = Cargar(id);
            accion(r);                                   // autorización + State
            _repo.CambiarEstado(id, r.Estado, Actual.Id); // SP (vuelve a validar)
        }

        private Requisicion Cargar(int id)
        {
            var dto = _repo.ObtenerPorId(id) ?? throw new InvalidOperationException("La requisición no existe.");
            var r = new Requisicion
            {
                Id = dto.Id, UsuarioId = dto.UsuarioId,
                Solicitante = dto.Solicitante, Departamento = dto.Departamento,
                PoliticaImpuesto = dto.PoliticaImpuesto
            };
            r.SetDescripcionInicial(dto.Descripcion);
            foreach (var i in dto.Items)
                r.AgregarItemInterno(new ItemRequisicion(i.Codigo, i.Descripcion, i.Cantidad, i.PrecioUnitario));
            r.RestaurarMontos(dto.Subtotal, dto.Impuesto, dto.Total);
            r.RestaurarEstado(dto.Estado);
            return r;
        }

        private static RequisicionDto ADto(Requisicion r) => new RequisicionDto
        {
            Id = r.Id, UsuarioId = r.UsuarioId, Solicitante = r.Solicitante, Departamento = r.Departamento,
            Descripcion = r.Descripcion, Subtotal = r.Subtotal, Impuesto = r.Impuesto, Total = r.Total,
            Estado = r.Estado, PoliticaImpuesto = r.PoliticaImpuesto,
            Items = r.Items.Select(i => new ItemDto
            {
                Codigo = i.Codigo, Descripcion = i.Descripcion, Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario, Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
