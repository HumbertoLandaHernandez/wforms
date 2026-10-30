# GenericaWeb — ASP.NET Web Forms + SQL Server + JWT

Aplicación Web Forms (.NET Framework 4.8) sobre la base **Generica**, esquema **Sistema**.
La autenticación usa **JWT (HS256)**: en una cookie HttpOnly para las páginas y en `Authorization: Bearer` para la API.

## Puesta en marcha

1. **Base de datos** (SSMS o sqlcmd), en este orden:
   - `database/01_Esquema.sql` crea la base, el esquema y las tablas.
   - `database/02_Datos.sql` carga los datos iniciales (páginas, roles, menú y el usuario admin).
2. **Cadena de conexión**: ajuste `connectionStrings/Generica` en `GenericaWeb/Web.config`.
3. **Clave JWT**: cambie `Jwt:Clave` (mínimo 32 caracteres) o defina la variable de entorno `GENERICA_JWT_CLAVE`.
4. Abra `GenericaWebForms.sln` en Visual Studio 2019/2022. Los paquetes NuGet se restauran solos. Ejecute con F5 (IIS Express, https://localhost:44330/).
5. Inicie sesión con **admin / Admin123!** y cambie la contraseña en *Seguridad → Usuarios*.

## Estructura

| Carpeta | Contenido |
|---|---|
| `Seguridad/` | `JwtServicio` (emite y valida el token), `JwtAuthModule` (IHttpModule que autentica cada petición), `HashContrasena` (PBKDF2-SHA256), `PaginaSegura` (página base que valida acceso y permisos) |
| `Datos/` | ADO.NET con consultas parametrizadas: un repositorio por tabla o por par encabezado/elementos |
| `Admin/` | Usuarios, Roles, Páginas, Catálogos (Nombre, NombreTipo, Permiso), Listas de permisos, Listas de páginas y Nombres completos |
| `Api/` | `Login.ashx` (POST → token) y `Perfil.ashx` (GET con Bearer) |

## Cómo se interpretó el esquema

| Tabla(s) | Uso en la aplicación |
|---|---|
| `Usuario` | Inicio de sesión (`UsuarioDesc` + hash en `Contraseña`). `UsuarioRolId` = rol principal, `UsuarioPermisosId` = permiso individual, `UsuarioNombreId` → `NombreLista`, `UsuarioPermisosPaginasId` → `PaginaLista`, `UsuarioPaginaHomeId` → `Pagina` |
| `UsuarioRol` | Todos los roles del usuario. El rol principal siempre se guarda también aquí |
| `Rol` | `RolPermisosId` → `PermisoLista`. `RolPaginaHomeId` y `RolClassId` se editan en Roles |
| `Permiso`, `PermisoLista`, `PermisoListaElemento` | Permisos de acción. **Permisos efectivos** = permisos de las listas de todos sus roles + su permiso individual. Para crear, modificar o eliminar se requiere `Seguridad:PermisoEscrituraId` (2 = Escritura) |
| `Pagina`, `PaginaLista`, `PaginaListaElemento` | `PaginaDesc` = ruta (`~/Admin/Usuarios.aspx`), `Descripcion` = texto del menú. Un usuario entra a una página si está en **su** lista de páginas. Las listas también forman los grupos del menú |
| `Nombre`, `NombreTipo`, `NombreLista`, `NombreListaElemento` | Nombre completo del usuario: `NombreListaDesc` es el texto que se muestra y los elementos son tipo + nombre. `NombreTipoId` es una clave de texto capturada (NOM, APP, APM…) |

- El rol `Seguridad:RolAdministradorId` (1), como principal o en `UsuarioRol`, tiene acceso y permisos totales.
- Permisos y acceso a páginas se leen de la BD en cada petición, así que los cambios aplican de inmediato. El JWT también lleva roles y permisos (claims `role` y `permiso`) para la API.
- `*Usys` recibe el usuario que modifica. Las columnas `datetime2` (`*DT`, `PaginaListadt` y las `*RV` de tipo datetime2) usan `DEFAULT (getdate())` y se actualizan al modificar. `NombreTipoRV` y `RolDT` son `rowversion`.
- Los Id numéricos se crearon como `IDENTITY`. Si su base ya existe con otra definición, revise `01_Esquema.sql` antes de usar `02_Datos.sql`.

### Diferencias de tipos en el esquema
Estas relaciones no pueden tener FK porque los tipos no coinciden, así que la aplicación las valida:
`Rol.RolPaginaHomeId` (tinyint) → `Pagina.PaginaId` (smallint) y `Usuario.UsuarioPermisosPaginasId` (int) → `PaginaLista.PaginaListaId` (smallint).
Al final de `01_Esquema.sql` hay un bloque comentado para alinearlos.

## API

```bash
curl -k -X POST https://localhost:44330/Api/Login.ashx -H "Content-Type: application/json" -d "{\"usuario\":\"admin\",\"contrasena\":\"Admin123!\"}"
```

```bash
curl -k https://localhost:44330/Api/Perfil.ashx -H "Authorization: Bearer <token>"
```

## Seguridad incluida
- Firma HS256 validada: emisor, audiencia y expiración; se rechazan `alg: none` y otros algoritmos.
- Cookie `HttpOnly`, `SameSite=Lax` y `Secure` bajo HTTPS. Se renueva automáticamente pasada la mitad de su vigencia, con datos frescos de la BD.
- `ViewStateUserKey` ligado al usuario (anti-CSRF) y redirección `ReturnUrl` limitada a rutas locales.
- El hash se calcula aunque el usuario no exista, para que el tiempo de respuesta no revele qué usuarios existen.
- `Web.Release.config` quita `debug`, activa `customErrors` y exige SSL en las cookies.

Pendientes recomendados para producción: bloqueo tras intentos fallidos, revocación de tokens (lista negra por `jti`) y concurrencia optimista usando las columnas `rowversion`.
