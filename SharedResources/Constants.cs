using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources
{
    public class Constants
    {
        public const string WebApiAddress = "https://localhost:7021";
        public const string WebAddress = "https://localhost:7044";
        public const int AccessTokenExpiry = 3600;
        public const int JWTTokenDays = 30;
    }
}