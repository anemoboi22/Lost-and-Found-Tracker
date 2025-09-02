using System.Linq;
using System.Windows.Input;
using Test.Views;
using Xamarin.Forms;

namespace Test
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public ICommand LogoutCommand { get; }

        public AppShell()
        {
            InitializeComponent();
            LogoutCommand = new Command(OnLogout);
            BindingContext = this;
            Routing.RegisterRoute(nameof(UserDetailsView), typeof(UserDetailsView));
        }

        private async void OnLogout()
        {
            bool answer = await DisplayAlert(
                "Logout",
                "Are you sure you want to log out?",
                "Yes",
                "No");

            if (answer)
            {
                // Clear user session
                App.CurrentUserEmail = null;

                // Navigate to the LoginPage
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
