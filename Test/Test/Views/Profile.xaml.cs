using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Test.Models;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        public ICommand GoBackButton { get; private set; }
        private User _user;

        public Profile()
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
                FullnameEntry.Text = _user.Fullname;
                EmailEntry.Text = _user.Username;
                CollegeDepartmentEntry.Text = _user.CollegeDepartment;
                RoomNumberEntry.Text = _user.RoomNumber;
                StudentIdEntry.Text = _user.StudentId;
            }
        }

        private async void OnGoBackCommandExecuted()
        {
            // Navigate to DashboardPage using Shell navigation
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullnameEntry?.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry?.Text) ||
                string.IsNullOrWhiteSpace(CollegeDepartmentEntry?.Text) ||
                string.IsNullOrWhiteSpace(RoomNumberEntry?.Text) ||
                string.IsNullOrWhiteSpace(StudentIdEntry?.Text))
            {
                await DisplayAlert("Error", "Please fill in all the required fields", "OK");
                return;
            }

            // Update the user details with new data 
            _user.Fullname = FullnameEntry.Text;
            _user.Username = EmailEntry.Text;
            _user.CollegeDepartment = CollegeDepartmentEntry.Text;
            _user.RoomNumber = RoomNumberEntry.Text;
            _user.StudentId = StudentIdEntry.Text;

            await App.Database.SaveUserAsync(_user);
            await DisplayAlert("Success", "Profile updated successfully", "OK");
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
