using System;
using System.Windows.Input;
using Test.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml; 
namespace Test.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignupPage : ContentPage
	{
        public ICommand GoBackButton { get; private set; }

        public SignupPage()
        {
            InitializeComponent();
            GoBackButton = new Command(OnGoBackCommandExecuted);
            BindingContext = this;
        }

        private async void OnGoBackCommandExecuted()
        {
            // Navigate to LoginPage using Shell navigation
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

            FullnameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
        }

        private async void Signup_Button_Clicked(object sender, EventArgs e)
        {
            string fullname = FullnameEntry.Text;
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            var existingUser = await App.Database.GetUserAsync(email);
            if (existingUser != null)
            {
                await DisplayAlert("Error", "Email already in use", "OK");
                return;
            }

            User newUser = new User
            {
                Username = email,
                Fullname = fullname,
                Password = password
            };

            await App.Database.SaveUserAsync(newUser);
            await DisplayAlert("Success", "Account created successfully", "OK");

            // Clear the entry fields
            FullnameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void Sign_TapGestureRecognizer(object sender, EventArgs e)
        {
            FullnameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

                FullnameEntry.Text = string.Empty;
                EmailEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                ConfirmPasswordEntry.Text = string.Empty;
            });

            // Return true to indicate the back button press is handled
            return true;
        }
    }
}