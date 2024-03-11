namespace RSLib.Encryption
{
    using System.Security.Cryptography;

    /// <summary>
    /// Class containing methods to encrypt a string and decrypt a bytes array.
    /// Key and IV are passed in as constructor parameters, where Key must be an array of 32 bytes and IV an array of 16 bytes.
    /// The encryption/decryption methods then use System.Security.Cryptography.RijndaelManaged.
    /// </summary>
    public sealed class Rijndael
    {
        private const uint KEY_LENGTH = 32;
        private const uint IV_LENGTH = 16;

        private byte[] _key;
        private byte[] _iv;

        public Rijndael(byte[] key, byte[] iv)
        {
            if (key == null)
                throw new System.ArgumentNullException(nameof(key));
            if (iv == null)
                throw new System.ArgumentNullException(nameof(iv));
            if (key.Length != KEY_LENGTH)
                throw new System.ArgumentException($"{nameof(key)} length must be equal to {KEY_LENGTH}!");
            if (iv.Length != IV_LENGTH)
                throw new System.ArgumentException($"{nameof(iv)} length must be equal to {IV_LENGTH}!");

            _key = key;
            _iv = iv;
        }

        /// <summary>
        /// Encrypts a string value to a bytes array, using Key and IV that have been passed as parameters during Rijndael construction.
        /// </summary>
        /// <param name="plainText">String value to encrypt.</param>
        /// <returns>Encrypted value as bytes array.</returns>
        public byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new System.ArgumentNullException($"{nameof(plainText)} is null or empty!");

            byte[] encrypted = null;

            try
            {
                using (RijndaelManaged rijndael = new RijndaelManaged())
                {
                    rijndael.Key = _key;
                    rijndael.IV = _iv;
                    ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                    using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (System.IO.StreamWriter swEncrypt = new System.IO.StreamWriter(csEncrypt))
                                swEncrypt.Write(plainText);

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypts a bytes array to a string, using Key and IV that have been passed as parameters during Rijndael construction.
        /// </summary>
        /// <param name="cipherText">Bytes array to decrypt.</param>
        /// <returns>Decrypted string value.</returns>
        public string Decrypt(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new System.ArgumentNullException($"{nameof(cipherText)} is null or empty!");

            string decrypted = string.Empty;

            try
            {
                using (RijndaelManaged rijndael = new RijndaelManaged())
                {
                    rijndael.Key = _key;
                    rijndael.IV = _iv;
                    ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                    using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(cipherText))
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            using (System.IO.StreamReader srDecrypt = new System.IO.StreamReader(csDecrypt))
                                decrypted = srDecrypt.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            return decrypted;
        }
    }
}