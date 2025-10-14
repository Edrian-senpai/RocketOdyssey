using System;

namespace RocketOdyssey.Database
{
    public static class SessionManager
    {
        public static string CurrentUsername { get; set; }

        // Prevent save exploits during launch countdown
        public static bool IsLaunchCountdownActive { get; set; } = false;

        // Detect if user exited during countdown
        public static bool RequiresRestartOnReturn { get; set; } = false;
    }
}
