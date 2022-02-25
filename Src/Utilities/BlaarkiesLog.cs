using System;
using System.Collections.Generic;

namespace Stargate.Utilities
{
    public class BlaarkiesLog
    {
        private const string MOD_NAME = "[STARGATE]";

        private static readonly Dictionary<string, DateTime> LastDebugDateTimeMap
            = new Dictionary<string, DateTime>();

        public static void OnScreen(string message)
        {
            ScreenMessages.PostScreenMessage($"{MOD_NAME} {message}", 60);
        }

        public static void Debug(
            string message,
            string category = "default",
            float waitSeconds = 1f)
        {
            var containsKey = LastDebugDateTimeMap.ContainsKey(category);
            if (!containsKey)
            {
                // initialize date in past, so that the first log for this category works
                var yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                LastDebugDateTimeMap.Add(category, yesterday);
            }

            var lastDebugTime = LastDebugDateTimeMap[category];
            if (waitSeconds == 0
                || DateTime.Now - lastDebugTime > TimeSpan.FromSeconds(waitSeconds))
            {
                LastDebugDateTimeMap[category] = DateTime.Now;
                UnityEngine.Debug.Log($"{MOD_NAME}[Debug] {message}");
            }
        }
    }
}
