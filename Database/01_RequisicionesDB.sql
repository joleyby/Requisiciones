/* =====================================================================
   RequisicionesDB - Script completo (re-ejecutable)
   Incluye: login con roles, jerarquía de superiores y límites de aprobación.
   Requiere SQL Server 2016 o superior.
   Ejecutar con QUOTED_IDENTIFIER activo (sqlcmd -I).
   ===================================================================== */

IF DB_ID('RequisicionesDB') IS NULL
    CREATE DATABASE RequisicionesDB;
GO
USE RequisicionesDB;
GO

/* ===================== LIMPIEZA (para poder re-ejecutar) ===================== */
DROP PROCEDURE IF EXISTS dbo.sp_Requisicion_Crear;
DROP PROCEDURE IF EXISTS dbo.sp_Requisicion_CambiarEstado;
DROP PROCEDURE IF EXISTS dbo.sp_Requisicion_ActualizarDescripcion;
DROP PROCEDURE IF EXISTS dbo.sp_Usuario_Crear;
DROP PROCEDURE IF EXISTS dbo.sp_Solicitante_Crear;     -- catálogo anterior (reemplazado por Usuario)
DROP PROCEDURE IF EXISTS dbo.sp_Departamento_Crear;
DROP VIEW      IF EXISTS dbo.vw_ReporteRequisicion;
DROP VIEW      IF EXISTS dbo.vw_RequisicionItem;
DROP VIEW      IF EXISTS dbo.vw_Requisicion;
DROP VIEW      IF EXISTS dbo.vw_UsuarioJerarquia;
DROP VIEW      IF EXISTS dbo.vw_UsuarioCredencial;
DROP VIEW      IF EXISTS dbo.vw_Usuario;
DROP VIEW      IF EXISTS dbo.vw_Solicitante;           -- catálogo anterior
DROP VIEW      IF EXISTS dbo.vw_Departamento;
DROP TABLE     IF EXISTS dbo.RequisicionItem;
DROP TABLE     IF EXISTS dbo.Requisicion;
DROP TABLE     IF EXISTS dbo.Usuario;
DROP TABLE     IF EXISTS dbo.Solicitante;              -- catálogo anterior
DROP TABLE     IF EXISTS dbo.Departamento;
DROP TYPE      IF EXISTS dbo.ItemRequisicionType;
GO

/* ===================== TABLAS ===================== */
CREATE TABLE dbo.Departamento (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);
GO

/* Usuario: login + rol + superior + límite de aprobación.
   Rol: 1 = Solicitante, 2 = Aprobador.
   Un Aprobador DEBE tener límite (> 0); un Solicitante NO puede tenerlo. */
CREATE TABLE dbo.Usuario (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario    NVARCHAR(50)  NOT NULL UNIQUE,
    NombreCompleto   NVARCHAR(100) NOT NULL,
    Departamento     NVARCHAR(100) NOT NULL REFERENCES dbo.Departamento(Nombre),
    Rol              TINYINT       NOT NULL,
    SuperiorId       INT           NULL REFERENCES dbo.Usuario(Id),
    LimiteAprobacion DECIMAL(18,2) NULL,
    PasswordHash     VARBINARY(64) NOT NULL,   -- PBKDF2-SHA256 (32 bytes)
    Salt             VARBINARY(32) NOT NULL,   -- 16 bytes aleatorios por usuario
    Iteraciones      INT           NOT NULL DEFAULT 10000,
    Activo           BIT           NOT NULL DEFAULT 1,
    CONSTRAINT CK_Usuario_Rol      CHECK (Rol IN (1, 2)),
    CONSTRAINT CK_Usuario_Limite   CHECK ((Rol = 2 AND LimiteAprobacion > 0) OR (Rol = 1 AND LimiteAprobacion IS NULL)),
    CONSTRAINT CK_Usuario_Superior CHECK (SuperiorId IS NULL OR SuperiorId <> Id)
);
GO

