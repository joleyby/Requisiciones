using System;
using Requisiciones.Business.Strategy;

namespace Requisiciones.Business.Singleton
{
    /// <summary>
    /// SINGLETON: punto único de configuración de la política de impuestos vigente.
    /// Cambiar la política no modifica la lógica de negocio (Strategy).
    /// </summary>
    public sealed class PoliticaImpuestos
    {
        private static readonly Lazy<PoliticaImpuestos> _instancia =
            new Lazy<PoliticaImpuestos>(() => new PoliticaImpuestos());
        public static PoliticaImpuestos Instancia => _instancia.Value;

        private readonly object _lock = new object();
        private IImpuestoStrategy _actual = new ImpuestoPorcentajeStrategy("IVA 16%", 0.16m);

        private PoliticaImpuestos() { }

        public IImpuestoStrategy Actual { get { lock (_lock) return _actual; } }

        public void Cambiar(IImpuestoStrategy estrategia)
        {
            if (estrategia == null) throw new ArgumentNullException(nameof(estrategia));
            lock (_lock) _actual = estrategia;
        }
    }
}
