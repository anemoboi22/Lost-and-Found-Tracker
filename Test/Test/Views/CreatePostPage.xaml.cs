using System;
using System.Collections.Generic;
using System.Windows.Input;
using Test.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePostPage : ContentPage
    {
        public ICommand GoBackButton { get; private set; }
        public ICommand PostCommand { get; private set; }
        private User _user;
        private List<string> _imagePaths;

        public CreatePostPage()
        {
            InitializeComponent();
            GoBackButton = new Command(OnGoBackCommandExecuted);
            PostCommand = new Command(OnPostCommandExecuted);
            _imagePaths = new List<string>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Assuming you have a method to get the currently logged-in user
            _user = await App.Database.GetUserAsync(App.CurrentUserEmail);
            if (_user != null)
            {
                NameText.Text = _user.Fullname;
                IDText.Text = _user.StudentId;
            }
        }

        private async void OnGoBackCommandExecuted()
        {
            // Clear the fields and images
            ClearFieldsAndImages();
            // Navigate to LoginPage using Shell navigation
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async void OnPostCommandExecuted()
        {
            // Logic to handle the post action
            string postContent = PostEditor.Text;
            if (string.IsNullOrWhiteSpace(postContent) || _imagePaths.Count == 0)
            {
                await DisplayAlert("Error", "Post content or images cannot be empty.", "OK");
                return;
            }

            string formattedDateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");

            var post = new Post
            {
                Content = postContent,
                ImagePaths = _imagePaths,
                UserId = _user.Id,
                CreationDate = formattedDateTime
            };

            await App.Database.SavePostAsync(post);

            await DisplayAlert("Success", "Post has been created successfully.", "OK");

            // Clear the fields and images
            ClearFieldsAndImages();

            // Navigate back to the previous page or dashboard
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async void OnPhotoTapped(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var action = await DisplayActionSheet("Add Photo", "Cancel", null, "Take Photo", "Choose from Gallery");
            if (action == "Take Photo")
            {
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        SaveToAlbum = true,
                        Directory = "Sample",
                        Name = "test.jpg"
                    });

                    if (photo != null)
                    {
                        _imagePaths.Add(photo.Path);
                        AddImageToStack(photo.Path);
                    }
                }
                else
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                }
            }
            else if (action == "Choose from Gallery")
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var photo = await CrossMedia.Current.PickPhotoAsync();

                    if (photo != null)
                    {
                        _imagePaths.Add(photo.Path);
                        AddImageToStack(photo.Path);
                    }
                }
            }
        }

        private void AddImageToStack(string imagePath)
        {
            var absoluteLayout = new AbsoluteLayout();

            var image = new Image
            {
                Source = ImageSource.FromFile(imagePath),
                HeightRequest = 200,
                Aspect = Aspect.AspectFill
            };

            var removeButton = new ImageButton
            {
                Source = "remove.png", // Set the Source to your remove.png image
                BackgroundColor = Color.Transparent,
                WidthRequest = 8,
                HeightRequest = 8
            };

            removeButton.Clicked += (sender, args) =>
            {
                ImagesStack.Children.Remove(absoluteLayout);
                _imagePaths.Remove(imagePath);  // Remove from the list as well
            };

            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);

            AbsoluteLayout.SetLayoutBounds(removeButton, new Rectangle(1, 0, 0.2, 0.2));
            AbsoluteLayout.SetLayoutFlags(removeButton, AbsoluteLayoutFlags.All);

            absoluteLayout.Children.Add(image);
            absoluteLayout.Children.Add(removeButton);

            ImagesStack.Children.Add(absoluteLayout);
        }

        private void ClearFieldsAndImages()
        {
            PostEditor.Text = string.Empty;
            _imagePaths.Clear();
            ImagesStack.Children.Clear();
        }

        protected override bool OnBackButtonPressed()
        {
            // Override the back button press to navigate to the DashboardPage
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Clear the fields and images
                ClearFieldsAndImages();
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            });

            // Return true to indicate the back button press is handled
            return true;
        }
    }
}
