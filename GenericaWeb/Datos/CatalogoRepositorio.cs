using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>Definición de un catálogo simple (Id + Descripción).</summary>
    public class CatalogoDefinicion
    {
        public string Clave { get; set; }
        public string Titulo { get; set; }
        public string Tabla { get; set; }
        public string ColumnaId { get; set; }
        public string ColumnaDesc { get; set; }
        public int LongitudDesc { get; set; }
        public string ColumnaUsys { get; set; }
        /// <summary>Permiso.PermisosUsys es int: se guarda el UsuarioId en lugar del nombre.</summary>
        public bool UsysEsEntero { get; set; }
        /// <summary>Columna datetime2 a actualizar en cada modificación (null si la tabla usa rowversion).</summary>
        public string ColumnaFecha { get; set; }
    }

    /// <summary>
    /// CRUD genérico para NombrePersonal, NombreBase, NombreTipo y Permiso.
    /// Los nombres de tabla/columna provienen solo de esta lista blanca, nunca del usuario.
    /// </summary>
    public static class CatalogoRepositorio
    {
        public static readonly IReadOnlyList<CatalogoDefinicion> Catalogos = new[]
        {
            new CatalogoDefinicion { Clave = "NombrePersonal", Titulo = "Nombres personales", Tabla = "Sistema.NombrePersonal",
                ColumnaId = "NombrePersonalId", ColumnaDesc = "NombrePersonalDesc", LongitudDesc = 150, ColumnaUsys = "NombrePersonalUsys" },
            new CatalogoDefinicion { Clave = "NombreBase", Titulo = "Nombres base", Tabla = "Sistema.NombreBase",
                ColumnaId = "NombreBaseId", ColumnaDesc = "NombreBaseDesc", LongitudDesc = 50, ColumnaUsys = "NombreBaseUsys", ColumnaFecha = "NombreBaseDT" },
            new CatalogoDefinicion { Clave = "NombreTipo", Titulo = "Tipos de nombre", Tabla = "Sistema.NombreTipo",
                ColumnaId = "NombreTipoId", ColumnaDesc = "NombreTipoDesc", LongitudDesc = 50, ColumnaUsys = "NombreTipoUsys" },
            new CatalogoDefinicion { Clave = "Permiso", Titulo = "Permisos", Tabla = "Sistema.Permiso",
                ColumnaId = "PermisoId", ColumnaDesc = "PermisoDesc", LongitudDesc = 50, ColumnaUsys = "PermisosUsys", UsysEsEntero = true }
        };

        public static CatalogoDefinicion Buscar(string clave)
        {
            return Catalogos.FirstOrDefault(c => c.Clave.Equals(clave ?? string.Empty, System.StringComparison.OrdinalIgnoreCase))
                   ?? Catalogos[0];
        }

        public static DataTable Listar(CatalogoDefinicion def)
        {
            return Db.Consultar($@"SELECT {def.ColumnaId} AS Id, {def.ColumnaDesc} AS Descripcion,
                                   CAST({def.ColumnaUsys} AS varchar(30)) AS Usys
                                   FROM {def.Tabla} ORDER BY {def.ColumnaDesc}");
        }

        public static string ObtenerDescripcion(CatalogoDefinicion def, int id)
        {
            return Db.Escalar($"SELECT {def.ColumnaDesc} FROM {def.Tabla} WHERE {def.ColumnaId} = @Id", Db.P("@Id", id)) as string;
        }

        public static void Guardar(CatalogoDefinicion def, int id, string descripcion, short usuarioId, string usuarioDesc)
        {
            var usys = def.UsysEsEntero ? Db.P("@Usys", (int)usuarioId) : Db.Texto("@Usys", usuarioDesc, 30);
            var parametros = new[] { Db.P("@Id", id), usys, Db.Texto("@Desc", descripcion, def.LongitudDesc) };

            if (id == 0)
            {
                Db.Ejecutar($"INSERT INTO {def.Tabla} ({def.ColumnaUsys}, {def.ColumnaDesc}) VALUES (@Usys, @Desc)", parametros);
            }
            else
            {
                var fecha = def.ColumnaFecha == null ? string.Empty : $", {def.ColumnaFecha} = SYSDATETIME()";
                Db.Ejecutar($"UPDATE {def.Tabla} SET {def.ColumnaUsys} = @Usys, {def.ColumnaDesc} = @Desc{fecha} WHERE {def.ColumnaId} = @Id", parametros);
            }
        }

        public static void Eliminar(CatalogoDefinicion def, int id)
        {
            Db.Ejecutar($"DELETE FROM {def.Tabla} WHERE {def.ColumnaId} = @Id", Db.P("@Id", id));
        }
    }
}
