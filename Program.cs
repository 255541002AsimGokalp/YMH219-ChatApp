using YMH219_ChatApp.Forms;

namespace YMH219_ChatApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LauncherForm());
    }
}