CREATE TABLE dbo.Requisicion (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId        INT           NOT NULL REFERENCES dbo.Usuario(Id),   -- quien la creó
    Solicitante      NVARCHAR(100) NOT NULL,                              -- nombre al momento de crearla (reportes)
    Departamento     NVARCHAR(100) NOT NULL REFERENCES dbo.Departamento(Nombre),
    Descripcion      NVARCHAR(300) NOT NULL,
    Subtotal         DECIMAL(18,2) NOT NULL,
    Impuesto         DECIMAL(18,2) NOT NULL,
    Total            DECIMAL(18,2) NOT NULL,
    Estado           TINYINT       NOT NULL DEFAULT 1,   -- 1 Creada, 2 EnRevision, 3 Aprobada, 4 Rechazada, 5 Cancelada
    PoliticaImpuesto NVARCHAR(50)  NOT NULL,
    FechaCreacion    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    ResueltoPorId    INT           NULL REFERENCES dbo.Usuario(Id),       -- quien aprobó/rechazó
    FechaResolucion  DATETIME2     NULL,
    CONSTRAINT CK_Req_Estado CHECK (Estado BETWEEN 1 AND 5),
    CONSTRAINT CK_Req_Total  CHECK (Total = Subtotal + Impuesto)
);
GO

CREATE TABLE dbo.RequisicionItem (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    RequisicionId  INT           NOT NULL REFERENCES dbo.Requisicion(Id),
    Codigo         NVARCHAR(50)  NOT NULL,
    Descripcion    NVARCHAR(200) NOT NULL,
    Cantidad       DECIMAL(18,2) NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18,2) NOT NULL CHECK (PrecioUnitario > 0),
    Subtotal       AS (Cantidad * PrecioUnitario) PERSISTED,
    CONSTRAINT UQ_Item_Req_Codigo UNIQUE (RequisicionId, Codigo)
);
GO

/* ===================== TIPO TABLA ===================== */
CREATE TYPE dbo.ItemRequisicionType AS TABLE (
    Codigo         NVARCHAR(50)  NOT NULL,
    Descripcion    NVARCHAR(200) NOT NULL,
    Cantidad       DECIMAL(18,2) NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL
);
GO

/* ===================== VISTAS (lectura y reportes) ===================== */
CREATE VIEW dbo.vw_Departamento AS
SELECT Id, Nombre FROM dbo.Departamento;
GO

/* Usuario SIN datos secretos (hash/salt) */
CREATE VIEW dbo.vw_Usuario AS
SELECT Id, NombreUsuario, NombreCompleto, Departamento, Rol, SuperiorId, LimiteAprobacion, Activo
FROM dbo.Usuario;
GO

/* Solo para autenticación */
CREATE VIEW dbo.vw_UsuarioCredencial AS
SELECT Id AS UsuarioId, NombreUsuario, PasswordHash, Salt, Iteraciones, Activo
FROM dbo.Usuario;
GO

/* Cadena de superiores de cada usuario: (UsuarioId) -> (SuperiorId, Nivel)
   Nivel 1 = superior directo, 2 = el superior de este, etc. */
CREATE VIEW dbo.vw_UsuarioJerarquia AS
WITH Cadena AS (
    SELECT Id AS UsuarioId, SuperiorId, 1 AS Nivel
    FROM dbo.Usuario
    WHERE SuperiorId IS NOT NULL
    UNION ALL
    SELECT c.UsuarioId, u.SuperiorId, c.Nivel + 1
    FROM Cadena c
    JOIN dbo.Usuario u ON u.Id = c.SuperiorId
    WHERE u.SuperiorId IS NOT NULL
)
SELECT UsuarioId, SuperiorId, Nivel FROM Cadena;
GO

