using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Requisiciones.Business.Services;
using Requisiciones.Business.Singleton;
using Requisiciones.Business.Strategy;
using Requisiciones.Entities;

namespace Requisiciones.Presentation
{
    /// <summary>Lógica del formulario. El diseño está en FormPrincipal.Designer.cs.</summary>
    public partial class FormPrincipal : Form
    {
        private readonly RequisicionService _service = new RequisicionService();
        private readonly AutenticacionService _auth = new AutenticacionService();
        private readonly BindingList<ItemInput> _items = new BindingList<ItemInput>();

        private readonly Dictionary<string, IImpuestoStrategy> _politicas = new Dictionary<string, IImpuestoStrategy>
        {
            { "IVA 16%", new ImpuestoPorcentajeStrategy("IVA 16%", 0.16m) },
            { "IVA 18%", new ImpuestoPorcentajeStrategy("IVA 18%", 0.18m) },
            { "Exento",  new ImpuestoExentoStrategy() }
        };

        /// <summary>True si el usuario pulsó "Cerrar sesión" (Program vuelve al login).</summary>
        public bool CerrarSesionSolicitado { get; private set; }

        public FormPrincipal()
        {
            InitializeComponent();
            InicializarDatos();
            ConfigurarSesion();
            Load += (s, e) => Refrescar();
        }

        /// <summary>Enlaza datos a los controles (lo que no es diseño puro).</summary>
        private void InicializarDatos()
        {
            gridItems.DataSource = _items;
            cboPolitica.Items.AddRange(_politicas.Keys.ToArray());
            cboPolitica.SelectedIndex = 0;   // dispara cboPolitica_SelectedIndexChanged
        }

        /// <summary>Muestra el usuario autenticado y habilita las acciones según su rol.</summary>
        private void ConfigurarSesion()
        {
            var u = Sesion.Instancia.Usuario;
            txtSolicitante.Text = u.NombreCompleto;   // el solicitante es siempre el usuario autenticado
            txtDepartamento.Text = u.Departamento;

            string limite = u.LimiteAprobacion.HasValue ? $"  |  Límite de aprobación: {u.LimiteAprobacion:N2}" : string.Empty;
            lblSesion.Text = $"Usuario: {u.NombreUsuario}  |  Rol: {u.Rol}{limite}";

            bool esAprobador = u.Rol == RolUsuario.Aprobador;
            btnAprobar.Enabled = esAprobador;
            btnRechazar.Enabled = esAprobador;
        }

        // ------------------------ eventos ------------------------
        private void cboPolitica_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Strategy: se cambia la política sin tocar la lógica de negocio
            PoliticaImpuestos.Instancia.Cambiar(_politicas[(string)cboPolitica.SelectedItem]);
            Previsualizar();
        }

        private void btnAgregar_Click(object sender, EventArgs e) => AgregarItem();
        private void btnCrear_Click(object sender, EventArgs e) => Crear();
        private void btnEnviar_Click(object sender, EventArgs e) => Ejecutar(_service.Enviar);
        private void btnAprobar_Click(object sender, EventArgs e) => Ejecutar(_service.Aprobar);
        private void btnRechazar_Click(object sender, EventArgs e) => Ejecutar(_service.Rechazar);
        private void btnCancelar_Click(object sender, EventArgs e) => Ejecutar(_service.Cancelar);
        private void btnReporte_Click(object sender, EventArgs e) => VerReporte();
        private void btnRefrescar_Click(object sender, EventArgs e) => Refrescar();

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            _auth.CerrarSesion();
            CerrarSesionSolicitado = true;
            Close();
        }

        private void gridItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;   // doble clic en el encabezado

            var item = gridItems.Rows[e.RowIndex].DataBoundItem as ItemInput;
            if (item == null) return;

            // Autocompletar los campos con el ítem seleccionado
            txtCodigo.Text = item.Codigo;
            txtItemDesc.Text = item.Descripcion;
            numCantidad.Value = item.Cantidad;
            numPrecio.Value = item.PrecioUnitario;

            // Se quita de la lista para corregirlo y volver a agregarlo
            _items.Remove(item);
            Previsualizar();

            txtItemDesc.Focus();
        }

        // ------------------------ acciones ------------------------
        private void AgregarItem()
        {
            _items.Add(new ItemInput
            {
                Codigo = txtCodigo.Text, Descripcion = txtItemDesc.Text,
                Cantidad = numCantidad.Value, PrecioUnitario = numPrecio.Value
            });
            if (!Previsualizar()) _items.RemoveAt(_items.Count - 1);
            else { txtCodigo.Clear(); txtItemDesc.Clear(); numCantidad.Value = 0; numPrecio.Value = 0; }
        }

        private bool Previsualizar()
        {
            if (_items.Count == 0) { Montos(0, 0, 0); return true; }
            try
            {
                var r = _service.Construir(Nz(txtDescripcion), _items, PoliticaSeleccionada());
                Montos(r.Subtotal, r.Impuesto, r.Total);
                return true;
            }
            catch (Exception ex) { Error(ex); return false; }
        }

        private void Crear()
        {
            try
            {
                int id = _service.Crear(txtDescripcion.Text, _items.ToList(), PoliticaSeleccionada());
                MessageBox.Show($"Requisición #{id} creada.");
                _items.Clear(); Montos(0, 0, 0);
                Refrescar();
            }
            catch (Exception ex) { Error(ex); }
        }

        private void Ejecutar(Action<int> accion)
        {
            var id = IdSeleccionado(); if (id == null) return;
            try { accion(id.Value); Refrescar(); } catch (Exception ex) { Error(ex); }
        }

        private void VerReporte()
        {
            var id = IdSeleccionado(); if (id == null) return;
            try
            {
                var datos = _service.ObtenerReporte(id.Value); // solo si está Aprobada
                new ReportForm(datos).ShowDialog(this);
            }
            catch (Exception ex) { Error(ex); }
        }

        private void Refrescar()
        {
            try
            {
                gridReqs.DataSource = _service.Listar();   // propias + las de su equipo
                OcultarColumna("Items");
                OcultarColumna("UsuarioId");
            }
            catch (Exception ex) { Error(ex); }
        }

        // ------------------------ utilidades ------------------------
        private IImpuestoStrategy PoliticaSeleccionada() => _politicas[(string)cboPolitica.SelectedItem];

        private int? IdSeleccionado()
        {
            if (gridReqs.CurrentRow == null) { MessageBox.Show("Seleccione una requisición."); return null; }
            return ((RequisicionDto)gridReqs.CurrentRow.DataBoundItem).Id;
        }

        private void OcultarColumna(string nombre)
        {
            if (gridReqs.Columns[nombre] != null) gridReqs.Columns[nombre].Visible = false;
        }

        private void Montos(decimal s, decimal i, decimal t)
        {
            lblSubtotal.Text = $"Subtotal: {s:N2}";
            lblImpuesto.Text = $"Impuesto: {i:N2}";
            lblTotal.Text = $"Total: {t:N2}";
        }

        private static string Nz(Control c, string def = "-") => string.IsNullOrWhiteSpace(c.Text) ? def : c.Text;

        private void Error(Exception ex) =>
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
