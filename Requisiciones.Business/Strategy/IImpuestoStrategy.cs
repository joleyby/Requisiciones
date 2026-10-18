namespace Requisiciones.Business.Strategy
{
    /// <summary>STRATEGY: contrato de la política de impuestos.</summary>
    public interface IImpuestoStrategy
    {
        string Nombre { get; }
        decimal Calcular(decimal subtotal);
    }
}
