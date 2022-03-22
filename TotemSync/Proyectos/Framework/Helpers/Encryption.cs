using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Helpers
{
    public static class Encryption
    {
        private const string Key = "MyPr1v4t3_K3Y$$_JMotion!";
        private static readonly byte[] salt = new byte[] { 27, 15, 78, 34, 62, 45, 84, 51, 255, 140, 66}; 

        public static string Encrypt(string value)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(value);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Key);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string encryptedResult = Convert.ToBase64String(bytesEncrypted);

            return encryptedResult;
        }

        public static string Decrypt(string value)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(value);
            byte[] passwordBytesDecrypt = Encoding.UTF8.GetBytes(Key);
            passwordBytesDecrypt = SHA256.Create().ComputeHash(passwordBytesDecrypt);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytesDecrypt);

            string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);

            Console.WriteLine("Decrypted: " + decryptedResult);

            return decryptedResult;
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }

            return decryptedBytes;
        }
    }
}


