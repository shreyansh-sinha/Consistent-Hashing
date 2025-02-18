using System.Security.Cryptography;
using System.Text;

namespace ConsistentHashing.Helpers
{
    public static class Calculations
    {

        public static int CalculateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convert the string to a byte array
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Compute the hash
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                int hashNumber = BitConverter.ToInt32(hashBytes, 0);

                return Math.Abs(hashNumber);
            }
        }

        public static string GenerateUniqueRandomString()
        {
            Random random = new Random();

            // Generate 4 random numbers (0-255) for each octet of the IP address
            int octet1 = random.Next(0, 256);
            int octet2 = random.Next(0, 256);
            int octet3 = random.Next(0, 256);
            int octet4 = random.Next(0, 256);

            string timestamp = DateTime.Now.Ticks.ToString();

            // Return the formatted IP address
            return $"{octet1}.{octet2}.{octet3}.{octet4}_{timestamp}";
        }
    }
}
