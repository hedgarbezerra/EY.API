using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.IntegrationTests
{
    public static class IPAddressGenerator
    {
        private static Random random = new Random();

        public static string Generate()
        {
            return string.Join(".", random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        }

    }
}
