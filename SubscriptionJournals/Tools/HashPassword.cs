using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionJournals.Tools
{
    public class HashPassword
    {
        public static string HashPass(string password)
        {
            //using secure Hash Algorithm for encrpyt and decrypt password
            using (var secureHash = new SHA1Managed())
            {
                var hash = Encoding.UTF8.GetBytes(password);
                var generatedHash = secureHash.ComputeHash(hash);
                var generatedHashStr = Convert.ToBase64String(generatedHash);
                return generatedHashStr;
            }
        }
    }
}
