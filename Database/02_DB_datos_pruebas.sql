/* =====================================================================
   RequisicionesDB - Datos de prueba (usuarios, jerarquía y requisiciones)
   Ejecutar DESPUES de RequisicionesDB_completo.sql
   Toda escritura se hace con procedimientos almacenados.
   Contraseña de TODOS los usuarios de prueba:  Demo123!

   Jerarquía y límites de aprobación:
     director  (Aprobador, límite 100,000.00)
       |- gerente  (Aprobador, límite 5,000.00)
       |    |- supervisor (Aprobador, límite 1,000.00)
       |         |- ana (Solicitante)
       |         |- carlos (Solicitante)
       |- luis  (Solicitante)
       |- maria (Solicitante)
   ===================================================================== */
USE RequisicionesDB;
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* ---------- Departamentos ---------- */
EXEC dbo.sp_Departamento_Crear N'Tecnología';
EXEC dbo.sp_Departamento_Crear N'Administración';
EXEC dbo.sp_Departamento_Crear N'Recursos Humanos';
EXEC dbo.sp_Departamento_Crear N'Compras';
EXEC dbo.sp_Departamento_Crear N'Finanzas';
GO

DECLARE @director INT, @gerente INT, @supervisor INT, @ana INT, @carlos INT, @luis INT, @maria INT;

/* ---------- Usuarios (hash PBKDF2-SHA256, 10000 iteraciones) ---------- */
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'director', @NombreCompleto = N'Laura Méndez', @Departamento = N'Administración', @Rol = 2, @SuperiorId = NULL, @Limite = 100000.00,
     @PasswordHash = 0x48926cf1e2c7b89c3bcd6f8d55045359b09d823659041017c4133edbee86512d,
     @Salt = 0x262ee10620b286ef98c169080596801a, @Iteraciones = 10000, @Id = @director OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'gerente', @NombreCompleto = N'Roberto Silva', @Departamento = N'Tecnología', @Rol = 2, @SuperiorId = @director, @Limite = 5000.00,
     @PasswordHash = 0x0cada78fce54f624e824fe0062ce1c713082444f8d784daeff74e45ad7c81181,
     @Salt = 0xd1d2b2091b01aed05acacb4ca0be89f4, @Iteraciones = 10000, @Id = @gerente OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'supervisor', @NombreCompleto = N'Elena Castro', @Departamento = N'Tecnología', @Rol = 2, @SuperiorId = @gerente, @Limite = 1000.00,
     @PasswordHash = 0xc34bf39e96fd434da56877bd374c85d289e80a50f5fa5c6af8967ffcca503bc7,
     @Salt = 0x428373d81b845eede8403bf15e45edef, @Iteraciones = 10000, @Id = @supervisor OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'ana', @NombreCompleto = N'Ana Pérez', @Departamento = N'Tecnología', @Rol = 1, @SuperiorId = @supervisor, @Limite = NULL,
     @PasswordHash = 0xb466a869dc3aefa4a331386a8343abafeecb9f2490b4f641ee60cac566ac2860,
     @Salt = 0xa4e972b0475960e6b1dde85fd58e21d0, @Iteraciones = 10000, @Id = @ana OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'carlos', @NombreCompleto = N'Carlos Díaz', @Departamento = N'Tecnología', @Rol = 1, @SuperiorId = @supervisor, @Limite = NULL,
     @PasswordHash = 0x3a4136ece29e30eff76a909baef7b93ea02ae9f2bef68ac40f4a0cbf3e8ab8b5,
     @Salt = 0x5d5bfd107a98861ed13db346f89866d5, @Iteraciones = 10000, @Id = @carlos OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'luis', @NombreCompleto = N'Luis Gómez', @Departamento = N'Administración', @Rol = 1, @SuperiorId = @director, @Limite = NULL,
     @PasswordHash = 0xe59ef01ddeeaf338990c72c14a8f36eed9c53434c18217a0497d13f2f48a2ff7,
     @Salt = 0x21226057503a61589a497525baa888a8, @Iteraciones = 10000, @Id = @luis OUTPUT;
EXEC dbo.sp_Usuario_Crear @NombreUsuario = N'maria', @NombreCompleto = N'María Rojas', @Departamento = N'Recursos Humanos', @Rol = 1, @SuperiorId = @director, @Limite = NULL,
     @PasswordHash = 0x28d893704aaf2da54f5850fa946be40f863f83313012ca968669cdf334417c5c,
     @Salt = 0xbe2e6c0db430552badd0db1808a1ae56, @Iteraciones = 10000, @Id = @maria OUTPUT;

DECLARE @Items dbo.ItemRequisicionType;
DECLARE @Id INT;

/* ---------- 1) Ana | CREADA | IVA 16% | Total 2,044.50
   Supervisor (1,000) NO puede aprobarla; gerente (5,000) y director sí. */
