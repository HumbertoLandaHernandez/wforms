/* =====================================================================
   Base de datos: Generica   Esquema: Sistema   (versión 2 del esquema)
   Generado a partir de INFORMATION_SCHEMA.COLUMNS proporcionado.
   - Tipos, longitudes, nulabilidad, orden de columnas y DEFAULT (getdate())
     respetan el original.
   - Columnas *Usys : usuario que creó/modificó el registro.
   - Columnas *DT / *dt y las *RV de tipo datetime2: fecha de alta/modificación.
   - NombreTipoRV y RolDT son timestamp (rowversion) automáticos.
   - Los Id numéricos de tablas catálogo/encabezado se crean como IDENTITY.
   ===================================================================== */
IF DB_ID(N'Generica') IS NULL
    CREATE DATABASE Generica COLLATE Modern_Spanish_CI_AS;
GO
USE Generica;
GO
IF SCHEMA_ID(N'Sistema') IS NULL
    EXEC(N'CREATE SCHEMA Sistema');
GO

/* ---------- Nombres ---------- */
CREATE TABLE Sistema.Nombre (
    NombreUsys  varchar(30)  NOT NULL,
    NombreDT    datetime2(7) NOT NULL CONSTRAINT DF_Nombre_DT DEFAULT (getdate()),
    NombreId    int IDENTITY(1,1) NOT NULL,
    NombreDesc  varchar(50)  NOT NULL,
    CONSTRAINT PK_Nombre PRIMARY KEY (NombreId)
);
GO
CREATE TABLE Sistema.NombreTipo (
    NombreTipoUsys  varchar(30) NOT NULL,
    NombreTipoRV    rowversion  NOT NULL,
    NombreTipoId    varchar(50) NOT NULL,   -- clave capturada por el usuario (p. ej. NOM, APP, APM)
    NombreTipoDesc  varchar(50) NOT NULL,
    CONSTRAINT PK_NombreTipo PRIMARY KEY (NombreTipoId)
);
GO
CREATE TABLE Sistema.NombreLista (
    NombreListaUsys  varchar(30)  NOT NULL,
    NombreListaDT    datetime2(7) NOT NULL CONSTRAINT DF_NombreLista_DT DEFAULT (getdate()),
    NombreListaId    int IDENTITY(1,1) NOT NULL,
    NombreListaDesc  varchar(50)  NOT NULL,   -- nombre completo que se muestra
    CONSTRAINT PK_NombreLista PRIMARY KEY (NombreListaId)
);
GO
CREATE TABLE Sistema.NombreListaElemento (
    NombreListaElementoUsys  varchar(30)  NOT NULL,
    NombreListaElementoRV    datetime2(7) NOT NULL CONSTRAINT DF_NombreListaElemento_RV DEFAULT (getdate()),
    NombreListaId            int          NOT NULL,
    NombreTipoId             varchar(50)  NOT NULL,
    NombreId                 int          NOT NULL,
    CONSTRAINT PK_NombreListaElemento PRIMARY KEY (NombreListaId, NombreTipoId, NombreId),
    CONSTRAINT FK_NombreListaElemento_NombreLista FOREIGN KEY (NombreListaId) REFERENCES Sistema.NombreLista (NombreListaId),
    CONSTRAINT FK_NombreListaElemento_NombreTipo  FOREIGN KEY (NombreTipoId)  REFERENCES Sistema.NombreTipo (NombreTipoId),
    CONSTRAINT FK_NombreListaElemento_Nombre      FOREIGN KEY (NombreId)      REFERENCES Sistema.Nombre (NombreId)
);
GO

