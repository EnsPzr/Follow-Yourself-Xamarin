using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using FollowYourSelfMobile.Models;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ActivityPage : ContentPage
    {
        SQLiteManager _manager = new SQLiteManager();
        public ActivityPage()
        {
            Title = "Tüm Aktivitelerim";
            var mainListView = new ExListView();
            var activityIndicatorLabel = new ExLabel() {
                Text = "Aktiviteleriniz getiriliyor...",
                TextColor = Color.Black
            };
            var activityIndicator = new ActivityIndicator()
            {
                Color = Color.DarkRed
            };
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding("IsBusy", source: this));
            var frame = new ExFrame()
            {
                CornerRadius = 10,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.LightGray
            };
            var activityIndicatorStackLayout = new ExStackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                //  Padding = new Thickness(20, 20, 20, 20),
            };
            activityIndicatorStackLayout.Children.Add(activityIndicatorLabel);
            activityIndicatorStackLayout.Children.Add(activityIndicator);
            frame.SetBinding(Frame.IsVisibleProperty, new Binding("IsBusy", source: this));
            frame.Content = activityIndicatorStackLayout;


            mainListView.ItemSelected += async (sender, e) =>
            {
                var selected = (Activity)e.SelectedItem;
                var listView = (ExListView)sender;
                if (selected != null)
                {
                    await Navigation.PushAsync(new ActivityAddAndUpdatePage(selected));
                    listView.SelectedItem = null;
                }
            };
            mainListView.ItemTemplate = App.ActivityPageDataTemplate;
            mainListView.SetBinding(ListView.ItemsSourceProperty, new Binding("activityList", source: typeof(App)));
            var mainGrid = new ExGrid();
            var bodyGrid = new ExGrid();
            bodyGrid.Children.Add(mainListView);
            mainGrid.Children.Add(bodyGrid);
            mainGrid.Children.Add(frame);
            Content = mainGrid;



            var newActivityToolbarItem = new ToolbarItem("Yeni", null, delegate { }, ToolbarItemOrder.Primary, 0);
            newActivityToolbarItem.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new ActivityAddAndUpdatePage(null), true);
            };
            this.ToolbarItems.Add(newActivityToolbarItem);
            var refreshListToolbarItem = new ToolbarItem("Yenile", null, delegate { }, ToolbarItemOrder.Primary, 1);
            this.ToolbarItems.Add(refreshListToolbarItem);
            refreshListToolbarItem.Clicked += async (sender, e) =>
            {
                Device.BeginInvokeOnMainThread((async () =>
                {
                    await ListData(mainListView);
                }));
            };

            Task.Factory.StartNew(async () => { await ListData(mainListView); });
        }

        private async Task ListData(ExListView ex)
        {
            // this.BackgroundColor = Color.LightGray;
            this.IsBusy = true;
            if (ex != null)
            {
                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                    {
                        ex.IsVisible = false;
                    }
                    ex.ItemsSource = null;
                    App.activityList = _manager.GetAllActivity();
                    ex.ItemsSource = App.activityList;
                    await Task.Delay(1000);
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                    {
                        ex.IsVisible = true;
                    }
                    await Task.Delay(1380);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            else
            {
                await DisplayAlert("hata", "a", "ok");
            }

            this.IsBusy = false;
            // this.BackgroundColor = Color.White;
        }
    }
}