CREATE VIEW dbo.vw_Requisicion AS
SELECT r.Id, r.UsuarioId, r.Solicitante, r.Departamento, r.Descripcion,
       r.Subtotal, r.Impuesto, r.Total, r.Estado, r.PoliticaImpuesto, r.FechaCreacion,
       ur.NombreCompleto AS ResueltoPor, r.FechaResolucion
FROM dbo.Requisicion r
LEFT JOIN dbo.Usuario ur ON ur.Id = r.ResueltoPorId;
GO

CREATE VIEW dbo.vw_RequisicionItem AS
SELECT RequisicionId, Codigo, Descripcion, Cantidad, PrecioUnitario, Subtotal
FROM dbo.RequisicionItem;
GO

CREATE VIEW dbo.vw_ReporteRequisicion AS
SELECT r.Id AS RequisicionId,
       r.Solicitante,
       r.Departamento,
       r.Descripcion,
       CASE r.Estado WHEN 1 THEN 'Creada'
                     WHEN 2 THEN 'En Revisión'
                     WHEN 3 THEN 'Aprobada'
                     WHEN 4 THEN 'Rechazada'
                     WHEN 5 THEN 'Cancelada' END AS Estado,
       r.PoliticaImpuesto,
       i.Codigo,
       i.Descripcion AS ItemDescripcion,
       i.Cantidad,
       i.PrecioUnitario,
       i.Subtotal    AS ItemSubtotal,
       r.Subtotal,
       r.Impuesto,
       r.Total
FROM dbo.Requisicion r
JOIN dbo.RequisicionItem i ON i.RequisicionId = r.Id;
GO

/* ===================== PROCEDIMIENTOS ALMACENADOS (escritura) ===================== */
CREATE PROCEDURE dbo.sp_Departamento_Crear
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Departamento (Nombre) VALUES (LTRIM(RTRIM(@Nombre)));
END
GO

CREATE PROCEDURE dbo.sp_Usuario_Crear
    @NombreUsuario  NVARCHAR(50),
    @NombreCompleto NVARCHAR(100),
    @Departamento   NVARCHAR(100),
    @Rol            TINYINT,
    @SuperiorId     INT = NULL,
    @Limite         DECIMAL(18,2) = NULL,
    @PasswordHash   VARBINARY(64),
    @Salt           VARBINARY(32),
    @Iteraciones    INT = 10000,
    @Id             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    -- Un superior debe ser un Aprobador activo
    IF @SuperiorId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id = @SuperiorId AND Rol = 2 AND Activo = 1)
        THROW 50010, 'El superior debe ser un usuario activo con rol Aprobador.', 1;

    INSERT dbo.Usuario (NombreUsuario, NombreCompleto, Departamento, Rol, SuperiorId, LimiteAprobacion,
                        PasswordHash, Salt, Iteraciones)
    VALUES (LTRIM(RTRIM(@NombreUsuario)), @NombreCompleto, @Departamento, @Rol, @SuperiorId, @Limite,
            @PasswordHash, @Salt, @Iteraciones);
    SET @Id = SCOPE_IDENTITY();
END
GO

/* Solicitante y Departamento se toman del USUARIO (no se aceptan del cliente) */
CREATE PROCEDURE dbo.sp_Requisicion_Crear
    @UsuarioId        INT,
    @Descripcion      NVARCHAR(300),
    @Subtotal         DECIMAL(18,2),
    @Impuesto         DECIMAL(18,2),
    @Total            DECIMAL(18,2),
    @PoliticaImpuesto NVARCHAR(50),
    @Items            dbo.ItemRequisicionType READONLY,
    @Id               INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Solicitante NVARCHAR(100), @Departamento NVARCHAR(100);
    SELECT @Solicitante = NombreCompleto, @Departamento = Departamento
    FROM dbo.Usuario WHERE Id = @UsuarioId AND Activo = 1;

    IF @Solicitante IS NULL THROW 50006, 'Usuario no válido o inactivo.', 1;
    IF NOT EXISTS (SELECT 1 FROM @Items) THROW 50001, 'La requisición debe tener al menos un ítem.', 1;

    BEGIN TRAN;
        INSERT dbo.Requisicion (UsuarioId, Solicitante, Departamento, Descripcion, Subtotal, Impuesto, Total, Estado, PoliticaImpuesto)
        VALUES (@UsuarioId, @Solicitante, @Departamento, @Descripcion, @Subtotal, @Impuesto, @Total, 1, @PoliticaImpuesto);

        SET @Id = SCOPE_IDENTITY();

        INSERT dbo.RequisicionItem (RequisicionId, Codigo, Descripcion, Cantidad, PrecioUnitario)
        SELECT @Id, Codigo, Descripcion, Cantidad, PrecioUnitario FROM @Items;
    COMMIT;
