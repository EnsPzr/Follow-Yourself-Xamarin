using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        private ExNumericEntry[] exEntries;
        private Dictionary<int, string> exPickersValue = new Dictionary<int, string>();
        private ExStackLayout mainExStackLayout = new ExStackLayout();
        private SQLiteManager _manager = new SQLiteManager();
        private ExDatePicker datePicker = new ExDatePicker();
        public ActivityStatusPage()
        {
            try
            {
                datePicker.DateSelected += async (sender, e) =>
                {
                    Device.BeginInvokeOnMainThread((async () => { await CreateDisplay(e.NewDate); }));
                };
                var mainGrid = new ExGrid();
                exPickersValue.Add(0, "Yapılmadı");
                exPickersValue.Add(1, "Yapıldı");
                var scrollView = new ScrollView();
                var bodyGrid = new ExGrid() { Padding = new Thickness(15, 15, 15, 15) };
                this.Title = "Günlük Aktivite İlerlemelerim";
                var activityIndicatorLabel = new ExLabel
                {
                    Text = "Günlük aktivite durumları getiriliyor...",
                    TextColor = Color.Black
                };
                var activityIndicatorExFrame = new ExFrame()
                {
                    CornerRadius = 10,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.LightGray
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
                activityIndicatorStackLayout.Children.Add(activityIndicatorLabel);
                activityIndicatorStackLayout.Children.Add(activityIndicator);
                activityIndicatorExFrame.Content = activityIndicatorStackLayout;
                activityIndicatorExFrame.SetBinding(Frame.IsVisibleProperty, new Binding("IsBusy", source: this));
                bodyGrid.Children.Add(mainExStackLayout);
                scrollView.Content = bodyGrid;
                mainGrid.Children.Add(activityIndicatorExFrame);
                mainGrid.Children.Add(scrollView);
                Content = mainGrid;
                Device.BeginInvokeOnMainThread((async () =>
                {
                    await CreateDisplay(DateTime.Today);
                }));
            }
            catch (Exception e)
            {
                DisplayAlert("Hata", e.Message, "Tamam");
            }
        }

        protected override void OnAppearing()
        {

        }
        public async Task CreateDisplay(DateTime? date)
        {
            try
            {
                this.IsBusy = true;
                if (date > DateTime.Today)
                {
                    await DisplayAlert("Hata", "Bugünün tarihinden ileri bir tarih giremezsiniz.", "Tamam");
                    date = DateTime.Today;
                }
                var queryDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                mainExStackLayout.IsVisible = false;
                mainExStackLayout.Children.Clear();
                this.datePicker.Date = queryDate;
                this.datePicker.Format = "dd/MM/yyyy";
                this.datePicker.VerticalOptions = LayoutOptions.Center;
                this.datePicker.MaximumDate = DateTime.Today;

                //var datePicker = new ExDatePicker()
                //{
                //    Date = queryDate,
                //    Format = "dd/MM/yyyy",
                //    VerticalOptions = LayoutOptions.Center,
                //    MaximumDate = DateTime.Today
                //};
                var dateLabel = new ExLabel()
                {
                    Text = "Günü Seçiniz:",
                    TextColor = Color.Blue,
                    Margin = new Thickness(10, 0, 0, 0),
                    // HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                mainExStackLayout.Children.Add(dateLabel);
                mainExStackLayout.Children.Add(datePicker);

                mainExStackLayout.Children.Add(datePicker);
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
                exEntries = new ExNumericEntry[activityStatusList.Count(p => p.ActivityTypes != Enums.ActivityTypes.YapildiYapilmadi)];
                var exPickersCounter = 0;
                var exEntryCellsCounter = 0;
                var lst = new List<ViewCell>();
                if (activityStatusList.Count > 0)
                {
                    var tabIndex = 0;
                    foreach (var activity in activityStatusList)
                    {
                        if (activity.ActivityTypes == Enums.ActivityTypes.Sayi)
                        {
                            exEntries[exEntryCellsCounter] = new ExNumericEntry()
                            {
                                Keyboard = Keyboard.Text,
                                Placeholder = activity.ActivityValue.ToString(),
                                ReturnCommandParameter = activity.ActivityStatusId,
                                TabIndex = tabIndex,
                                HorizontalTextAlignment = TextAlignment.Center,
                            };
                            mainExStackLayout.Children.Add(new ExLabel()
                            {
                                Text = activity.ActivityName,
                                TextColor = Color.Blue,
                                Margin = new Thickness(10, 0, 0, 0),
                                //  HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
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
                                ClassId = activity.ActivityStatusId.ToString(),
                                TabIndex = tabIndex
                            };
                            mainExStackLayout.Children.Add(new ExLabel()
                            {
                                Text = activity.ActivityName,
                                TextColor = Color.Blue,
                                Margin = new Thickness(10, 0, 0, 0),
                                // HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center
                            });
                            mainExStackLayout.Children.Add(exPickers[exPickersCounter]);
                            exPickersCounter++;
                        }

                        tabIndex++;
                    }
                }
                var saveButton = new Exbutton()
                {
                    Text = "Kaydet",
                    BackgroundColor = Color.FromHex("#449D44"),
                    TextColor = Color.White,
                    CornerRadius = 10,
                    VerticalOptions = LayoutOptions.Center
                };
                saveButton.Clicked += async (sender, e) =>
                {
                    foreach (var exEntry in exEntries)
                    {
                        if (exEntry.Text != null)
                        {
                            double value;
                            if (double.TryParse(exEntry.Text, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out value))
                            {
                                _manager.UpdateActivityStatus(int.Parse(exEntry.ReturnCommandParameter.ToString()), value);
                            }
                            
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
                await Task.Delay(1000);
                mainExStackLayout.IsVisible = true;
                await Task.Delay(1380);
                this.IsBusy = false;
            }
            catch (Exception e)
            {
                DisplayAlert("Hata", e.Message, "Tamam");
            }
        }

    }
}
