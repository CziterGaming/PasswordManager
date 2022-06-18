using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager
{
    internal static class EncryptionManager
    {
        internal static string globalPassword;
        public static string EncryptString(string input, string password)
        {
            byte[] bytesToBeEncrypted = UTF8Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = UTF8Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }
        public static string DecryptString(string input, string password)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = UTF8Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            return UTF8Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Mode = CipherMode.ECB;
                AES.Padding = PaddingMode.PKCS7;
                AES.Key = passwordBytes;

                using (ICryptoTransform encrypto = AES.CreateEncryptor())
                {
                    encryptedBytes = encrypto.TransformFinalBlock(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                }
            }

            return encryptedBytes;
        }
        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Mode = CipherMode.ECB;
                AES.Padding = PaddingMode.PKCS7;
                AES.Key = passwordBytes;

                using (ICryptoTransform decrypto = AES.CreateDecryptor())
                {
                    decryptedBytes = decrypto.TransformFinalBlock(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                }
            }

            return decryptedBytes;
        }
    }
}
