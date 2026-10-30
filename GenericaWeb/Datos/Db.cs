using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace GenericaWeb.Datos
{
    /// <summary>Acceso a SQL Server con ADO.NET y consultas parametrizadas.</summary>
    public static class Db
    {
        private static string CadenaConexion => ConfigurationManager.ConnectionStrings["Generica"].ConnectionString;

        public static SqlConnection Abrir()
        {
            var conexion = new SqlConnection(CadenaConexion);
            conexion.Open();
            return conexion;
        }

        public static DataTable Consultar(string sql, params SqlParameter[] parametros)
        {
            using (var conexion = Abrir())
            using (var comando = Crear(conexion, null, sql, parametros))
            using (var adaptador = new SqlDataAdapter(comando))
            {
                var tabla = new DataTable();
                adaptador.Fill(tabla);
                return tabla;
            }
        }

        public static DataRow ConsultarFila(string sql, params SqlParameter[] parametros)
        {
            var tabla = Consultar(sql, parametros);
            return tabla.Rows.Count > 0 ? tabla.Rows[0] : null;
        }

        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            using (var conexion = Abrir())
            using (var comando = Crear(conexion, null, sql, parametros))
                return comando.ExecuteScalar();
        }

        public static int Ejecutar(string sql, params SqlParameter[] parametros)
        {
            using (var conexion = Abrir())
            using (var comando = Crear(conexion, null, sql, parametros))
                return comando.ExecuteNonQuery();
        }

        public static int Ejecutar(SqlConnection conexion, SqlTransaction transaccion, string sql, params SqlParameter[] parametros)
        {
            using (var comando = Crear(conexion, transaccion, sql, parametros))
                return comando.ExecuteNonQuery();
        }

        public static object Escalar(SqlConnection conexion, SqlTransaction transaccion, string sql, params SqlParameter[] parametros)
        {
            using (var comando = Crear(conexion, transaccion, sql, parametros))
                return comando.ExecuteScalar();
        }

        /// <summary>Ejecuta el trabajo en una transacción; si lanza excepción se revierte.</summary>
        public static void Transaccion(Action<SqlConnection, SqlTransaction> trabajo)
        {
            using (var conexion = Abrir())
            using (var transaccion = conexion.BeginTransaction())
            {
                trabajo(conexion, transaccion);
                transaccion.Commit();
            }
        }

        public static SqlParameter P(string nombre, object valor) =>
            new SqlParameter(nombre, valor ?? DBNull.Value);

        /// <summary>Parámetro varchar (evita conversiones implícitas nvarchar→varchar).</summary>
        public static SqlParameter Texto(string nombre, string valor, int longitud) =>
            new SqlParameter(nombre, SqlDbType.VarChar, longitud) { Value = (object)valor ?? DBNull.Value };

        /// <summary>Mensaje amigable para errores comunes de SQL Server.</summary>
        public static string MensajeError(SqlException ex)
        {
            switch (ex.Number)
            {
                case 547:
                    return "No se puede completar la operación: el registro está relacionado con otros datos.";
                case 2601:
                case 2627:
                    return "Ya existe un registro con ese valor.";
                default:
                    Trace.TraceError(ex.ToString());
                    return "Ocurrió un error en la base de datos.";
            }
        }

        private static SqlCommand Crear(SqlConnection conexion, SqlTransaction transaccion, string sql, SqlParameter[] parametros)
        {
            var comando = new SqlCommand(sql, conexion, transaccion);
            if (parametros != null && parametros.Length > 0)
                comando.Parameters.AddRange(parametros);
            return comando;
        }
    }
}
