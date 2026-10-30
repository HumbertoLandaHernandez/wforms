/* =====================================================================
   Datos iniciales
   Usuario: admin   Contraseña: Admin123!   (cámbiela al primer ingreso)
   ===================================================================== */
USE Generica;
GO
SET NOCOUNT ON;
BEGIN TRANSACTION;

SET IDENTITY_INSERT Sistema.NombrePersonal ON;
INSERT Sistema.NombrePersonal (NombrePersonalUsys, NombrePersonalId, NombrePersonalDesc)
VALUES ('sistema', 1, 'Administrador del Sistema');
SET IDENTITY_INSERT Sistema.NombrePersonal OFF;

SET IDENTITY_INSERT Sistema.NombreTipo ON;
INSERT Sistema.NombreTipo (NombreTipoUsys, NombreTipoId, NombreTipoDesc) VALUES
('sistema', 1, 'Nombre'), ('sistema', 2, 'Apellido paterno'), ('sistema', 3, 'Apellido materno');
SET IDENTITY_INSERT Sistema.NombreTipo OFF;

SET IDENTITY_INSERT Sistema.NombreBase ON;
INSERT Sistema.NombreBase (NombreBaseUsys, NombreBaseId, NombreBaseDesc) VALUES
('sistema', 1, 'Administrador'), ('sistema', 2, 'Sistema');
SET IDENTITY_INSERT Sistema.NombreBase OFF;

INSERT Sistema.NombreLista (NombreListaUsys, NombrePersonalId, NombreTipoId, NombreBaseId) VALUES
('sistema', 1, 1, 1), ('sistema', 1, 2, 2);

SET IDENTITY_INSERT Sistema.Pagina ON;
INSERT Sistema.Pagina (PaginaUsys, PaginaId, PaginaDesc, Descripcion) VALUES
('sistema', 1, '~/Default.aspx',               'Inicio'),
('sistema', 2, '~/Admin/Usuarios.aspx',        'Usuarios'),
('sistema', 3, '~/Admin/Roles.aspx',           'Roles'),
('sistema', 4, '~/Admin/Paginas.aspx',         'Páginas'),
('sistema', 5, '~/Admin/Catalogos.aspx',       'Catálogos'),
('sistema', 6, '~/Admin/PermisosPaginas.aspx', 'Permisos por página'),
('sistema', 7, '~/Admin/ListasPaginas.aspx',   'Menú (listas de páginas)'),
('sistema', 8, '~/Admin/NombresLista.aspx',    'Composición de nombres');
SET IDENTITY_INSERT Sistema.Pagina OFF;

SET IDENTITY_INSERT Sistema.Permiso ON;
INSERT Sistema.Permiso (PermisosUsys, PermisoId, PermisoDesc) VALUES
(0, 1, 'Solo lectura'), (0, 2, 'Lectura y escritura');
SET IDENTITY_INSERT Sistema.Permiso OFF;

-- Lista 1: todas las páginas.  Lista 2: solo Inicio.
INSERT Sistema.PermisoLista (PermisoListaUsys, PermisoListaId, PermisoListaPaginaId)
SELECT 'sistema', 1, PaginaId FROM Sistema.Pagina;
INSERT Sistema.PermisoLista (PermisoListaUsys, PermisoListaId, PermisoListaPaginaId)
VALUES ('sistema', 2, 1);

SET IDENTITY_INSERT Sistema.Rol ON;
INSERT Sistema.Rol (RolUsys, RolId, RolDesc, RolPaginaHomeId, RolPermisosId, RolClassId) VALUES
('sistema', 1, 'Administrador', 1, 1, 1),
('sistema', 2, 'Consulta',      1, 2, 2);
SET IDENTITY_INSERT Sistema.Rol OFF;

SET IDENTITY_INSERT Sistema.ListaPagina ON;
INSERT Sistema.ListaPagina (ListaPaginaUsys, ListaPaginaId, ListaPaginaDesc, ListaPaginaComentario) VALUES
('sistema', 1, 'Seguridad', 'Usuarios, roles, páginas y permisos'),
('sistema', 2, 'Catálogos', 'Catálogos generales');
SET IDENTITY_INSERT Sistema.ListaPagina OFF;

INSERT Sistema.ListaPaginaElemento (ListaPaginaElementoUsys, ListaPaginaId, PaginaId) VALUES
('sistema', 1, 2), ('sistema', 1, 3), ('sistema', 1, 4), ('sistema', 1, 6), ('sistema', 1, 7),
('sistema', 2, 5), ('sistema', 2, 8);

SET IDENTITY_INSERT Sistema.Usuario ON;
INSERT Sistema.Usuario (UsuarioUsys, UsuarioId, UsuarioDesc, [Contraseña], UsuarioRolId, UsuarioPermisosId,
                        UsuarioNombreId, UsuarioPermisosPaginasId, UsuarioPaginaHomeId)
VALUES ('sistema', 1, 'admin',
        'PBKDF2$100000$tY8185FbyHwbi+5giKKGng==$EdVcrxYdb29Km/ZgiESLoDiq2Z3OhPDDCIbC5bEYCQc=',
        1, 2, 1, 1, 1);
SET IDENTITY_INSERT Sistema.Usuario OFF;

COMMIT TRANSACTION;
GO