INSERT @Items VALUES ('LAP-001', 'Laptop 14 pulgadas', 2, 850.00),
                     ('MOU-002', 'Mouse inalámbrico',   5,  12.50);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @ana, @Descripcion = N'Equipos para nuevo personal',
     @Subtotal = 1762.50, @Impuesto = 282.00, @Total = 2044.50, @PoliticaImpuesto = 'IVA 16%', @Items = @Items, @Id = @Id OUTPUT;
DELETE FROM @Items;

/* ---------- 2) Ana | EN REVISIÓN | IVA 16% | Total 133.40  (el supervisor puede aprobarla) */
INSERT @Items VALUES ('PAP-010', 'Papel bond carta (resma)', 10,  4.00),
                     ('TIN-011', 'Cartucho de tinta negra',   3, 25.00);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @ana, @Descripcion = N'Suministros de oficina',
     @Subtotal = 115.00, @Impuesto = 18.40, @Total = 133.40, @PoliticaImpuesto = 'IVA 16%', @Items = @Items, @Id = @Id OUTPUT;
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 2, @UsuarioId = @ana;        -- Enviar
DELETE FROM @Items;

/* ---------- 3) María | APROBADA por el director | IVA 18% | Total 1,274.40  (permite reporte) */
INSERT @Items VALUES ('SIL-020', 'Silla ergonómica', 4, 120.00),
                     ('ESC-021', 'Escritorio',       2, 300.00);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @maria, @Descripcion = N'Mobiliario para sala de trabajo',
     @Subtotal = 1080.00, @Impuesto = 194.40, @Total = 1274.40, @PoliticaImpuesto = 'IVA 18%', @Items = @Items, @Id = @Id OUTPUT;
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 2, @UsuarioId = @maria;      -- Enviar
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 3, @UsuarioId = @director;   -- Aprobar
DELETE FROM @Items;

/* ---------- 4) Carlos | RECHAZADA por el supervisor | Exento | Total 600.00 */
INSERT @Items VALUES ('MON-030', 'Monitor 24 pulgadas', 3, 200.00);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @carlos, @Descripcion = N'Monitores adicionales',
     @Subtotal = 600.00, @Impuesto = 0.00, @Total = 600.00, @PoliticaImpuesto = 'Exento', @Items = @Items, @Id = @Id OUTPUT;
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 2, @UsuarioId = @carlos;     -- Enviar
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 4, @UsuarioId = @supervisor; -- Rechazar
DELETE FROM @Items;

/* ---------- 5) Luis | CANCELADA | IVA 16% | Total 174.00 */
INSERT @Items VALUES ('TEC-040', 'Teclado mecánico', 10, 15.00);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @luis, @Descripcion = N'Teclados de reemplazo',
     @Subtotal = 150.00, @Impuesto = 24.00, @Total = 174.00, @PoliticaImpuesto = 'IVA 16%', @Items = @Items, @Id = @Id OUTPUT;
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 5, @UsuarioId = @luis;       -- Cancelar
DELETE FROM @Items;

/* ---------- 6) Ana | EN REVISIÓN | IVA 16% | Total 13,920.00
   Excede supervisor (1,000) y gerente (5,000): SOLO el director (100,000) puede aprobarla. */
INSERT @Items VALUES ('SRV-050', 'Servidor para desarrollo', 2, 6000.00);
EXEC dbo.sp_Requisicion_Crear @UsuarioId = @ana, @Descripcion = N'Servidor para desarrollo',
     @Subtotal = 12000.00, @Impuesto = 1920.00, @Total = 13920.00, @PoliticaImpuesto = 'IVA 16%', @Items = @Items, @Id = @Id OUTPUT;
EXEC dbo.sp_Requisicion_CambiarEstado @Id = @Id, @NuevoEstado = 2, @UsuarioId = @ana;        -- Enviar
GO

/* ---------- Verificación ---------- */
SELECT u.NombreUsuario, u.NombreCompleto, u.Departamento,
       CASE u.Rol WHEN 1 THEN 'Solicitante' ELSE 'Aprobador' END AS Rol,
       s.NombreUsuario AS Superior, u.LimiteAprobacion
FROM dbo.vw_Usuario u LEFT JOIN dbo.vw_Usuario s ON s.Id = u.SuperiorId ORDER BY u.Id;

SELECT Id, Solicitante, Total, PoliticaImpuesto, ResueltoPor,
       CASE Estado WHEN 1 THEN 'Creada' WHEN 2 THEN 'En Revisión' WHEN 3 THEN 'Aprobada'
                   WHEN 4 THEN 'Rechazada' WHEN 5 THEN 'Cancelada' END AS Estado
FROM dbo.vw_Requisicion ORDER BY Id;
GO
