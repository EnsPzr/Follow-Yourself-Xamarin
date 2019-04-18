using System;
using System.Collections.Generic;
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
            var activityIndicatorLabel = new ExLabel() { Text = "Aktiviteleriniz getiriliyor..." };
            var activityIndicator = new ActivityIndicator();
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding("IsBusy", source: this));
            var activityIndicatorStackLayout = new ExStackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            activityIndicatorStackLayout.Children.Add(activityIndicatorLabel);
            activityIndicatorStackLayout.Children.Add(activityIndicator);
            activityIndicatorStackLayout.SetBinding(Grid.IsVisibleProperty, new Binding("IsBusy", source: this));



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
            var mainGrid = new ExGrid();
            mainGrid.Children.Add(mainListView);
            mainGrid.Children.Add(activityIndicatorStackLayout);
            Content = mainGrid;



            var newActivityToolbarItem = new ToolbarItem("Yeni", null, delegate { }, ToolbarItemOrder.Primary, 0);
            newActivityToolbarItem.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new ActivityAddAndUpdatePage(null), true);
            };
            this.ToolbarItems.Add(newActivityToolbarItem);
            var refreshListToolbarItem = new ToolbarItem("Yenile", null, delegate { }, ToolbarItemOrder.Primary, 1);
            refreshListToolbarItem.Clicked += async (sender, e) =>
            {
                await Task.Run(async () => { ListData(mainListView); });
            };
            this.ToolbarItems.Add(refreshListToolbarItem);



            Task.Run(async () => { ListData(mainListView); });
        }

        private async void ListData(ExListView ex)
        {
            this.IsBusy = true;
            if (ex != null)
            {
                ex.IsVisible = false;
                ex.ItemsSource = null;
                App.activityList = _manager.GetAllActivity();
                ex.ItemsSource = App.activityList;
                Thread.Sleep(1000);
                ex.IsVisible = true;
            }
            else
            {
                await DisplayAlert("hata", "a", "ok");
            }
            this.IsBusy = false;
        }
    }
}
