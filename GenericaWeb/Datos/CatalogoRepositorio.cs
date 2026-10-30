using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        /// <summary>true: Id varchar capturado por el usuario (NombreTipo). false: Id IDENTITY.</summary>
        public bool IdEsTexto { get; set; }
        public int LongitudId { get; set; }
        public string ColumnaDesc { get; set; }
        public int LongitudDesc { get; set; }
        public string ColumnaUsys { get; set; }
        /// <summary>Columna datetime2 a actualizar en cada modificación (null si la tabla usa timestamp).</summary>
        public string ColumnaFecha { get; set; }
    }

    /// <summary>
    /// CRUD genérico para Nombre, NombreTipo y Permiso.
    /// Los nombres de tabla/columna provienen solo de esta lista blanca, nunca del usuario.
    /// </summary>
    public static class CatalogoRepositorio
    {
        public static readonly IReadOnlyList<CatalogoDefinicion> Catalogos = new[]
        {
            new CatalogoDefinicion { Clave = "Nombre", Titulo = "Nombres", Tabla = "Sistema.Nombre",
                ColumnaId = "NombreId", ColumnaDesc = "NombreDesc", LongitudDesc = 50, ColumnaUsys = "NombreUsys", ColumnaFecha = "NombreDT" },
            new CatalogoDefinicion { Clave = "NombreTipo", Titulo = "Tipos de nombre", Tabla = "Sistema.NombreTipo",
                ColumnaId = "NombreTipoId", IdEsTexto = true, LongitudId = 50, ColumnaDesc = "NombreTipoDesc", LongitudDesc = 50,
                ColumnaUsys = "NombreTipoUsys" },
            new CatalogoDefinicion { Clave = "Permiso", Titulo = "Permisos", Tabla = "Sistema.Permiso",
                ColumnaId = "PermisoId", ColumnaDesc = "PermisoDesc", LongitudDesc = 50, ColumnaUsys = "PermisosUsys", ColumnaFecha = "PermisosRV" }
        };

        public static CatalogoDefinicion Buscar(string clave)
        {
            return Catalogos.FirstOrDefault(c => c.Clave.Equals(clave ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                   ?? Catalogos[0];
        }

        public static DataTable Listar(CatalogoDefinicion def)
        {
            return Db.Consultar($@"SELECT CAST({def.ColumnaId} AS varchar(50)) AS Id, {def.ColumnaDesc} AS Descripcion,
                                   {def.ColumnaUsys} AS Usys
                                   FROM {def.Tabla} ORDER BY {def.ColumnaDesc}");
        }

        public static string ObtenerDescripcion(CatalogoDefinicion def, string id)
        {
            return Db.Escalar($"SELECT {def.ColumnaDesc} FROM {def.Tabla} WHERE {def.ColumnaId} = @Id", ParametroId(def, id)) as string;
        }

        /// <summary>
        /// idExistente vacío = alta. En catálogos con Id de texto, idNuevo es la clave capturada para el alta
        /// (la clave no se modifica al editar porque la referencian otras tablas).
        /// </summary>
        public static void Guardar(CatalogoDefinicion def, string idExistente, string idNuevo, string descripcion, string usys)
        {
            var esAlta = string.IsNullOrEmpty(idExistente);
            var parametros = new[]
            {
                ParametroId(def, esAlta ? idNuevo : idExistente),
                Db.Texto("@Usys", usys, 30),
                Db.Texto("@Desc", descripcion, def.LongitudDesc)
            };

            if (esAlta && def.IdEsTexto)
            {
                Db.Ejecutar($"INSERT INTO {def.Tabla} ({def.ColumnaUsys}, {def.ColumnaId}, {def.ColumnaDesc}) VALUES (@Usys, @Id, @Desc)", parametros);
            }
            else if (esAlta)
            {
                Db.Ejecutar($"INSERT INTO {def.Tabla} ({def.ColumnaUsys}, {def.ColumnaDesc}) VALUES (@Usys, @Desc)", parametros);
            }
            else
            {
                var fecha = def.ColumnaFecha == null ? string.Empty : $", {def.ColumnaFecha} = SYSDATETIME()";
                Db.Ejecutar($"UPDATE {def.Tabla} SET {def.ColumnaUsys} = @Usys, {def.ColumnaDesc} = @Desc{fecha} WHERE {def.ColumnaId} = @Id", parametros);
            }
        }

        public static void Eliminar(CatalogoDefinicion def, string id)
        {
            Db.Ejecutar($"DELETE FROM {def.Tabla} WHERE {def.ColumnaId} = @Id", ParametroId(def, id));
        }

        private static SqlParameter ParametroId(CatalogoDefinicion def, string id)
        {
            return def.IdEsTexto ? Db.Texto("@Id", id, def.LongitudId) : Db.P("@Id", int.Parse(id));
        }
    }
}
