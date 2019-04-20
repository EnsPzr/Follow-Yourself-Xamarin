using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using FollowYourSelfMobile.Models;
using FollowYourSelfMobile.ViewModel;
using Java.Security;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ActivityStatusPage : ContentPage
    {
        private ExPicker[] exPickers;
        private ExEntry[] exEntries;
        private Dictionary<int, string> exPickersValue = new Dictionary<int, string>();
        private ExStackLayout mainExStackLayout = new ExStackLayout()
        {
            Padding = new Thickness(0, 20, 0, 0)
        };
        private SQLiteManager _manager = new SQLiteManager();
        public ActivityStatusPage()
        {
            try
            {
                exPickersValue.Add(0, "Yapılmadı");
                exPickersValue.Add(1, "Yapıldı");
                var scrollView = new ScrollView();

                this.Title = "Günlük Aktivite İlerlemelerim";
                var mainGrid = new ExGrid() { Padding = new Thickness(15, 0, 15, 0) };
                scrollView.Content = mainExStackLayout;
                var activityIndicatorLabel = new ExLabel
                {
                    Text = "Günlük aktivite durumları getiriliyor..."
                };
                var activityIndicator = new ActivityIndicator()
                {
                    Color = Color.DarkRed
                };
                activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding("IsBusy", source: this));
                var activityIndicatorStackLayout = new ExStackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                activityIndicatorStackLayout.SetBinding(IsVisibleProperty, new Binding("IsBusy", source: this));
                activityIndicatorStackLayout.Children.Add(activityIndicatorLabel);
                activityIndicatorStackLayout.Children.Add(activityIndicator);

                mainGrid.Children.Add(scrollView);
                mainGrid.Children.Add(activityIndicatorStackLayout);

                Content = mainGrid;

                CreateDisplay(DateTime.Today);
            }
            catch (Exception e)
            {
                DisplayAlert("Hata", e.Message, "Tamam");
            }
        }

        public async void CreateDisplay(DateTime? date)
        {
            try
            {
                this.IsBusy = true;
                if (date > DateTime.Today)
                {
                    await DisplayAlert("Hata", "Bugünün tarihinden ileri bir tarih giremezsiniz.", "Tamam");
                    date=DateTime.Today;
                }
                var queryDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                mainExStackLayout.Children.Clear();
                var datePicker = new ExDatePicker()
                {
                    Date = queryDate,
                    Format = "dd/MM/yyyy",
                    VerticalOptions = LayoutOptions.Center
                };
                var dateLabel = new ExLabel()
                {
                    Text = "Günü Seçiniz:",
                    VerticalTextAlignment = TextAlignment.Center
                };
                mainExStackLayout.Children.Add(dateLabel);
                mainExStackLayout.Children.Add(datePicker);
                datePicker.DateSelected += async (sender, e) => { CreateDisplay(e.NewDate); };
                mainExStackLayout.Children.Add(datePicker);
                mainExStackLayout.IsVisible = false;
                var allActivities = _manager.GetAllActivity(date, true);
                foreach (var activity in allActivities)
                {
                    if (!_manager.IsThereActivityStatus(activity.ActivityId, queryDate))
                    {
                        _manager.InsertActivityStatus(activity.ActivityId, queryDate);
                    }
                }

                var allActivitiesNoFilter = _manager.GetAllActivity();
                var allActivityStatus = _manager.GetAllActivityStatuses(queryDate);
                var activityStatusList = allActivityStatus.Select(p => new ActivityStatusViewModel()
                {
                    ActivityName = allActivitiesNoFilter.FirstOrDefault(a => a.ActivityId == p.ActivityId) != null ?
                        allActivitiesNoFilter.FirstOrDefault(a => a.ActivityId == p.ActivityId).ActivityName
                        : "",
                    ActivityValue = p.ActivityValue,
                    ActivityStatusId = p.ActivityStatusId,
                    ActivityTypes = allActivitiesNoFilter.FirstOrDefault(a => a.ActivityId == p.ActivityId) != null ?
                        allActivitiesNoFilter.FirstOrDefault(a => a.ActivityId == p.ActivityId).ActivityTypes :
                        Enums.ActivityTypes.YapildiYapilmadi
                }).ToList();
                exPickers = new ExPicker[activityStatusList.Count(p => p.ActivityTypes == Enums.ActivityTypes.YapildiYapilmadi)];
                exEntries = new ExEntry[activityStatusList.Count(p => p.ActivityTypes != Enums.ActivityTypes.YapildiYapilmadi)];
                var exPickersCounter = 0;
                var exEntryCellsCounter = 0;
                var lst = new List<ViewCell>();
                foreach (var activity in activityStatusList)
                {
                    if (activity.ActivityTypes == Enums.ActivityTypes.Sayi)
                    {
                        exEntries[exEntryCellsCounter] = new ExEntry()
                        {
                            Keyboard = Keyboard.Numeric,
                            Placeholder = activity.ActivityValue.ToString(),
                            ReturnCommandParameter = activity.ActivityStatusId
                        };
                        mainExStackLayout.Children.Add(new ExLabel()
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Text = activity.ActivityName
                        });
                        mainExStackLayout.Children.Add(exEntries[exEntryCellsCounter]);
                        exEntryCellsCounter++;
                    }
                    else
                    {
                        exPickers[exPickersCounter] = new ExPicker()
                        {
                            ItemsSource = exPickersValue.Values.ToList(),
                            SelectedItem = exPickersValue.FirstOrDefault(p => p.Key == int.Parse(activity.ActivityValue.ToString())).Value,
                            Title = "Durum Seçiniz",
                            ClassId = activity.ActivityStatusId.ToString()
                        };
                        mainExStackLayout.Children.Add(new ExLabel()
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Text = activity.ActivityName
                        });
                        mainExStackLayout.Children.Add(exPickers[exPickersCounter]);
                        exPickersCounter++;
                    }
                }
                var saveButton = new Exbutton()
                {
                    Text = "Kaydet",
                    BackgroundColor = Color.GreenYellow,
                    VerticalOptions = LayoutOptions.Center
                };
                saveButton.Clicked += async (sender, e) =>
                {
                    foreach (var exEntry in exEntries)
                    {
                        if (exEntry.Text != null)
                        {
                            _manager.UpdateActivityStatus(int.Parse(exEntry.ReturnCommandParameter.ToString()), Convert.ToDouble(exEntry.Text));
                        }
                        //  DisplayAlert("Deneme", $"{Convert.ToDouble(exEntry.Text)} {exEntry.ReturnCommandParameter}", "a");
                    }

                    foreach (var exPicker in exPickers)
                    {
                        _manager.UpdateActivityStatus(int.Parse(exPicker.ClassId.ToString()),
                            exPickersValue.FirstOrDefault(p => p.Value == exPicker.SelectedItem).Key);
                        //DisplayAlert("Deneme", $"{exPicker.SelectedItem} {exPicker.ClassId}", "a");
                    }

                    await DisplayAlert("Başarı", "Kayıt işlemi başarı ile tamamlandı.", "Tamam");
                };
                mainExStackLayout.Children.Add(saveButton);
                Thread.Sleep(1000);
                mainExStackLayout.IsVisible = true;
                this.IsBusy = false;
            }
            catch (Exception e)
            {
                DisplayAlert("Hata", e.Message, "Tamam");
            }
        }
    }
}
