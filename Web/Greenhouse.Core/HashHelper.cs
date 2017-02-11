using System.Security.Cryptography;
using System.Text;

namespace Greenhouse.Core
{
    public static class HashHelper
    {
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Unicode.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));          
            }
            return sBuilder.ToString();
        }
    }
}