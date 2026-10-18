using System;
using System.Windows.Forms;
using Requisiciones.Business.Services;

namespace Requisiciones.Presentation
{
    /// <summary>Lógica del login. El diseño está en LoginForm.Designer.cs.</summary>
    public partial class LoginForm : Form
    {
        private readonly AutenticacionService _auth = new AutenticacionService();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            try
            {
                _auth.Iniciar(txtUsuario.Text, txtClave.Text);
                DialogResult = DialogResult.OK;   // cierra el diálogo
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                txtClave.Clear();
                txtClave.Focus();
            }
        }
    }
}
