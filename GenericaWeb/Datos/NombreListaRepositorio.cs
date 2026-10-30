using System;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>
    /// Sistema.NombreLista (nombre completo) y Sistema.NombreListaElemento (sus partes: tipo + nombre).
    /// Cada usuario tiene un nombre completo (Usuario.UsuarioNombreId).
    /// </summary>
    public static class NombreListaRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT nl.NombreListaId, nl.NombreListaDesc, nl.NombreListaUsys, nl.NombreListaDT,
       (SELECT COUNT(*) FROM Sistema.NombreListaElemento e WHERE e.NombreListaId = nl.NombreListaId) AS Elementos
FROM Sistema.NombreLista nl
ORDER BY nl.NombreListaDesc");
        }

        public static string ObtenerDescripcion(int listaId)
        {
            return Db.Escalar("SELECT NombreListaDesc FROM Sistema.NombreLista WHERE NombreListaId = @Id", Db.P("@Id", listaId)) as string;
        }

        /// <summary>Inserta o actualiza el encabezado. Devuelve el Id.</summary>
        public static int Guardar(int listaId, string desc, string usys)
        {
            if (listaId == 0)
            {
                return Convert.ToInt32(Db.Escalar(@"
INSERT INTO Sistema.NombreLista (NombreListaUsys, NombreListaDesc) VALUES (@Usys, @Desc);
SELECT CAST(SCOPE_IDENTITY() AS int);",
                    Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50)));
            }

            Db.Ejecutar(@"UPDATE Sistema.NombreLista SET NombreListaUsys = @Usys, NombreListaDT = SYSDATETIME(),
                          NombreListaDesc = @Desc WHERE NombreListaId = @Id",
                Db.P("@Id", listaId), Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50));
            return listaId;
        }

        /// <summary>Elimina el nombre completo y sus elementos. Falla (FK) si un usuario lo usa.</summary>
        public static void Eliminar(int listaId)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.NombreListaElemento WHERE NombreListaId = @Id", Db.P("@Id", listaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.NombreLista WHERE NombreListaId = @Id", Db.P("@Id", listaId));
            });
        }

        /// <summary>Elementos en el orden en que se agregaron.</summary>
        public static DataTable Elementos(int listaId)
        {
            return Db.Consultar(@"
SELECT e.NombreListaId, e.NombreTipoId, t.NombreTipoDesc, e.NombreId, n.NombreDesc, e.NombreListaElementoUsys
FROM Sistema.NombreListaElemento e
JOIN Sistema.NombreTipo t ON t.NombreTipoId = e.NombreTipoId
JOIN Sistema.Nombre n     ON n.NombreId = e.NombreId
WHERE e.NombreListaId = @Id
ORDER BY e.NombreListaElementoRV, e.NombreTipoId", Db.P("@Id", listaId));
        }

        public static void AgregarElemento(int listaId, string nombreTipoId, int nombreId, string usys)
        {
            Db.Ejecutar(@"INSERT INTO Sistema.NombreListaElemento (NombreListaElementoUsys, NombreListaId, NombreTipoId, NombreId)
                          VALUES (@Usys, @Lista, @Tipo, @Nombre)",
                Db.Texto("@Usys", usys, 30), Db.P("@Lista", listaId), Db.Texto("@Tipo", nombreTipoId, 50), Db.P("@Nombre", nombreId));
        }

        public static void EliminarElemento(int listaId, string nombreTipoId, int nombreId)
        {
            Db.Ejecutar("DELETE FROM Sistema.NombreListaElemento WHERE NombreListaId = @Lista AND NombreTipoId = @Tipo AND NombreId = @Nombre",
                Db.P("@Lista", listaId), Db.Texto("@Tipo", nombreTipoId, 50), Db.P("@Nombre", nombreId));
        }

        /// <summary>Une los nombres de los elementos (en orden de alta) para formar la descripción.</summary>
        public static string ComponerDescripcion(int listaId)
        {
            var partes = Elementos(listaId).Rows.Cast<DataRow>().Select(f => (string)f["NombreDesc"]);
            var texto = string.Join(" ", partes).Trim();
            return texto.Length > 50 ? texto.Substring(0, 50) : texto;
        }
    }
}
