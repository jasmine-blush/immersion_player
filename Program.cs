namespace immersion_player
{
    internal static class Program
    {
        private static TrayApp? _mainTrayApp;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                _mainTrayApp = new TrayApp();
                Application.Run(_mainTrayApp);
            }
            catch(Exception)
            {
                _mainTrayApp?.MainPlayer.Dispose();
                _mainTrayApp?.Dispose();
                throw;
            }
        }
    }
}