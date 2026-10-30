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
| `Seguridad/` | `JwtServicio` (emite y valida el token), `JwtAuthModule` (IHttpModule que autentica cada petición), `HashContrasena` (PBKDF2-SHA256), `PaginaSegura` (página base que valida permisos) |
| `Datos/` | ADO.NET con consultas parametrizadas: un repositorio por tabla |
| `Admin/` | Mantenimiento de Usuarios, Roles, Páginas, Catálogos, Permisos por página, Menú y Composición de nombres |
| `Api/` | `Login.ashx` (POST → token) y `Perfil.ashx` (GET con Bearer) |

## Cómo se interpretó el esquema

- **Pagina.PaginaDesc** guarda la ruta de la página (`~/Admin/Usuarios.aspx`) y **Descripcion** el texto del menú.
- **PermisoLista** (`PermisoListaId`, `PermisoListaPaginaId`) agrupa páginas permitidas. Una página es accesible si está en la lista del **rol** (`Rol.RolPermisosId`) o en la lista adicional del **usuario** (`Usuario.UsuarioPermisosPaginasId`; 0 = ninguna). El rol `Seguridad:RolAdministradorId` ve todo.
- **ListaPagina / ListaPaginaElemento** definen los grupos del menú. Cada usuario ve solo las páginas a las que tiene acceso.
- **Permiso** define el nivel de acción: el `PermisoId` configurado en `Seguridad:PermisoSoloLecturaId` solo puede consultar.
- **Usuario.Contraseña** guarda un hash `PBKDF2$iteraciones$sal$hash` (83 caracteres), nunca la contraseña en texto plano.
- Columnas `*Usys` reciben el usuario que modifica. `Permiso.PermisosUsys` es `int` en el esquema, así que ahí se guarda el `UsuarioId`. Las columnas `*DT` usan `SYSDATETIME()` y las `*RV`/`RolDT` son `rowversion`.
- Los `Id` se crearon como `IDENTITY`, excepto `PermisoListaId`, que es un número de grupo.

### Diferencias de tipos en el esquema original
Estas relaciones no pueden tener FK porque los tipos no coinciden, así que la aplicación las valida:
`Rol.RolPaginaHomeId` (tinyint) → `Pagina.PaginaId` (smallint), `PermisoLista.PermisoListaPaginaId` (int) → `PaginaId`, y
`ListaPaginaElemento.ListaPaginaId` (smallint) → `ListaPagina.ListaPaginaId` (tinyint).
Al final de `01_Esquema.sql` hay un bloque comentado para alinearlos si lo desea.

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
