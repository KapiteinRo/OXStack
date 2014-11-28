using System;
using System.Security.Cryptography;
using System.Text;

namespace OXStack.Helpers
{
    /// <summary>
    /// Various string functions
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// Compute MD5 hash
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        public static string MD5(string sInput)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] cpData = md5.ComputeHash(Encoding.UTF8.GetBytes(sInput));
                for (int i = 0; i < cpData.Length; i++)
                    sb.Append(cpData[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
