using System;

namespace Requisiciones.Business.Strategy
{
    public class ImpuestoPorcentajeStrategy : IImpuestoStrategy
    {
        private readonly decimal _tasa;
        public string Nombre { get; }

        public ImpuestoPorcentajeStrategy(string nombre, decimal tasa)
        {
            if (tasa < 0) throw new ArgumentException("La tasa no puede ser negativa.");
            Nombre = nombre; _tasa = tasa;
        }

        public decimal Calcular(decimal subtotal) => Math.Round(subtotal * _tasa, 2, MidpointRounding.AwayFromZero);
    }

    public class ImpuestoExentoStrategy : IImpuestoStrategy
    {
        public string Nombre => "Exento";
        public decimal Calcular(decimal subtotal) => 0m;
    }
}
