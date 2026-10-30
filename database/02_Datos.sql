/* =====================================================================
   Datos iniciales
   Usuario: admin   Contraseña: Admin123!   (cámbiela al primer ingreso)
   ===================================================================== */
USE Generica;
GO
SET NOCOUNT ON;
BEGIN TRANSACTION;

/* Nombres */
SET IDENTITY_INSERT Sistema.Nombre ON;
INSERT Sistema.Nombre (NombreUsys, NombreId, NombreDesc) VALUES
('sistema', 1, 'Administrador'), ('sistema', 2, 'del Sistema');
SET IDENTITY_INSERT Sistema.Nombre OFF;

INSERT Sistema.NombreTipo (NombreTipoUsys, NombreTipoId, NombreTipoDesc) VALUES
('sistema', 'NOM', 'Nombre'), ('sistema', 'APP', 'Apellido paterno'), ('sistema', 'APM', 'Apellido materno');

SET IDENTITY_INSERT Sistema.NombreLista ON;
INSERT Sistema.NombreLista (NombreListaUsys, NombreListaId, NombreListaDesc)
VALUES ('sistema', 1, 'Administrador del Sistema');
SET IDENTITY_INSERT Sistema.NombreLista OFF;

INSERT Sistema.NombreListaElemento (NombreListaElementoUsys, NombreListaId, NombreTipoId, NombreId) VALUES
('sistema', 1, 'NOM', 1), ('sistema', 1, 'APP', 2);

/* Páginas */
SET IDENTITY_INSERT Sistema.Pagina ON;
INSERT Sistema.Pagina (PaginaUsys, PaginaId, PaginaDesc, Descripcion) VALUES
('sistema', 1, '~/Default.aspx',              'Inicio'),
('sistema', 2, '~/Admin/Usuarios.aspx',       'Usuarios'),
('sistema', 3, '~/Admin/Roles.aspx',          'Roles'),
('sistema', 4, '~/Admin/Paginas.aspx',        'Páginas'),
('sistema', 5, '~/Admin/Catalogos.aspx',      'Catálogos'),
('sistema', 6, '~/Admin/ListasPermisos.aspx', 'Listas de permisos'),
('sistema', 7, '~/Admin/ListasPaginas.aspx',  'Listas de páginas (menú)'),
('sistema', 8, '~/Admin/NombresLista.aspx',   'Nombres completos');
SET IDENTITY_INSERT Sistema.Pagina OFF;

SET IDENTITY_INSERT Sistema.PaginaLista ON;
INSERT Sistema.PaginaLista (PaginaListaUsys, PaginaListaId, PaginaListaDesc, PaginaListaComentario) VALUES
('sistema', 1, 'Seguridad', 'Usuarios, roles, páginas y permisos'),
('sistema', 2, 'Catálogos', 'Catálogos generales');
SET IDENTITY_INSERT Sistema.PaginaLista OFF;

INSERT Sistema.PaginaListaElemento (PaginaListaElementoUsys, PaginaListaId, PaginaId) VALUES
('sistema', 1, 2), ('sistema', 1, 3), ('sistema', 1, 4), ('sistema', 1, 6), ('sistema', 1, 7),
('sistema', 2, 5), ('sistema', 2, 8);

/* Permisos */
SET IDENTITY_INSERT Sistema.Permiso ON;
INSERT Sistema.Permiso (PermisosUsys, PermisoId, PermisoDesc) VALUES
('sistema', 1, 'Lectura'), ('sistema', 2, 'Escritura');
SET IDENTITY_INSERT Sistema.Permiso OFF;

SET IDENTITY_INSERT Sistema.PermisoLista ON;
INSERT Sistema.PermisoLista (PermisoListaUsys, PermisoListaId, PermisoListaDesc) VALUES
('sistema', 1, 'Total'), ('sistema', 2, 'Consulta');
SET IDENTITY_INSERT Sistema.PermisoLista OFF;

INSERT Sistema.PermisoListaElemento (PermisoListaElementoUsys, PermisoListaId, PermisoId) VALUES
('sistema', 1, 1), ('sistema', 1, 2), ('sistema', 2, 1);

/* Roles */
SET IDENTITY_INSERT Sistema.Rol ON;
INSERT Sistema.Rol (RolUsys, RolId, RolDesc, RolPaginaHomeId, RolPermisosId, RolClassId) VALUES
('sistema', 1, 'Administrador', 1, 1, 1),
('sistema', 2, 'Consulta',      1, 2, 2);
SET IDENTITY_INSERT Sistema.Rol OFF;

/* Usuario administrador */
SET IDENTITY_INSERT Sistema.Usuario ON;
INSERT Sistema.Usuario (UsuarioUsys, UsuarioDT, UsuarioId, UsuarioDesc, [Contraseña], UsuarioRolId, UsuarioPermisosId,
                        UsuarioNombreId, UsuarioPermisosPaginasId, UsuarioPaginaHomeId)
VALUES ('sistema', SYSDATETIME(), 1, 'admin',
        'PBKDF2$100000$tY8185FbyHwbi+5giKKGng==$EdVcrxYdb29Km/ZgiESLoDiq2Z3OhPDDCIbC5bEYCQc=',
        1, 2, 1, 1, 1);
SET IDENTITY_INSERT Sistema.Usuario OFF;

INSERT Sistema.UsuarioRol (UsuarioRolUsys, UsuarioRolDT, UsuarioId, RolId)
VALUES ('sistema', SYSDATETIME(), 1, 1);

COMMIT TRANSACTION;
GO