END
GO

/* Valida transición de estado, permisos y límite de aprobación (defensa en profundidad:
   la capa de negocio ya valida lo mismo antes de llamar a este procedimiento). */
CREATE PROCEDURE dbo.sp_Requisicion_CambiarEstado
    @Id          INT,
    @NuevoEstado TINYINT,
    @UsuarioId   INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Actual TINYINT, @Creador INT, @Total DECIMAL(18,2);
    SELECT @Actual = Estado, @Creador = UsuarioId, @Total = Total
    FROM dbo.Requisicion WHERE Id = @Id;

    IF @Actual IS NULL THROW 50002, 'La requisición no existe.', 1;
    IF @Actual IN (3, 4, 5) THROW 50003, 'La requisición está en un estado final y no puede modificarse.', 1;
    IF NOT ( (@Actual = 1 AND @NuevoEstado IN (2, 5))
          OR (@Actual = 2 AND @NuevoEstado IN (3, 4, 5)) )
        THROW 50004, 'Transición de estado no válida.', 1;

    DECLARE @Limite DECIMAL(18,2), @Activo BIT;
    SELECT @Limite = LimiteAprobacion, @Activo = Activo FROM dbo.Usuario WHERE Id = @UsuarioId;
    IF @Activo IS NULL OR @Activo = 0 THROW 50006, 'Usuario no válido o inactivo.', 1;

    IF @NuevoEstado IN (2, 5)            -- Enviar / Cancelar: solo el creador
    BEGIN
        IF @UsuarioId <> @Creador
            THROW 50007, 'Solo quien creó la requisición puede enviarla o cancelarla.', 1;
    END
    ELSE                                 -- Aprobar / Rechazar: solo un superior del solicitante
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.vw_UsuarioJerarquia WHERE UsuarioId = @Creador AND SuperiorId = @UsuarioId)
            THROW 50008, 'Solo un superior del solicitante puede aprobar o rechazar la requisición.', 1;
        IF @NuevoEstado = 3 AND (@Limite IS NULL OR @Total > @Limite)
            THROW 50009, 'El total de la requisición excede su límite de aprobación.', 1;
    END

    UPDATE dbo.Requisicion
    SET Estado          = @NuevoEstado,
        ResueltoPorId   = CASE WHEN @NuevoEstado IN (3, 4) THEN @UsuarioId ELSE ResueltoPorId END,
        FechaResolucion = CASE WHEN @NuevoEstado IN (3, 4) THEN SYSDATETIME() ELSE FechaResolucion END
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE dbo.sp_Requisicion_ActualizarDescripcion
    @Id          INT,
    @Descripcion NVARCHAR(300),
    @UsuarioId   INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Requisicion WHERE Id = @Id AND Estado = 1)
        THROW 50005, 'Solo se puede modificar una requisición en estado Creada.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Requisicion WHERE Id = @Id AND UsuarioId = @UsuarioId)
        THROW 50011, 'Solo quien creó la requisición puede modificarla.', 1;
    UPDATE dbo.Requisicion SET Descripcion = @Descripcion WHERE Id = @Id;
END
GO
