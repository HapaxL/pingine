using System;

namespace pingine.Main
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            new MainWindow().Run(60);
        }
    }
}
