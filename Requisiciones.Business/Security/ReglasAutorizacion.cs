using System;
using System.Collections.Generic;
using System.Linq;
using Requisiciones.Entities;

namespace Requisiciones.Business.Security
{
    /// <summary>
    /// Reglas de autorización de la requisición:
    ///  - Enviar / Cancelar / Modificar: solo quien la creó.
    ///  - Aprobar / Rechazar: solo un superior del solicitante (cadena de superiores) con rol Aprobador.
    ///  - Aprobar además exige Total &lt;= límite de aprobación del superior (si lo excede debe escalar).
    /// </summary>
    public static class ReglasAutorizacion
    {
        public static void ValidarCreador(UsuarioDto usuario, Requisicion r, string accion)
        {
            if (r.UsuarioId != usuario.Id)
                throw new UnauthorizedAccessException($"Solo quien creó la requisición puede {accion}la.");
        }

        public static void ValidarVisibilidad(UsuarioDto usuario, Requisicion r, IEnumerable<UsuarioDto> cadena)
        {
            if (r.UsuarioId != usuario.Id && !EsSuperior(usuario, cadena))
                throw new UnauthorizedAccessException("No tiene acceso a esta requisición.");
        }

        public static void ValidarRechazo(UsuarioDto usuario, Requisicion r, IEnumerable<UsuarioDto> cadena)
        {
            if (usuario.Rol != RolUsuario.Aprobador || !EsSuperior(usuario, cadena))
                throw new UnauthorizedAccessException(
                    "Solo un superior del solicitante puede aprobar o rechazar esta requisición.");
        }

        public static void ValidarAprobacion(UsuarioDto usuario, Requisicion r, IEnumerable<UsuarioDto> cadena)
        {
            var lista = cadena.ToList();
            ValidarRechazo(usuario, r, lista);

            if (usuario.LimiteAprobacion == null || r.Total > usuario.LimiteAprobacion.Value)
            {
                var alternativo = lista.FirstOrDefault(s => s.LimiteAprobacion >= r.Total);
                string sugerencia = alternativo != null
                    ? $" Puede aprobarla: {alternativo.NombreCompleto}."
                    : " Ningún superior tiene un límite suficiente.";
                throw new UnauthorizedAccessException(
                    $"El total ({r.Total:N2}) excede su límite de aprobación ({usuario.LimiteAprobacion:N2}).{sugerencia}");
            }
        }

        private static bool EsSuperior(UsuarioDto usuario, IEnumerable<UsuarioDto> cadena) =>
            cadena.Any(s => s.Id == usuario.Id);
    }
}
