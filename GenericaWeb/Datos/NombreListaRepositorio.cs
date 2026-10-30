using System.Data;

namespace GenericaWeb.Datos
{
    /// <summary>Sistema.NombreLista: partes (tipo + base) que componen cada nombre personal.</summary>
    public static class NombreListaRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT nl.NombrePersonalId, np.NombrePersonalDesc, nl.NombreTipoId, nt.NombreTipoDesc,
       nl.NombreBaseId, nb.NombreBaseDesc, nl.NombreListaUsys
FROM Sistema.NombreLista nl
JOIN Sistema.NombrePersonal np ON np.NombrePersonalId = nl.NombrePersonalId
JOIN Sistema.NombreTipo nt     ON nt.NombreTipoId = nl.NombreTipoId
JOIN Sistema.NombreBase nb     ON nb.NombreBaseId = nl.NombreBaseId
ORDER BY np.NombrePersonalDesc, nl.NombreTipoId");
        }

        public static void Agregar(int nombrePersonalId, byte nombreTipoId, int nombreBaseId, string usys)
        {
            Db.Ejecutar(@"INSERT INTO Sistema.NombreLista (NombreListaUsys, NombrePersonalId, NombreTipoId, NombreBaseId)
                          VALUES (@Usys, @Personal, @Tipo, @Base)",
                Db.Texto("@Usys", usys, 30), Db.P("@Personal", nombrePersonalId), Db.P("@Tipo", nombreTipoId), Db.P("@Base", nombreBaseId));
        }

        public static void Eliminar(int nombrePersonalId, byte nombreTipoId, int nombreBaseId)
        {
            Db.Ejecutar("DELETE FROM Sistema.NombreLista WHERE NombrePersonalId = @Personal AND NombreTipoId = @Tipo AND NombreBaseId = @Base",
                Db.P("@Personal", nombrePersonalId), Db.P("@Tipo", nombreTipoId), Db.P("@Base", nombreBaseId));
        }
    }
}
