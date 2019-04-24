using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ReportPage : ContentPage
    {
        private ExGrid mainGrid = new ExGrid()
        {
            Padding = new Thickness(15, 15, 15, 15)
        };
        private ExGrid bodyGrid = new ExGrid()
        {
            ColumnSpacing = 0,
            RowSpacing = 5
        };
        private SQLiteManager _manager = new SQLiteManager();
        public ReportPage()
        {
            this.Title = "Rapor";
            var activityIndicatorLabel = new ExLabel()
            {
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
            var scrollView = new ScrollView();



            var activityNameLabel = new ExLabel()
            {
                Text = "Aktivite Adı",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            var totalNumberOfActivity = new ExLabel()
            {
                Text = "Toplam Yapılma",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            var numberOfActivitiesThisYear = new ExLabel()
            {
                Text = "Bu Yıl",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            var numberOfActivitiesThisMonth = new ExLabel()
            {
                Text = "Bu Ay",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            var numberOfActivitiesThisWeek = new ExLabel()
            {
                Text = "Bu Hafta",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            bodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            bodyGrid.Children.Add(activityNameLabel, 0, 0);
            bodyGrid.Children.Add(totalNumberOfActivity, 1, 0);
            bodyGrid.Children.Add(numberOfActivitiesThisYear, 2, 0);
            bodyGrid.Children.Add(numberOfActivitiesThisMonth, 3, 0);
            bodyGrid.Children.Add(numberOfActivitiesThisWeek, 4, 0);



            scrollView.Content = bodyGrid;
            mainGrid.Children.Add(frame);
            mainGrid.Children.Add(scrollView);
            Content = mainGrid;

            Device.BeginInvokeOnMainThread((async () => { await CreateDisplay(); }));
        }

        public async Task CreateDisplay()
        {
            this.IsBusy = true;
            await Task.Delay(1000);
            var allActivities = _manager.GetAllActivity().OrderBy(p => p.ActivityName);
            var rowNumber = 1;
            var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            foreach (var activity in allActivities)
            {
                bodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                var allActivityStatus = _manager.GetAllActivityStatusesById(activity.ActivityId);
                var activityNameLabel = new ExLabel()
                {
                    Text = activity.ActivityName,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                var totalNumberOfActivityLabel = new ExLabel()
                {
                    Text = allActivityStatus.Sum(p => p.ActivityValue).ToString(),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                var numberOfActivitiesThisYearLabel = new ExLabel()
                {
                    Text = allActivityStatus.Where(p => p.Date.Year == DateTime.Today.Year).Sum(p => p.ActivityValue).ToString(),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                var numberOfActivitiesThisMonthLabel = new ExLabel()
                {
                    Text = allActivityStatus.Where(p => p.Date.Year == DateTime.Today.Year && p.Date.Month == DateTime.Today.Month).Sum(p => p.ActivityValue).ToString(),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                var numberOfActivitiesThisWeekLabel = new ExLabel()
                {
                    Text = allActivityStatus.Where(p => p.Date > DateTime.Today.AddDays(-dayOfWeek)).Sum(p => p.ActivityValue).ToString(),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                if (rowNumber % 2 == 1)
                {
                    activityNameLabel.BackgroundColor = Color.LightGray;
                    totalNumberOfActivityLabel.BackgroundColor = Color.LightGray;
                    numberOfActivitiesThisYearLabel.BackgroundColor = Color.LightGray;
                    numberOfActivitiesThisMonthLabel.BackgroundColor = Color.LightGray;
                    numberOfActivitiesThisWeekLabel.BackgroundColor = Color.LightGray;
                }
                bodyGrid.Children.Add(activityNameLabel, 0, rowNumber);
                bodyGrid.Children.Add(totalNumberOfActivityLabel, 1, rowNumber);
                bodyGrid.Children.Add(numberOfActivitiesThisYearLabel, 2, rowNumber);
                bodyGrid.Children.Add(numberOfActivitiesThisMonthLabel, 3, rowNumber);
                bodyGrid.Children.Add(numberOfActivitiesThisWeekLabel, 4, rowNumber);
                rowNumber++;
                await Task.Delay(300);
            }

            var backupButton = new Exbutton()
            {
                Text = "Yedek Al",
                BackgroundColor = Color.FromHex("#449D44"),
                TextColor = Color.White,
                CornerRadius = 10,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 7, 0, 0)
            };
            //bodyGrid.Children.Add(backupButton, 0, rowNumber);
            //bodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            //bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            //Grid.SetColumnSpan(backupButton, 5);
            //backupButton.Clicked += async (sender, e) => { await DisplayAlert("t", "t", "a"); };
            await Task.Delay(1380);
            this.IsBusy = false;
        }
    }
}
