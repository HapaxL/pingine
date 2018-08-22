using System;

namespace pingine.Main
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            var window = new MainWindow();
            window.Run(Config.UPS); // option sets the amount of times OnUpdateFrame(e) is called every second
        }
    }
}
