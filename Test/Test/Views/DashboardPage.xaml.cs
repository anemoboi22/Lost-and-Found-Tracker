using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                // Load posts with full user names and set the binding context
                await LoadPostsAndUpdateUsernames();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task LoadPostsAndUpdateUsernames()
        {
            try
            {
                var posts = await App.Database.GetPostsAsync();

                // Load all usernames in parallel
                var userTasks = posts.Select(post => UpdateUserNameAsync(post));
                await Task.WhenAll(userTasks);

                // Update the UI on the main thread
                Device.BeginInvokeOnMainThread(() =>
                {
                    BindingContext = posts.OrderByDescending(post => post.CreationDate).ToList();
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task UpdateUserNameAsync(Post post)
        {
            try
            {
                var user = await App.Database.GetUserAsync(post.UserId).ConfigureAwait(false);
                if (user != null)
                {
                    // Update the UI on the main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        post.UserName = user.Fullname;
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert("Delete", "Are you sure you want to delete this post?", "Yes", "No").ConfigureAwait(false);
                if (answer)
                {
                    var post = (Post)((ImageButton)sender).BindingContext;

                    var user = await App.Database.GetUserAsync(App.CurrentUserEmail).ConfigureAwait(false);

                    if (user != null && user.Id == post.UserId)
                    {
                        // Delete post from the database
                        await App.Database.DeletePostAsync(post).ConfigureAwait(false);

                        // Refresh the posts list
                        await LoadPostsAndUpdateUsernames();
                    }
                    else
                    {
                        // Show error message on the main thread
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Error", "You are not authorized to delete this post.", "OK");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void OnEditorTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(CreatePostPage)}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void OnUserDetailsViewClicked(object sender, EventArgs e)
        {
            try
            {
                var post = (Post)((ImageButton)sender).BindingContext;
                await Shell.Current.GoToAsync($"UserDetailsView?UserId={post.UserId}");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Perform the UI logic on the main thread
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    // Call a separate method to handle async operations
                    await HandleLogoutAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            });

            return true; // Return true to cancel the default back button action
        }

        private async Task HandleLogoutAsync()
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");

            if (answer)
            {
                App.CurrentUserEmail = null;
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        private void HandleException(Exception ex)
        {
            if (ex is TargetInvocationException tie && tie.InnerException != null)
            {
                ex = tie.InnerException; // Unwrap the inner exception
            }

            // Log or display the exception
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", ex.Message, "OK");
            });
        }
    }
}