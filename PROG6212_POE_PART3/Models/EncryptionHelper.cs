using System.Security.Cryptography;
using System.Text;

namespace PROG6212_POE_PART3.Models
{
    public static class EncryptionHelper
    {
        // Secret key used for encryption/decryption (keep it safe!)
        private static readonly string Key = "CMCS@2025#EncryptionKey!";

        // Create and configure AES encryption object
        private static Aes CreateAes()
        {
            var aes = Aes.Create();

            // Derive a 256-bit key from the secret string and a salt
            var keyBytes = new Rfc2898DeriveBytes(Key, Encoding.UTF8.GetBytes("S@ltKey")).GetBytes(32);
            aes.Key = keyBytes;

            // Initialize IV to zero bytes (deterministic, but not recommended for production)
            aes.IV = new byte[16];
            return aes;
        }

        // Encrypts a file from inputPath to outputPath
        public static void EncryptFile(string inputPath, string outputPath)
        {
            using var aes = CreateAes();
            using var fsInput = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            // Create a crypto stream for writing encrypted bytes
            using var cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write);

            // Copy original file content to crypto stream (encrypts on-the-fly)
            fsInput.CopyTo(cs);
        }

        // Decrypts a file from inputPath to outputPath
        public static void DecryptFile(string inputPath, string outputPath)
        {
            using var aes = CreateAes();
            using var fsInput = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            // Create a crypto stream for reading decrypted bytes
            using var cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);

            // Copy decrypted content to output file
            cs.CopyTo(fsOutput);
        }

        // Decrypts an encrypted file into a provided stream (for in-memory operations)
        public static void DecryptFileToStream(string encryptedFilePath, Stream outputStream)
        {
            using var aes = CreateAes();
            using var fsInput = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read);

            // CryptoStream to read decrypted bytes into the output stream
            using var cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);

            // Copy decrypted content to the provided stream
            cs.CopyTo(outputStream);

            // Reset stream position to start for reading
            outputStream.Position = 0;
        }
    }
}
