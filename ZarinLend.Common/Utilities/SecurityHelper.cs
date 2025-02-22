using Common.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities
{
    public enum HashMethod
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        MD5,
    }
    public static class SecurityHelper
    {
        #region Hash methods
        public static string GetHash(this string input, HashMethod hashMethod)
        {
             HashAlgorithm hashAlgorithm=null;
            switch ( hashMethod)
            {
                case HashMethod.SHA1:
                    hashAlgorithm = SHA1.Create();
                    break;
                case HashMethod.SHA256:
                    hashAlgorithm = SHA256.Create();
                    break;
                case HashMethod.SHA384:
                    hashAlgorithm = SHA384.Create();
                    break;
                case HashMethod.SHA512:
                    hashAlgorithm = SHA512.Create();
                    break;
                case HashMethod.MD5:
                    hashAlgorithm = MD5.Create();
                    break;
            }

            if (hashAlgorithm != null)
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < byteHash.Length; i++)
                {
                    builder.Append(byteHash[i].ToString("x2"));
                }
                hashAlgorithm.Dispose();
                hashAlgorithm.Clear();
                return builder.ToString();
            }

            throw new AppException("'hashMethod' is not valid!");
        }
        public static string GetSha256Hash(this string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = sha256.ComputeHash(byteValue);
                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < byteHash.Length; i++)
                {
                    builder.Append(byteHash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GetSha512Hash(this string input)
        {
            using (var sha512 = SHA512.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = sha512.ComputeHash(byteValue);
                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < byteHash.Length; i++)
                {
                    builder.Append(byteHash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion Hash methods

        #region Encrypt/Encrypted methods
        public static string StringToBase64(this string encrString)
        {
            byte[] array;
            string decrypted;
            try
            {
                array = Convert.FromBase64String(encrString);
                decrypted = Encoding.UTF8.GetString(array);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        public static string Base64ToString(this string strEncrypted)
        {
            byte[] array = Encoding.UTF8.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(array);
            return encrypted;
        }

        private readonly static byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        private static byte[] DeriveKeyFromPassword(string password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            var hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }

        public static async Task<byte[]> EncryptAsync(string clearText, string passPhrase)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = DeriveKeyFromPassword(passPhrase);
            aes.IV = IV;
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
            await cryptoStream.FlushFinalBlockAsync();
            return output.ToArray();
        }

        public static async Task<string> EncryptAsStringAsync(string clearText, string passPhrase)
        {
            var encryptArray = await EncryptAsync(clearText, passPhrase);
            return Convert.ToBase64String(encryptArray);
        }

        public static async Task<string> DecryptAsync(byte[] encrypted, string passphrase)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = DeriveKeyFromPassword(passphrase);
            aes.IV = IV;
            using MemoryStream input = new(encrypted);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);
            return Encoding.Unicode.GetString(output.ToArray());
        }

        public static async Task<string> DecryptAsync(this string encrypted, string passPhrase)
        {
            encrypted = encrypted.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encrypted);
            return await DecryptAsync(cipherBytes, passPhrase);
        }

        #endregion Encrypt/Encrypted methods
    }
}
