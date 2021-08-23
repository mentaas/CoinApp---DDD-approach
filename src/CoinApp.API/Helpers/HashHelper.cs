using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.API.Helpers
{
    public class HashHelper
    {
        public string Hash { get; private set; }

        public string Salt { get; private set; }

        public HashHelper()
        {

        }

        public HashHelper(string password)
        {
            var saltBytes = new byte[32];
            using (var provider = new RNGCryptoServiceProvider())
                provider.GetNonZeroBytes(saltBytes);
            Salt = Convert.ToBase64String(saltBytes);
            Hash = ComputeHash(Salt, password);
        }

        public static string ComputeHash(string salt, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 1000))
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }

        public static bool Verify(string salt, string hash, string password)
        {
            if (salt != null && hash != null)
            {
                string computedHash = ComputeHash(salt, password);
                return hash == computedHash;
            }
            return false;
        }

        public static string GetSha256FromString(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }
}
