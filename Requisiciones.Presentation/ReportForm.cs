using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Requisiciones.Entities;

namespace Requisiciones.Presentation
{
    /// <summary>Visor (ReportViewer). Los datos provienen de vw_ReporteRequisicion.</summary>
    public class ReportForm : Form
    {
        public ReportForm(List<ReporteRequisicionDto> datos)
        {
            Text = "Reporte de Requisición";
            Width = 900; Height = 700;
            var viewer = new ReportViewer { Dock = DockStyle.Fill, ProcessingMode = ProcessingMode.Local };
            Controls.Add(viewer);

            viewer.LocalReport.ReportPath = Path.Combine(Application.StartupPath, "Reports", "RptRequisicion.rdlc");
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DsReporte", datos)); // nombre del DataSet en el RDLC
            Load += (s, e) => viewer.RefreshReport();
        }
    }
}
