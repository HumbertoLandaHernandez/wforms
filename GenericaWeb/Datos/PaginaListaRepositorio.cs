using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>
    /// Sistema.PaginaLista y Sistema.PaginaListaElemento: grupos del menú y, a la vez, la lista de
    /// páginas permitidas que se asigna a cada usuario (Usuario.UsuarioPermisosPaginasId).
    /// </summary>
    public static class PaginaListaRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT pl.PaginaListaId, pl.PaginaListaDesc, pl.PaginaListaComentario, pl.PaginaListaUsys,
       (SELECT COUNT(*) FROM Sistema.PaginaListaElemento e WHERE e.PaginaListaId = pl.PaginaListaId) AS Paginas,
       (SELECT COUNT(*) FROM Sistema.Usuario u WHERE u.UsuarioPermisosPaginasId = pl.PaginaListaId) AS Usuarios
FROM Sistema.PaginaLista pl
ORDER BY pl.PaginaListaDesc");
        }

        public static DataRow Obtener(short listaId)
        {
            return Db.ConsultarFila("SELECT PaginaListaId, PaginaListaDesc, PaginaListaComentario FROM Sistema.PaginaLista WHERE PaginaListaId = @Id",
                Db.P("@Id", listaId));
        }

        public static HashSet<short> Elementos(short listaId)
        {
            var tabla = Db.Consultar("SELECT PaginaId FROM Sistema.PaginaListaElemento WHERE PaginaListaId = @Id", Db.P("@Id", listaId));
            return new HashSet<short>(tabla.Rows.Cast<DataRow>().Select(f => (short)f[0]));
        }

        /// <summary>Inserta o actualiza la lista y reemplaza sus páginas. Devuelve el Id.</summary>
        public static short Guardar(short listaId, string desc, string comentario, IEnumerable<short> paginas, string usys)
        {
            var id = listaId;
            Db.Transaccion((c, t) =>
            {
                if (id == 0)
                {
                    id = Convert.ToInt16(Db.Escalar(c, t, @"
INSERT INTO Sistema.PaginaLista (PaginaListaUsys, PaginaListaDesc, PaginaListaComentario) VALUES (@Usys, @Desc, @Comentario);
SELECT CAST(SCOPE_IDENTITY() AS int);",
                        Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50), Db.Texto("@Comentario", comentario, 500)));
                }
                else
                {
                    Db.Ejecutar(c, t, @"UPDATE Sistema.PaginaLista SET PaginaListaUsys = @Usys, PaginaListadt = SYSDATETIME(),
                                        PaginaListaDesc = @Desc, PaginaListaComentario = @Comentario WHERE PaginaListaId = @Id",
                        Db.P("@Id", id), Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50), Db.Texto("@Comentario", comentario, 500));
                }

                Db.Ejecutar(c, t, "DELETE FROM Sistema.PaginaListaElemento WHERE PaginaListaId = @Id", Db.P("@Id", id));
                foreach (var paginaId in paginas.Distinct())
                {
                    Db.Ejecutar(c, t, @"INSERT INTO Sistema.PaginaListaElemento (PaginaListaElementoUsys, PaginaListaId, PaginaId)
                                        VALUES (@Usys, @Id, @PaginaId)",
                        Db.Texto("@Usys", usys, 30), Db.P("@Id", id), Db.P("@PaginaId", paginaId));
                }
            });
            return id;
        }

        /// <summary>Elimina la lista. Devuelve false si algún usuario la tiene asignada.</summary>
        public static bool Eliminar(short listaId)
        {
            var usada = Convert.ToInt32(Db.Escalar("SELECT COUNT(1) FROM Sistema.Usuario WHERE UsuarioPermisosPaginasId = @Id", Db.P("@Id", (int)listaId)));
            if (usada > 0)
                return false;

            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PaginaListaElemento WHERE PaginaListaId = @Id", Db.P("@Id", listaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PaginaLista WHERE PaginaListaId = @Id", Db.P("@Id", listaId));
            });
            return true;
        }
    }
}
