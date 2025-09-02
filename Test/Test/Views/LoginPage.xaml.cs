using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            string username = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            var user = await App.Database.GetUserAsync(username);

            if (user == null)
            {
                await DisplayAlert("Error", "User does not exist.", "OK");
                PasswordEntry.Text = string.Empty;
                return;
            }

            if (user.Password != password)
            {
                await DisplayAlert("Error", "Invalid password", "OK");
                PasswordEntry.Text = string.Empty;
                return;
            }

            App.CurrentUserEmail = username;
            PasswordEntry.Text = string.Empty;
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async void Sign_TapGestureRecognizer(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(SignupPage)}");
        }
    }
}
