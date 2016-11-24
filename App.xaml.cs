using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using MahApps.Metro;


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
            Console.WriteLine("Parametros");
            foreach(string arg in args)
            {
                Console.WriteLine("Argument: " + arg);
            }
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }

 
}
