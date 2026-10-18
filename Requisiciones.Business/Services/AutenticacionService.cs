using System;
using Requisiciones.Business.Security;
using Requisiciones.Business.Singleton;
using Requisiciones.DataAccess;
using Requisiciones.Entities;

namespace Requisiciones.Business.Services
{
    public class AutenticacionService
    {
        private static readonly byte[] SaltFalsa = new byte[16];
        private readonly IUsuarioRepository _repo;

        public AutenticacionService() : this(new UsuarioRepository()) { }
        public AutenticacionService(IUsuarioRepository repo) { _repo = repo; }

        public UsuarioDto Iniciar(string nombreUsuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrEmpty(clave))
                throw new InvalidOperationException("Ingrese usuario y contraseña.");

            var usuario = _repo.ObtenerPorNombreUsuario(nombreUsuario.Trim());
            if (usuario == null || !usuario.Activo)
            {
                // trabajo equivalente para no revelar si el usuario existe
                PasswordHasher.Hash(clave, SaltFalsa, PasswordHasher.IteracionesPorDefecto);
                throw CredencialesInvalidas();
            }

            var credencial = _repo.ObtenerCredencial(usuario.Id);
            if (credencial == null || !PasswordHasher.Verificar(clave, credencial))
                throw CredencialesInvalidas();

            Sesion.Instancia.Iniciar(usuario);
            return usuario;
        }

        public void CerrarSesion() => Sesion.Instancia.Cerrar();

        private static Exception CredencialesInvalidas() =>
            new InvalidOperationException("Usuario o contraseña incorrectos.");
    }
}