/* ---------- Páginas y listas de páginas ---------- */
CREATE TABLE Sistema.Pagina (
    PaginaUsys   varchar(30)  NOT NULL,
    PaginaDT     datetime2(7) NOT NULL CONSTRAINT DF_Pagina_DT DEFAULT (getdate()),
    PaginaId     smallint IDENTITY(1,1) NOT NULL,
    PaginaDesc   varchar(50)  NOT NULL,   -- ruta de la página, p. ej. ~/Admin/Usuarios.aspx
    Descripcion  varchar(150) NOT NULL,   -- texto que se muestra en el menú
    CONSTRAINT PK_Pagina PRIMARY KEY (PaginaId),
    CONSTRAINT UQ_Pagina_PaginaDesc UNIQUE (PaginaDesc)
);
GO
CREATE TABLE Sistema.PaginaLista (
    PaginaListaUsys        varchar(30)  NOT NULL,
    PaginaListadt          datetime2(7) NOT NULL CONSTRAINT DF_PaginaLista_dt DEFAULT (getdate()),
    PaginaListaId          smallint IDENTITY(1,1) NOT NULL,
    PaginaListaDesc        varchar(50)  NOT NULL,
    PaginaListaComentario  varchar(500) NOT NULL,
    CONSTRAINT PK_PaginaLista PRIMARY KEY (PaginaListaId)
);
GO
CREATE TABLE Sistema.PaginaListaElemento (
    PaginaListaElementoUsys  varchar(30)  NOT NULL,
    PaginaListaElementoRV    datetime2(7) NOT NULL CONSTRAINT DF_PaginaListaElemento_RV DEFAULT (getdate()),
    PaginaListaId            smallint     NOT NULL,
    PaginaId                 smallint     NOT NULL,
    CONSTRAINT PK_PaginaListaElemento PRIMARY KEY (PaginaListaId, PaginaId),
    CONSTRAINT FK_PaginaListaElemento_PaginaLista FOREIGN KEY (PaginaListaId) REFERENCES Sistema.PaginaLista (PaginaListaId),
    CONSTRAINT FK_PaginaListaElemento_Pagina      FOREIGN KEY (PaginaId)      REFERENCES Sistema.Pagina (PaginaId)
);
GO

/* ---------- Permisos y listas de permisos ---------- */
CREATE TABLE Sistema.Permiso (
    PermisosUsys  varchar(30)  NOT NULL,
    PermisosRV    datetime2(7) NOT NULL CONSTRAINT DF_Permiso_RV DEFAULT (getdate()),
    PermisoId     smallint IDENTITY(1,1) NOT NULL,
    PermisoDesc   varchar(50)  NOT NULL,
    CONSTRAINT PK_Permiso PRIMARY KEY (PermisoId)
);
GO
CREATE TABLE Sistema.PermisoLista (
    PermisoListaUsys  varchar(30)  NOT NULL,
    PermisoListaRV    datetime2(7) NOT NULL CONSTRAINT DF_PermisoLista_RV DEFAULT (getdate()),
    PermisoListaId    int IDENTITY(1,1) NOT NULL,
    PermisoListaDesc  varchar(50)  NOT NULL,
    CONSTRAINT PK_PermisoLista PRIMARY KEY (PermisoListaId)
);
GO
CREATE TABLE Sistema.PermisoListaElemento (
    PermisoListaElementoUsys  varchar(30)  NOT NULL,
    PermisoListaElementoDT    datetime2(7) NOT NULL CONSTRAINT DF_PermisoListaElemento_DT DEFAULT (getdate()),
    PermisoListaId            int          NOT NULL,
    PermisoId                 smallint     NOT NULL,
    CONSTRAINT PK_PermisoListaElemento PRIMARY KEY (PermisoListaId, PermisoId),
    CONSTRAINT FK_PermisoListaElemento_PermisoLista FOREIGN KEY (PermisoListaId) REFERENCES Sistema.PermisoLista (PermisoListaId),
    CONSTRAINT FK_PermisoListaElemento_Permiso      FOREIGN KEY (PermisoId)      REFERENCES Sistema.Permiso (PermisoId)
);
GO

/* ---------- Roles ---------- */
CREATE TABLE Sistema.Rol (
    RolUsys          varchar(30) NOT NULL,
    RolDT            rowversion  NOT NULL,   -- timestamp en el esquema original
    RolId            tinyint IDENTITY(1,1) NOT NULL,
    RolDesc          varchar(50) NOT NULL,
    RolPaginaHomeId  tinyint     NOT NULL,   -- PaginaId (tinyint vs smallint: sin FK)
    RolPermisosId    int         NOT NULL,
    RolClassId       tinyint     NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (RolId),
    CONSTRAINT FK_Rol_PermisoLista FOREIGN KEY (RolPermisosId) REFERENCES Sistema.PermisoLista (PermisoListaId)
);
GO

