﻿
using System.IO;

namespace MESI_APP.Helpers
{
    public static class Constants
    {
        public static string InitialConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InitialConfig.json");
        public static string LatestConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LatestConfig.json");
        public static int WindowDefaultWidth = 1100;
        public static int WindowDefaultHeight = 700;

    }
}
