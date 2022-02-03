namespace Stargate.Utilities
{
    public class BlaarkiesLog
    {
        private const string MOD_NAME = "[STARGATE]";

        public static void OnScreen(string message)
        {
            ScreenMessages.PostScreenMessage($"{MOD_NAME} {message}", 60);
        }
    }
}
