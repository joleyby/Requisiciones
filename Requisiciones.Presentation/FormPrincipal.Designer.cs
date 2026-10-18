namespace Requisiciones.Presentation
{
    partial class FormPrincipal
    {
        /// <summary>Contenedor de componentes del diseñador.</summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>Solo diseño: crea, posiciona y configura los controles.</summary>
        private void InitializeComponent()
        {
            this.lblSolicitante = new System.Windows.Forms.Label();
            this.txtSolicitante = new System.Windows.Forms.TextBox();
            this.lblDepartamento = new System.Windows.Forms.Label();
            this.txtDepartamento = new System.Windows.Forms.TextBox();
            this.lblPolitica = new System.Windows.Forms.Label();
            this.cboPolitica = new System.Windows.Forms.ComboBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.txtItemDesc = new System.Windows.Forms.TextBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.numPrecio = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.gridItems = new System.Windows.Forms.DataGridView();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblImpuesto = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.gridReqs = new System.Windows.Forms.DataGridView();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnAprobar = new System.Windows.Forms.Button();
            this.btnRechazar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnReporte = new System.Windows.Forms.Button();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.lblSesion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridReqs)).BeginInit();
            this.SuspendLayout();
            //
            // lblSolicitante
            //
            this.lblSolicitante.AutoSize = true;
            this.lblSolicitante.Location = new System.Drawing.Point(10, 13);
            this.lblSolicitante.Name = "lblSolicitante";
            this.lblSolicitante.TabIndex = 0;
            this.lblSolicitante.Text = "Solicitante";
            //
            // txtSolicitante
            //
            this.txtSolicitante.Location = new System.Drawing.Point(100, 10);
            this.txtSolicitante.Name = "txtSolicitante";
            this.txtSolicitante.ReadOnly = true;
            this.txtSolicitante.Size = new System.Drawing.Size(180, 20);
            this.txtSolicitante.TabIndex = 1;
            //
            // lblDepartamento
            //
            this.lblDepartamento.AutoSize = true;
            this.lblDepartamento.Location = new System.Drawing.Point(300, 13);
            this.lblDepartamento.Name = "lblDepartamento";
            this.lblDepartamento.TabIndex = 2;
            this.lblDepartamento.Text = "Departamento";
            //
            // txtDepartamento
            //
            this.txtDepartamento.Location = new System.Drawing.Point(390, 10);
            this.txtDepartamento.Name = "txtDepartamento";
            this.txtDepartamento.ReadOnly = true;
            this.txtDepartamento.Size = new System.Drawing.Size(180, 20);
            this.txtDepartamento.TabIndex = 3;
            //
            // lblPolitica
            //
            this.lblPolitica.AutoSize = true;
            this.lblPolitica.Location = new System.Drawing.Point(590, 13);
            this.lblPolitica.Name = "lblPolitica";
            this.lblPolitica.TabIndex = 4;
            this.lblPolitica.Text = "Política imp.";
            //
            // cboPolitica
            //
            this.cboPolitica.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPolitica.FormattingEnabled = true;
            this.cboPolitica.Location = new System.Drawing.Point(680, 10);
            this.cboPolitica.Name = "cboPolitica";
            this.cboPolitica.Size = new System.Drawing.Size(120, 21);
            this.cboPolitica.TabIndex = 5;
            this.cboPolitica.SelectedIndexChanged += new System.EventHandler(this.cboPolitica_SelectedIndexChanged);
            //
            // lblDescripcion
            //
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(10, 45);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.TabIndex = 6;
            this.lblDescripcion.Text = "Descripción";
            //
            // txtDescripcion
            //
            this.txtDescripcion.Location = new System.Drawing.Point(100, 42);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(470, 20);
            this.txtDescripcion.TabIndex = 7;
            //
            // lblCodigo
            //
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Location = new System.Drawing.Point(10, 83);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.TabIndex = 8;
            this.lblCodigo.Text = "Código";
            //
            // txtCodigo
            //
            this.txtCodigo.Location = new System.Drawing.Point(60, 80);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(80, 20);
            this.txtCodigo.TabIndex = 9;
            //
            // lblItem
            //
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(150, 83);
            this.lblItem.Name = "lblItem";
            this.lblItem.TabIndex = 10;
            this.lblItem.Text = "Ítem";
            //
            // txtItemDesc
            //
            this.txtItemDesc.Location = new System.Drawing.Point(190, 80);
            this.txtItemDesc.Name = "txtItemDesc";
            this.txtItemDesc.Size = new System.Drawing.Size(200, 20);
            this.txtItemDesc.TabIndex = 11;
            //
            // lblCantidad
            //
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Location = new System.Drawing.Point(400, 83);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.TabIndex = 12;
            this.lblCantidad.Text = "Cant.";
            //
            // numCantidad
            //
            this.numCantidad.DecimalPlaces = 2;
            this.numCantidad.Location = new System.Drawing.Point(440, 80);
            this.numCantidad.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Size = new System.Drawing.Size(70, 20);
            this.numCantidad.TabIndex = 13;
            //
            // lblPrecio
            //
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Location = new System.Drawing.Point(520, 83);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.TabIndex = 14;
            this.lblPrecio.Text = "Precio";
            //
            // numPrecio
            //
            this.numPrecio.DecimalPlaces = 2;
            this.numPrecio.Location = new System.Drawing.Point(565, 80);
            this.numPrecio.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numPrecio.Name = "numPrecio";
            this.numPrecio.Size = new System.Drawing.Size(90, 20);
            this.numPrecio.TabIndex = 15;
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(670, 78);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(110, 28);
            this.btnAgregar.TabIndex = 16;
            this.btnAgregar.Text = "Agregar ítem";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // gridItems
            //
            this.gridItems.AllowUserToAddRows = false;
            this.gridItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItems.Location = new System.Drawing.Point(10, 114);
            this.gridItems.Name = "gridItems";
            this.gridItems.ReadOnly = true;
            this.gridItems.Size = new System.Drawing.Size(770, 150);
            this.gridItems.TabIndex = 17;
            this.gridItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridItems_CellDoubleClick);
            //
            // lblSubtotal
            //
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Location = new System.Drawing.Point(800, 117);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.TabIndex = 18;
            this.lblSubtotal.Text = "Subtotal: 0.00";
            //
            // lblImpuesto
            //
            this.lblImpuesto.AutoSize = true;
            this.lblImpuesto.Location = new System.Drawing.Point(800, 142);
            this.lblImpuesto.Name = "lblImpuesto";
            this.lblImpuesto.TabIndex = 19;
            this.lblImpuesto.Text = "Impuesto: 0.00";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(800, 167);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.TabIndex = 20;
            this.lblTotal.Text = "Total: 0.00";
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(800, 204);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(150, 28);
            this.btnCrear.TabIndex = 21;
            this.btnCrear.Text = "Crear requisición";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // gridReqs
            //
            this.gridReqs.AllowUserToAddRows = false;
            this.gridReqs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridReqs.Location = new System.Drawing.Point(10, 279);
            this.gridReqs.MultiSelect = false;
            this.gridReqs.Name = "gridReqs";
            this.gridReqs.ReadOnly = true;
            this.gridReqs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridReqs.Size = new System.Drawing.Size(960, 270);
            this.gridReqs.TabIndex = 22;
            //
            // btnEnviar
            //
            this.btnEnviar.Location = new System.Drawing.Point(10, 559);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(130, 28);
            this.btnEnviar.TabIndex = 23;
            this.btnEnviar.Text = "Enviar a revisión";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            //
            // btnAprobar
            //
            this.btnAprobar.Location = new System.Drawing.Point(150, 559);
            this.btnAprobar.Name = "btnAprobar";
            this.btnAprobar.Size = new System.Drawing.Size(130, 28);
            this.btnAprobar.TabIndex = 24;
            this.btnAprobar.Text = "Aprobar";
            this.btnAprobar.UseVisualStyleBackColor = true;
            this.btnAprobar.Click += new System.EventHandler(this.btnAprobar_Click);
            //
            // btnRechazar
            //
            this.btnRechazar.Location = new System.Drawing.Point(290, 559);
            this.btnRechazar.Name = "btnRechazar";
            this.btnRechazar.Size = new System.Drawing.Size(130, 28);
            this.btnRechazar.TabIndex = 25;
            this.btnRechazar.Text = "Rechazar";
            this.btnRechazar.UseVisualStyleBackColor = true;
            this.btnRechazar.Click += new System.EventHandler(this.btnRechazar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(430, 559);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 28);
            this.btnCancelar.TabIndex = 26;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // btnReporte
            //
            this.btnReporte.Location = new System.Drawing.Point(570, 559);
            this.btnReporte.Name = "btnReporte";
            this.btnReporte.Size = new System.Drawing.Size(120, 28);
            this.btnReporte.TabIndex = 27;
            this.btnReporte.Text = "Ver reporte";
            this.btnReporte.UseVisualStyleBackColor = true;
            this.btnReporte.Click += new System.EventHandler(this.btnReporte_Click);
            //
            // btnRefrescar
            //
            this.btnRefrescar.Location = new System.Drawing.Point(700, 559);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(100, 28);
            this.btnRefrescar.TabIndex = 28;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            //
            // btnCerrarSesion
            //
            this.btnCerrarSesion.Location = new System.Drawing.Point(820, 559);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(150, 28);
            this.btnCerrarSesion.TabIndex = 29;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = true;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            //
            // lblSesion
            //
            this.lblSesion.AutoSize = true;
            this.lblSesion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSesion.Location = new System.Drawing.Point(10, 600);
            this.lblSesion.Name = "lblSesion";
            this.lblSesion.TabIndex = 30;
            this.lblSesion.Text = "Sesión";
            //
            // FormPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 721);
            this.Controls.Add(this.lblSolicitante);
            this.Controls.Add(this.txtSolicitante);
            this.Controls.Add(this.lblDepartamento);
            this.Controls.Add(this.txtDepartamento);
            this.Controls.Add(this.lblPolitica);
            this.Controls.Add(this.cboPolitica);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblItem);
            this.Controls.Add(this.txtItemDesc);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.numCantidad);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.numPrecio);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblImpuesto);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.gridReqs);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.btnAprobar);
            this.Controls.Add(this.btnRechazar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnReporte);
            this.Controls.Add(this.btnRefrescar);
            this.Controls.Add(this.btnCerrarSesion);
            this.Controls.Add(this.lblSesion);
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Requisiciones";
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridReqs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblSolicitante;
        private System.Windows.Forms.TextBox txtSolicitante;
        private System.Windows.Forms.Label lblDepartamento;
        private System.Windows.Forms.TextBox txtDepartamento;
        private System.Windows.Forms.Label lblPolitica;
        private System.Windows.Forms.ComboBox cboPolitica;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.TextBox txtItemDesc;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.NumericUpDown numPrecio;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.DataGridView gridItems;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblImpuesto;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.DataGridView gridReqs;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnAprobar;
        private System.Windows.Forms.Button btnRechazar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnReporte;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Label lblSesion;
    }
}