/* ---------- Usuarios ---------- */
CREATE TABLE Sistema.Usuario (
    UsuarioUsys               varchar(50)  NOT NULL,
    UsuarioDT                 datetime2(7) NOT NULL,
    UsuarioId                 smallint IDENTITY(1,1) NOT NULL,
    UsuarioDesc               varchar(30)  NOT NULL,   -- nombre de inicio de sesión
    [Contraseña]              varchar(100) NOT NULL,   -- hash PBKDF2 (nunca texto plano)
    UsuarioRolId              tinyint      NOT NULL,   -- rol principal
    UsuarioPermisosId         smallint     NOT NULL,   -- permiso individual adicional
    UsuarioNombreId           int          NOT NULL,   -- NombreLista
    UsuarioPermisosPaginasId  int          NOT NULL,   -- PaginaLista (int vs smallint: sin FK)
    UsuarioPaginaHomeId       smallint     NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuario_UsuarioDesc UNIQUE (UsuarioDesc),
    CONSTRAINT FK_Usuario_Rol         FOREIGN KEY (UsuarioRolId)        REFERENCES Sistema.Rol (RolId),
    CONSTRAINT FK_Usuario_Permiso     FOREIGN KEY (UsuarioPermisosId)   REFERENCES Sistema.Permiso (PermisoId),
    CONSTRAINT FK_Usuario_NombreLista FOREIGN KEY (UsuarioNombreId)     REFERENCES Sistema.NombreLista (NombreListaId),
    CONSTRAINT FK_Usuario_PaginaHome  FOREIGN KEY (UsuarioPaginaHomeId) REFERENCES Sistema.Pagina (PaginaId)
);
GO
CREATE TABLE Sistema.UsuarioRol (
    UsuarioRolUsys  varchar(30)  NOT NULL,
    UsuarioRolDT    datetime2(7) NOT NULL,
    UsuarioId       smallint     NOT NULL,
    RolId           tinyint      NOT NULL,
    CONSTRAINT PK_UsuarioRol PRIMARY KEY (UsuarioId, RolId),
    CONSTRAINT FK_UsuarioRol_Usuario FOREIGN KEY (UsuarioId) REFERENCES Sistema.Usuario (UsuarioId),
    CONSTRAINT FK_UsuarioRol_Rol     FOREIGN KEY (RolId)     REFERENCES Sistema.Rol (RolId)
);
GO

/* ---------- Índices de apoyo ---------- */
CREATE INDEX IX_PaginaListaElemento_Pagina   ON Sistema.PaginaListaElemento (PaginaId);
CREATE INDEX IX_PermisoListaElemento_Permiso ON Sistema.PermisoListaElemento (PermisoId);
CREATE INDEX IX_UsuarioRol_Rol               ON Sistema.UsuarioRol (RolId);
GO

/* ---------------------------------------------------------------------
   OPCIONAL: dos relaciones tienen tipos distintos y se validan en la aplicación.
   Si puede alinear los tipos, descomente para tener integridad referencial:

ALTER TABLE Sistema.Rol ALTER COLUMN RolPaginaHomeId smallint NOT NULL;
ALTER TABLE Sistema.Rol ADD CONSTRAINT FK_Rol_PaginaHome FOREIGN KEY (RolPaginaHomeId) REFERENCES Sistema.Pagina (PaginaId);
ALTER TABLE Sistema.Usuario ALTER COLUMN UsuarioPermisosPaginasId smallint NOT NULL;
ALTER TABLE Sistema.Usuario ADD CONSTRAINT FK_Usuario_PaginaLista FOREIGN KEY (UsuarioPermisosPaginasId) REFERENCES Sistema.PaginaLista (PaginaListaId);
   --------------------------------------------------------------------- */
