using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluxs.Securest.ApiWeb
{
    public static class Hashing
    {
        public static string ComputeMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            var md5 = System.Security.Cryptography.MD5.Create();
            var data = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(data);

            var buffer = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                buffer.AppendFormat("{0:X2}", hash[i]);
            }

            return buffer.ToString();
        }
    }
}