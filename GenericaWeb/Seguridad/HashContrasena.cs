using System;
using System.Security.Cryptography;

namespace GenericaWeb.Seguridad
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2-SHA256.
    /// Formato almacenado (83 caracteres, cabe en varchar(100)): PBKDF2$iteraciones$salBase64$hashBase64
    /// </summary>
    public static class HashContrasena
    {
        private const string Prefijo = "PBKDF2";
        private const int Iteraciones = 100000;
        private const int LongitudSal = 16;
        private const int LongitudHash = 32;
        public const int LongitudMinima = 8;

        /// <summary>Hash válido de una contraseña aleatoria; se usa para igualar tiempos cuando el usuario no existe.</summary>
        public static readonly string HashFicticio = Generar(Guid.NewGuid().ToString());

        public static string Generar(string contrasena)
        {
            var sal = new byte[LongitudSal];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(sal);

            var hash = Derivar(contrasena, sal, Iteraciones, LongitudHash);
            return string.Join("$", Prefijo, Iteraciones.ToString(), Convert.ToBase64String(sal), Convert.ToBase64String(hash));
        }

        public static bool Verificar(string contrasena, string almacenado)
        {
            if (contrasena == null || string.IsNullOrEmpty(almacenado))
                return false;

            var partes = almacenado.Split('$');
            int iteraciones;
            if (partes.Length != 4 || partes[0] != Prefijo || !int.TryParse(partes[1], out iteraciones) || iteraciones <= 0)
                return false;

            byte[] sal, esperado;
            try
            {
                sal = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var real = Derivar(contrasena, sal, iteraciones, esperado.Length);
            return IgualesTiempoConstante(real, esperado);
        }

        private static byte[] Derivar(string contrasena, byte[] sal, int iteraciones, int longitud)
        {
            using (var kdf = new Rfc2898DeriveBytes(contrasena, sal, iteraciones, HashAlgorithmName.SHA256))
                return kdf.GetBytes(longitud);
        }

        private static bool IgualesTiempoConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            var diferencia = 0;
            for (var i = 0; i < a.Length; i++)
                diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }
    }
}
