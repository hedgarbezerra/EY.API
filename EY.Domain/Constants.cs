using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain
{
    public static class Constants
    {
        public static class Options
        {
            public static class Tasks
            {
                public const string IpAddressUpdater_RepeatInMinutesKey = "Tasks:IpAddressUpdater:RepeatEveryMinutes";
            }
            public static class Api
            {
                public const string VersionKey = "API:Version";
                public const string ApiName = "API:Name";
            }
        }

        public static class ConnectionStrings
        {
            public const string SqlServer = "SqlServerInstance";
        }
    }
}
