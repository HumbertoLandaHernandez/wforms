using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>Sistema.ListaPagina (grupos del menú) y Sistema.ListaPaginaElemento (páginas de cada grupo).</summary>
    public static class ListaPaginaRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT lp.ListaPaginaId, lp.ListaPaginaDesc, lp.ListaPaginaComentario, lp.ListaPaginaUsys,
       (SELECT COUNT(*) FROM Sistema.ListaPaginaElemento e WHERE e.ListaPaginaId = lp.ListaPaginaId) AS Paginas
FROM Sistema.ListaPagina lp
ORDER BY lp.ListaPaginaDesc");
        }

        public static DataRow Obtener(byte listaId)
        {
            return Db.ConsultarFila("SELECT ListaPaginaId, ListaPaginaDesc, ListaPaginaComentario FROM Sistema.ListaPagina WHERE ListaPaginaId = @Id",
                Db.P("@Id", listaId));
        }

        public static HashSet<short> Elementos(byte listaId)
        {
            var tabla = Db.Consultar("SELECT PaginaId FROM Sistema.ListaPaginaElemento WHERE ListaPaginaId = @Id", Db.P("@Id", (short)listaId));
            return new HashSet<short>(tabla.Rows.Cast<DataRow>().Select(f => (short)f[0]));
        }

        /// <summary>Inserta o actualiza la lista y reemplaza sus páginas. Devuelve el Id.</summary>
        public static byte Guardar(byte listaId, string desc, string comentario, IEnumerable<short> paginas, string usys)
        {
            var id = listaId;
            Db.Transaccion((c, t) =>
            {
                if (id == 0)
                {
                    id = Convert.ToByte(Db.Escalar(c, t, @"
INSERT INTO Sistema.ListaPagina (ListaPaginaUsys, ListaPaginaDesc, ListaPaginaComentario) VALUES (@Usys, @Desc, @Comentario);
SELECT CAST(SCOPE_IDENTITY() AS int);",
                        Db.Texto("@Usys", usys, 50), Db.Texto("@Desc", desc, 50), Db.Texto("@Comentario", comentario, 500)));
                }
                else
                {
                    Db.Ejecutar(c, t, @"UPDATE Sistema.ListaPagina SET ListaPaginaUsys = @Usys, ListaPaginaDesc = @Desc,
                                        ListaPaginaComentario = @Comentario WHERE ListaPaginaId = @Id",
                        Db.P("@Id", id), Db.Texto("@Usys", usys, 50), Db.Texto("@Desc", desc, 50), Db.Texto("@Comentario", comentario, 500));
                }

                Db.Ejecutar(c, t, "DELETE FROM Sistema.ListaPaginaElemento WHERE ListaPaginaId = @Id", Db.P("@Id", (short)id));
                foreach (var paginaId in paginas.Distinct())
                {
                    Db.Ejecutar(c, t, @"INSERT INTO Sistema.ListaPaginaElemento (ListaPaginaElementoUsys, ListaPaginaId, PaginaId)
                                        VALUES (@Usys, @Id, @PaginaId)",
                        Db.Texto("@Usys", usys, 309), Db.P("@Id", (short)id), Db.P("@PaginaId", paginaId));
                }
            });
            return id;
        }

        public static void Eliminar(byte listaId)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.ListaPaginaElemento WHERE ListaPaginaId = @Id", Db.P("@Id", (short)listaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.ListaPagina WHERE ListaPaginaId = @Id", Db.P("@Id", listaId));
            });
        }
    }
}
