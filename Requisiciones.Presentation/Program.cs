using System;
using System.Windows.Forms;

namespace Requisiciones.Presentation
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var login = new LoginForm())
                    if (login.ShowDialog() != DialogResult.OK) break;   // Salir / cerrar = terminar

                var principal = new FormPrincipal();
                Application.Run(principal);
                if (!principal.CerrarSesionSolicitado) break;           // volver al login solo si cerró sesión
            }
        }
    }
}
