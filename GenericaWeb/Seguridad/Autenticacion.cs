using GenericaWeb.Datos;
using GenericaWeb.Modelos;

namespace GenericaWeb.Seguridad
{
    public static class Autenticacion
    {
        /// <summary>Valida usuario y contraseña. Devuelve null si las credenciales no son válidas.</summary>
        public static UsuarioSesion Autenticar(string usuarioDesc, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(usuarioDesc) || string.IsNullOrEmpty(contrasena))
                return null;

            string hash;
            var usuario = UsuarioRepositorio.ObtenerParaLogin(usuarioDesc.Trim(), out hash);

            // Se calcula el hash aunque el usuario no exista para no revelarlo por diferencia de tiempos.
            var valida = HashContrasena.Verificar(contrasena, hash ?? HashContrasena.HashFicticio);
            return valida && usuario != null ? usuario : null;
        }
    }
}
