using System.Collections.Generic;
using Requisiciones.Entities;

namespace Requisiciones.DataAccess
{
    public interface IUsuarioRepository
    {
        UsuarioDto ObtenerPorNombreUsuario(string nombreUsuario);   // vw_Usuario
        CredencialDto ObtenerCredencial(int usuarioId);             // vw_UsuarioCredencial
        List<UsuarioDto> ObtenerCadenaSuperiores(int usuarioId);    // vw_UsuarioJerarquia + vw_Usuario
    }
}
