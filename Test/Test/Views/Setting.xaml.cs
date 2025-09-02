using System;
using System.Windows.Input;
using Test.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Setting : ContentPage
    {
        public ICommand GoBackButton { get; private set; }
        private User _user;

        public Setting()
        {
            InitializeComponent();
            GoBackButton = new Command(OnGoBackCommandExecuted);
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Assuming you have a method to get the currently logged-in user
            _user = await App.Database.GetUserAsync(App.CurrentUserEmail);
            if (_user != null)
            {
                oldPasswordEntry.Text = _user.Password;
            }
        }

        private async void OnGoBackCommandExecuted()
        {
            // Navigate to LoginPage using Shell navigation
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            string newPassword = newPasswordEntry.Text;
            string confirmPassword = confirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (newPassword != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            try
            {
                _user.Password = newPassword; // Update user's password
                await App.Database.SaveUserAsync(_user);

                // Update the oldPasswordEntry to display the new password
                oldPasswordEntry.Text = newPassword;

                // Clear the new password fields
                newPasswordEntry.Text = string.Empty;
                confirmPasswordEntry.Text = string.Empty;

                await DisplayAlert("Success", "Password updated successfully", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update password: {ex.Message}", "OK");
            }
        }

        private async void Delete_Button_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete your account and all your posts?", "Yes", "No");
            if (!confirm) return;

            try
            {
                // Retrieve and delete all posts associated with the user
                var userPosts = await App.Database.GetPostsAsync();
                userPosts = userPosts.FindAll(post => post.UserId == _user.Id);

                foreach (var post in userPosts)
                {
                    // Optionally delete associated photos here
                    // For example, delete from local storage if needed

                    await App.Database.DeletePostAsync(post);
                }

                // Delete the user
                await App.Database.DeleteUserAsync(_user);

                await DisplayAlert("Success", "Account and posts deleted successfully", "OK");

                // Navigate to LoginPage or a suitable page
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete account and posts: {ex.Message}", "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Override the back button press to navigate to the DashboardPage
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            });

            // Return true to indicate the back button press is handled
            return true;
        }
    }
}
