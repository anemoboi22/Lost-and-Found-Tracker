using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class UserDetailsView : ContentPage
    {
        private int _userId;
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                LoadUserDetails(_userId);
            }
        }

        public ICommand GoBackButton { get; private set; }

        public UserDetailsView()
        {
            InitializeComponent();
            GoBackButton = new Command(OnGoBackCommandExecuted);
            BindingContext = this;
        }

        private async void LoadUserDetails(int userId)
        {
            try
            {
                // Retrieve user details from the database
                var user = await App.Database.GetUserAsync(userId);
                if (user != null)
                {
                    // Display user details
                    FullnameLabel.Text = user.Fullname;
                    StudentIdLabel.Text = user.StudentId;
                    CollegeDepartmentLabel.Text = user.CollegeDepartment;
                    RoomNumberLabel.Text = user.RoomNumber;
                }
                else
                {
                    await DisplayAlert("Error", "User details not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to retrieve user details: {ex.Message}", "OK");
            }
        }

        private async void OnGoBackCommandExecuted()
        {
            // Navigate to DashboardPage using Shell navigation
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
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
