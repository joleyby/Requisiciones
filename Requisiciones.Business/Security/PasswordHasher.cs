using System.Security.Cryptography;
using Requisiciones.Entities;

namespace Requisiciones.Business.Security
{
    /// <summary>PBKDF2-SHA256 con salt por usuario (nunca se guarda la contraseña).</summary>
    public static class PasswordHasher
    {
        private const int LongitudHash = 32;
        private const int LongitudSalt = 16;
        public const int IteracionesPorDefecto = 10000;

        public static byte[] Hash(string password, byte[] salt, int iteraciones)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iteraciones, HashAlgorithmName.SHA256))
                return kdf.GetBytes(LongitudHash);
        }

        public static bool Verificar(string password, CredencialDto credencial)
        {
            var calculado = Hash(password, credencial.Salt, credencial.Iteraciones);
            return CompararEnTiempoConstante(calculado, credencial.PasswordHash);
        }

        public static byte[] NuevaSalt()
        {
            var salt = new byte[LongitudSalt];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            return salt;
        }

        private static bool CompararEnTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
