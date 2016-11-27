using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using MahApps.Metro;
using System.IO;

namespace BusinessCats
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var themeName = BusinessCats.Properties.Settings.Default.ThemeName;
            
            var appTheme = ThemeManager.GetAppTheme(themeName);
            if (appTheme != null)
            {
                ThemeManager.ChangeAppStyle(Application.Current,
                                            ThemeManager.GetAccent("Blue"),
                                            appTheme);
            }

            base.OnStartup(e);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                if (args[0].Equals("-snipandpaste"))
                {
                    Console.WriteLine("Sniping to " + args[1]);
                    string file = Directory.GetCurrentDirectory()+"\\shadowcat.txt";
                    System.IO.File.Delete(file);
                    System.IO.File.WriteAllLines(file, args);
                    Console.ReadLine();
                }
            }
            else {
                Console.WriteLine("Launching");
                var application = new App();
                application.InitializeComponent();
                application.Run();
            }
        }
    }

 
}
