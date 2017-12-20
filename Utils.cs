using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeespinClub
{
   static class Utils
    {
        public class Converter
        {
            public static byte[] ConvertToKOKData(string data)
            {
                 //Copyright Weespin :)
                var checksum = BitConverter.GetBytes(Convert.ToInt16(data.Length));

                Array.Reverse(checksum, 0, checksum.Length);
                var databin = Encoding.ASCII.GetBytes(data);
				
                return Combine(checksum, databin);
            }
            private static byte[] Combine(byte[] first, byte[] second)
            {
                byte[] ret = new byte[first.Length + second.Length];
                Buffer.BlockCopy(first, 0, ret, 0, first.Length);
                Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
                return ret;
            }
            public static string ByteArrayToString(byte[] ba)
            {
                string hex = BitConverter.ToString(ba);
                return hex.Replace("-", "");
            }
            public static decimal ByteArrayToDecimal(byte[] src, int offset)
            {
                var i1 = BitConverter.ToInt32(src, offset);
                var i2 = BitConverter.ToInt32(src, offset + 4);
                var i3 = BitConverter.ToInt32(src, offset + 8);
                var i4 = BitConverter.ToInt32(src, offset + 12);

                return new decimal(new int[] { i1, i2, i3, i4 });
            }
        }  
        public static List<Proxyes> LoadProxy(string path)
        {
            List<Proxyes> ret = new List<Proxyes>();
            var file = File.ReadAllLines(path);
            foreach (var s in file)
            {
                var ip = s.Before(":");
                var port = s.After(":");
                ret.Add(new Proxyes() { IP = ip, port = port });
            }
            return ret;
        }
        public static List<Account> GetAccountsFromFile(string path)
        {
            List<Account> ret = new List<Account>();
            var file = File.ReadAllLines(path);
            foreach (var s in file)
            {
                var id = s.Before(":");
                var auth_key = s.After(":");
                ret.Add(new Account(){auth_key=auth_key,id=int.Parse(id)});
            }
            return ret;
        }

        internal class Proxyes
        {
            public string IP;
            public string port;
        }
        internal class Account
        {
            public int id;
            public string auth_key;

            
        }

    }
    static class SubstringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}
