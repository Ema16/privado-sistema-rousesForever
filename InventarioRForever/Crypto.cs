using System.Security.Cryptography;
using System.Text;

namespace InventarioRForever
{
    public class Crypto
    {
        public static string Hash(string value)
        {

            // Create an instance of the hashing algorithm you want to use
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value of the input data
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));

                // Convert the hash bytes to a hexadecimal string
                string hashString = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hashString;
            }
        }
    }
}
