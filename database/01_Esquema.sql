/* =====================================================================
   Base de datos: Generica   Esquema: Sistema
   Generado a partir de INFORMATION_SCHEMA.COLUMNS proporcionado.
   - Los tipos, longitudes, nulabilidad y orden de columnas respetan el original.
   - Columnas *Usys : usuario que creó/modificó el registro.
   - Columnas *DT   : fecha de alta/modificación (datetime2(7)).
   - Columnas *RV / RolDT : timestamp (rowversion) automático.
   ===================================================================== */
IF DB_ID(N'Generica') IS NULL
    CREATE DATABASE Generica COLLATE Modern_Spanish_CI_AS;
GO
USE Generica;
GO
IF SCHEMA_ID(N'Sistema') IS NULL
    EXEC(N'CREATE SCHEMA Sistema');
GO

/* ---------- Catálogos ---------- */
CREATE TABLE Sistema.NombrePersonal (
    NombrePersonalUsys  varchar(30)  NOT NULL,
    NombrePersonalRV    rowversion   NOT NULL,
    NombrePersonalId    int IDENTITY(1,1) NOT NULL,
    NombrePersonalDesc  varchar(150) NOT NULL,
    CONSTRAINT PK_NombrePersonal PRIMARY KEY (NombrePersonalId)
);
GO
CREATE TABLE Sistema.NombreTipo (
    NombreTipoUsys  varchar(30) NOT NULL,
    NombreTipoRV    rowversion  NOT NULL,
    NombreTipoId    tinyint IDENTITY(1,1) NOT NULL,
    NombreTipoDesc  varchar(50) NOT NULL,
    CONSTRAINT PK_NombreTipo PRIMARY KEY (NombreTipoId)
);
GO
CREATE TABLE Sistema.NombreBase (
    NombreBaseUsys  varchar(30)  NOT NULL,
    NombreBaseDT    datetime2(7) NOT NULL CONSTRAINT DF_NombreBase_DT DEFAULT SYSDATETIME(),
    NombreBaseId    int IDENTITY(1,1) NOT NULL,
    NombreBaseDesc  varchar(50)  NOT NULL,
    CONSTRAINT PK_NombreBase PRIMARY KEY (NombreBaseId)
);
GO
CREATE TABLE Sistema.Pagina (
    PaginaUsys   varchar(30)  NOT NULL,
    PaginaDT     datetime2(7) NOT NULL CONSTRAINT DF_Pagina_DT DEFAULT SYSDATETIME(),
    PaginaId     smallint IDENTITY(1,1) NOT NULL,
    PaginaDesc   varchar(50)  NOT NULL,   -- ruta de la página, p. ej. ~/Admin/Usuarios.aspx
    Descripcion  varchar(150) NOT NULL,   -- texto que se muestra en el menú
    CONSTRAINT PK_Pagina PRIMARY KEY (PaginaId),
    CONSTRAINT UQ_Pagina_PaginaDesc UNIQUE (PaginaDesc)
);
GO
CREATE TABLE Sistema.Permiso (
    PermisosUsys  int         NOT NULL,   -- int en el esquema original: se guarda el UsuarioId
    PermisosRV    rowversion  NOT NULL,
    PermisoId     smallint IDENTITY(1,1) NOT NULL,
    PermisoDesc   varchar(50) NOT NULL,
    CONSTRAINT PK_Permiso PRIMARY KEY (PermisoId)
);
GO

/* ---------- Permisos por página: una "lista" = conjunto de páginas ---------- */
CREATE TABLE Sistema.PermisoLista (
    PermisoListaUsys      varchar(30) NOT NULL,
    PermisoListaRV        rowversion  NOT NULL,
    PermisoListaId        int         NOT NULL,
    PermisoListaPaginaId  int         NOT NULL,   -- PaginaId (int vs smallint: sin FK)
    CONSTRAINT PK_PermisoLista PRIMARY KEY (PermisoListaId, PermisoListaPaginaId)
);
GO

CREATE TABLE Sistema.Rol (
    RolUsys          varchar(30) NOT NULL,
    RolDT            rowversion  NOT NULL,   -- timestamp en el esquema original
    RolId            tinyint IDENTITY(1,1) NOT NULL,
    RolDesc          varchar(50) NOT NULL,
    RolPaginaHomeId  tinyint     NOT NULL,   -- PaginaId (tinyint vs smallint: sin FK)
    RolPermisosId    int         NOT NULL,   -- PermisoLista.PermisoListaId
    RolClassId       tinyint     NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (RolId)
);
GO

