using System.Collections.Generic;

namespace Requisiciones.Entities
{
    /// <summary>Usuario sin datos secretos (hash/salt).</summary>
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Departamento { get; set; }
        public RolUsuario Rol { get; set; }
        public int? SuperiorId { get; set; }
        public decimal? LimiteAprobacion { get; set; }
        public bool Activo { get; set; }
    }

    /// <summary>Credenciales: solo se usan al iniciar sesión.</summary>
    public class CredencialDto
    {
        public int UsuarioId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public int Iteraciones { get; set; }
    }

    public class ItemDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class RequisicionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Solicitante { get; set; }
        public string Departamento { get; set; }
        public string Descripcion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public EstadoRequisicion Estado { get; set; }
        public string PoliticaImpuesto { get; set; }
        public string ResueltoPor { get; set; }
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }

    /// <summary>Fila de la vista vw_ReporteRequisicion (una por ítem).</summary>
    public class ReporteRequisicionDto
    {
        public int RequisicionId { get; set; }
        public string Solicitante { get; set; }
        public string Departamento { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public string PoliticaImpuesto { get; set; }
        public string Codigo { get; set; }
        public string ItemDescripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ItemSubtotal { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
    }
}
