using System;
using System.IO;
using System.Threading.Tasks;
using Test.Services;
using Test.Views;
using Xamarin.Forms;

namespace Test
{
    public partial class App : Application
    {
        public static string CurrentUserEmail { get; set; }
        static DatabaseHelper database;

        public static DatabaseHelper Database
        {
            get
            {
                if (database == null)
                {
                    database = new DatabaseHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Test.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new SplashScreen();
            LoadAppResources();
        }

        private async void LoadAppResources()
        {
            await Task.Delay(2000);
            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            // Handle when the app sleeps
        }

        protected override void OnResume()
        {
            // Handle when the app resumes
        }
    }
}
