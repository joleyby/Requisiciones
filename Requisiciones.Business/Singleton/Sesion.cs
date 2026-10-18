using System;
using Requisiciones.Entities;

namespace Requisiciones.Business.Singleton
{
    /// <summary>SINGLETON: usuario autenticado de la aplicación.</summary>
    public sealed class Sesion
    {
        private static readonly Lazy<Sesion> _instancia = new Lazy<Sesion>(() => new Sesion());
        public static Sesion Instancia => _instancia.Value;

        private readonly object _lock = new object();
        private UsuarioDto _usuario;

        private Sesion() { }

        public UsuarioDto Usuario { get { lock (_lock) return _usuario; } }
        public bool EstaAutenticado => Usuario != null;

        public void Iniciar(UsuarioDto usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            lock (_lock) _usuario = usuario;
        }

        public void Cerrar() { lock (_lock) _usuario = null; }
    }
}
