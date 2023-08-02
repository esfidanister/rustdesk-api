using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RustDeskAPI.Models
{
    public class Utility
    {
        public static string GetMd5(string text)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                int length = data.Length;

                for (int i = 0; i < length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
                return (sb.ToString());
            }
        }
    }
}