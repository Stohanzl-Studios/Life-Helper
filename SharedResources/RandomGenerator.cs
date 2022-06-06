using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources
{
    public static class RandomGenerator
    {
        public static int Next(int min, int max) { return RandomNumberGenerator.GetInt32(min, max); }
        public static string NextString(int length) { return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length)).Remove(length - 1); }
        public static string NextStringInt(int length)
        {
            string result = "";
            for(int i = 0; i < length; i++)
            {
                result += RandomNumberGenerator.GetInt32(0, 10);
            }
            return result;
        }
    }
}
