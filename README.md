# Sistema de Requisiciones – Arquitectura N Capas

Cada capa es un **proyecto independiente** enlazado por referencias:

    Presentation  ->  Business  ->  DataAccess  ->  Entities
         \_____________\_____________________________/^
                       (todas referencian Entities)

| Proyecto | Responsabilidad | Patrones |
|---|---|---|
| Requisiciones.Entities | Enums y modelos | – |
| Requisiciones.DataAccess | ADO.NET. Escrituras = SP, lecturas/reportes = Vistas | **Singleton** (`DbConnectionFactory`) |
| Requisiciones.Business | Reglas de negocio | **Builder**, **Strategy**, **State**, **Singleton** (`PoliticaImpuestos`) |
| Requisiciones.Presentation | WinForms + ReportViewer | – |

## Puesta en marcha
1. Ejecutar `Database/01_RequisicionesDB.sql` en SQL Server y los datos de ejemplo `Database/02_DB_datos_pruebas.sql`.
2. Ajustar la cadena `RequisicionesDb` en `Requisiciones.Presentation/App.config`.
3. Crear el RDLC (ver `Reports/RDLC.txt`).
4. Abrir `Requisiciones.sln`, establecer Presentation como proyecto de inicio y ejecutar.

## Trazabilidad de reglas
- Builder + ≥1 ítem: `RequisicionBuilder.Build()`
- Montos automáticos: `Requisicion` (setters privados) + `Recalcular()`; CHECK `Total = Subtotal + Impuesto` en BD
- Ítem > 0 y sin duplicados: `ItemRequisicion` + `AgregarItemInterno` + `UNIQUE(RequisicionId, Codigo)` + CHECKs
- Strategy impuestos: `IImpuestoStrategy` (IVA %, Exento); política activa en `PoliticaImpuestos` (Singleton)
- State: `Estados.cs`; transiciones inválidas lanzan excepción y también las rechaza `sp_Requisicion_CambiarEstado`
- Reporte solo Aprobada: `RequisicionService.ObtenerReporte()` -> `vw_ReporteRequisicion`

## Login, roles, superiores y límites (v2)
- **Login**: `LoginForm` -> `AutenticacionService`. Contraseñas con PBKDF2-SHA256 + salt por usuario. La sesión vive en el Singleton `Sesion`.
- **Roles**: `Solicitante` (crea, envía y cancela las suyas) y `Aprobador` (además aprueba/rechaza las de su equipo).
- **Superiores**: `Usuario.SuperiorId` forma una jerarquía (vista recursiva `vw_UsuarioJerarquia`). Solo un superior del solicitante puede aprobar/rechazar.
- **Límite de aprobación**: un Aprobador solo aprueba requisiciones con `Total <= LimiteAprobacion`; si lo excede, debe aprobarla un superior con mayor límite (escalamiento). Rechazar no tiene límite.
- **Doble validación**: `ReglasAutorizacion` (negocio) y `sp_Requisicion_CambiarEstado` (BD).
- **Visibilidad**: cada usuario ve sus requisiciones y las de quienes tiene a su cargo.
- Usuarios de prueba (contraseña `Demo123!`): director, gerente, supervisor, ana, carlos, luis, maria.