/* ---------- Menú: listas (grupos) de páginas ---------- */
CREATE TABLE Sistema.ListaPagina (
    ListaPaginaUsys        varchar(50)  NOT NULL,
    ListaPaginaRV          rowversion   NOT NULL,
    ListaPaginaId          tinyint IDENTITY(1,1) NOT NULL,
    ListaPaginaDesc        varchar(50)  NOT NULL,
    ListaPaginaComentario  varchar(500) NOT NULL,
    CONSTRAINT PK_ListaPagina PRIMARY KEY (ListaPaginaId)
);
GO
CREATE TABLE Sistema.ListaPaginaElemento (
    ListaPaginaElementoUsys  varchar(309) NOT NULL,
    ListaPaginaElementoRV    rowversion   NOT NULL,
    ListaPaginaId            smallint     NOT NULL,   -- ListaPagina.ListaPaginaId (smallint vs tinyint: sin FK)
    PaginaId                 smallint     NOT NULL,
    CONSTRAINT PK_ListaPaginaElemento PRIMARY KEY (ListaPaginaId, PaginaId),
    CONSTRAINT FK_ListaPaginaElemento_Pagina FOREIGN KEY (PaginaId) REFERENCES Sistema.Pagina (PaginaId)
);
GO

/* ---------- Composición de nombres ---------- */
CREATE TABLE Sistema.NombreLista (
    NombreListaUsys   varchar(30) NOT NULL,
    NombreListaRV     rowversion  NOT NULL,
    NombrePersonalId  int         NOT NULL,
    NombreTipoId      tinyint     NOT NULL,
    NombreBaseId      int         NOT NULL,
    CONSTRAINT PK_NombreLista PRIMARY KEY (NombrePersonalId, NombreTipoId, NombreBaseId),
    CONSTRAINT FK_NombreLista_NombrePersonal FOREIGN KEY (NombrePersonalId) REFERENCES Sistema.NombrePersonal (NombrePersonalId),
    CONSTRAINT FK_NombreLista_NombreTipo     FOREIGN KEY (NombreTipoId)     REFERENCES Sistema.NombreTipo (NombreTipoId),
    CONSTRAINT FK_NombreLista_NombreBase     FOREIGN KEY (NombreBaseId)     REFERENCES Sistema.NombreBase (NombreBaseId)
);
GO

/* ---------- Usuarios ---------- */
CREATE TABLE Sistema.Usuario (
    UsuarioUsys               varchar(50)  NOT NULL,
    UsuarioDT                 datetime2(7) NOT NULL CONSTRAINT DF_Usuario_DT DEFAULT SYSDATETIME(),
    UsuarioId                 smallint IDENTITY(1,1) NOT NULL,
    UsuarioDesc               varchar(30)  NOT NULL,   -- nombre de inicio de sesión
    [Contraseña]              varchar(100) NOT NULL,   -- hash PBKDF2 (nunca texto plano)
    UsuarioRolId              tinyint      NOT NULL,
    UsuarioPermisosId         smallint     NOT NULL,
    UsuarioNombreId           int          NOT NULL,
    UsuarioPermisosPaginasId  int          NOT NULL,   -- PermisoLista.PermisoListaId (0 = ninguna)
    UsuarioPaginaHomeId       smallint     NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuario_UsuarioDesc UNIQUE (UsuarioDesc),
    CONSTRAINT FK_Usuario_Rol            FOREIGN KEY (UsuarioRolId)        REFERENCES Sistema.Rol (RolId),
    CONSTRAINT FK_Usuario_Permiso        FOREIGN KEY (UsuarioPermisosId)   REFERENCES Sistema.Permiso (PermisoId),
    CONSTRAINT FK_Usuario_NombrePersonal FOREIGN KEY (UsuarioNombreId)     REFERENCES Sistema.NombrePersonal (NombrePersonalId),
    CONSTRAINT FK_Usuario_PaginaHome     FOREIGN KEY (UsuarioPaginaHomeId) REFERENCES Sistema.Pagina (PaginaId)
);
GO

/* ---------- Índices de apoyo ---------- */
CREATE INDEX IX_PermisoLista_Pagina ON Sistema.PermisoLista (PermisoListaPaginaId);
CREATE INDEX IX_ListaPaginaElemento_Pagina ON Sistema.ListaPaginaElemento (PaginaId);
GO

/* ---------------------------------------------------------------------
   OPCIONAL: el esquema original tiene tipos distintos entre columnas
   relacionadas, por lo que esas relaciones se validan en la aplicación.
   Si puede alinear los tipos, descomente para tener integridad referencial:

ALTER TABLE Sistema.Rol ALTER COLUMN RolPaginaHomeId smallint NOT NULL;
ALTER TABLE Sistema.Rol ADD CONSTRAINT FK_Rol_PaginaHome FOREIGN KEY (RolPaginaHomeId) REFERENCES Sistema.Pagina (PaginaId);
ALTER TABLE Sistema.PermisoLista DROP CONSTRAINT PK_PermisoLista;
ALTER TABLE Sistema.PermisoLista ALTER COLUMN PermisoListaPaginaId smallint NOT NULL;
ALTER TABLE Sistema.PermisoLista ADD CONSTRAINT PK_PermisoLista PRIMARY KEY (PermisoListaId, PermisoListaPaginaId);
ALTER TABLE Sistema.PermisoLista ADD CONSTRAINT FK_PermisoLista_Pagina FOREIGN KEY (PermisoListaPaginaId) REFERENCES Sistema.Pagina (PaginaId);
   --------------------------------------------------------------------- */
