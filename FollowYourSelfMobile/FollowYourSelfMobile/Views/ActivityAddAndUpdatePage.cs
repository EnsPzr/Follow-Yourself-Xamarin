using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using FollowYourSelfMobile.Models;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ActivityAddAndUpdatePage : ExContentPage
    {
        private readonly SQLiteManager _manager = new SQLiteManager();
        private Dictionary<bool, string> isActiveStatusDictionary = new Dictionary<bool, string>();
        private Dictionary<Enums.ActivityTypes, string> activityTypesDictionary = new Dictionary<Enums.ActivityTypes, string>();
        public ActivityAddAndUpdatePage(Activity activity = null)
        {
            foreach (var activityType in (Enums.ActivityTypes[])Enum.GetValues(typeof(Enums.ActivityTypes)))
            {
                activityTypesDictionary.Add(activityType, Enums.GetEnumDescription(activityType));
            }
            var mainGrid = new ExGrid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(20, 0, 20, 0)
            };
            //mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            //mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            //mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            //mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            TableSection tableSection = null;
            TableRoot tableRoot = null;
            if (activity == null)
            {
                tableSection = new TableSection("Aktivite Ekle");
                tableRoot = new TableRoot("Aktivite Ekle");
            }
            else
            {
                tableSection = new TableSection("Aktivite Güncelle");
                tableRoot = new TableRoot("Aktivite Güncelle");
            }
            var activityNameEntry = new ExEntry()
            {
                Placeholder = "Aktivite Adı",
            };
            var activityNameEntryViewCell = new ViewCell();

            if (activity != null)
            {
                var activityNameLabel = new ExLabel()
                {
                    Text = "Aktivite Adı:",
                    TextColor = Color.Blue,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                activityNameEntryViewCell.View = new ExStackLayout()
                {
                    Children =
                    {
                        activityNameLabel,
                        activityNameEntry
                    }
                };
            }
            else
            {
                activityNameEntryViewCell.View = activityNameEntry;
            }
            tableSection.Add(activityNameEntryViewCell);



            var activityTypesPicker = new ExPicker()
            {
                Title = "Aktivite Türünü Seçiniz",
                TextColor = Color.Black,
                ItemsSource = activityTypesDictionary.Values.ToList()
            };
            var activityTypesPickerViewCell = new ViewCell();
            if (activity != null)
            {
                var activityTypesPickerLabel = new ExLabel()
                {
                    Text = "Aktivite Türü:",
                    TextColor = Color.Blue,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                activityTypesPickerViewCell.View = new ExStackLayout()
                {
                    Children =
                    {
                        activityTypesPickerLabel,
                        activityTypesPicker
                    }
                };
            }
            else
            {
                activityTypesPickerViewCell.View = activityTypesPicker;
            }
            tableSection.Add(activityTypesPickerViewCell);


            var activityIsActivePicker = new ExPicker()
            {
                Title = "Aktivite Aktiflik Durumu",
                TextColor = Color.Black,
                ItemsSource = isActiveStatusDictionary.Values.ToList()
            };
            if (activity != null)
            {
                var activityIsActivePickerLabel = new ExLabel()
                {
                    Text = "Aktivite Aktiflik Durumu:",
                    TextColor = Color.Blue,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                isActiveStatusDictionary.Add(false, "Aktif Değil");
                isActiveStatusDictionary.Add(true, "Aktif");
                activityIsActivePicker.ItemsSource = isActiveStatusDictionary.Values.ToList();
                activityIsActivePicker.SelectedItem = isActiveStatusDictionary
                    .FirstOrDefault(p => p.Key == activity.IsActive).Value;
                var activityIsActivePickerViewCell = new ViewCell
                {
                    View = new ExStackLayout()
                    {
                        Children =
                        {
                            activityIsActivePickerLabel,
                            activityIsActivePicker
                        }
                    }
                };
                tableSection.Add(activityIsActivePickerViewCell);
                activityNameEntry.Text = activity.ActivityName;
                activityTypesPicker.SelectedItem = Enums.GetEnumDescription(activity.ActivityTypes);
            }
            var saveButton = new Exbutton()
            {
                Text = "Kaydet",
                BackgroundColor = Color.FromHex("#449D44"),
                TextColor = Color.White,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var saveButtonViewCell = new ViewCell
            {
                View = saveButton
            };
            tableSection.Add(saveButtonViewCell);

            tableRoot.Add(tableSection);
            var tableView = new TableView(tableRoot)
            {
                Intent = TableIntent.Form,
                HasUnevenRows = true,
                RowHeight = -1,
                HeightRequest = -2
            };
            mainGrid.Children.Add(tableView);

            saveButton.Clicked += async (sender, e) =>
            {
                if (activityNameEntry.Text.Length != 0)
                {
                    if (activity == null)
                    {
                        if (!_manager.IsThereActivity(activityNameEntry.Text, null))
                        {
                            var newActivity = new Activity()
                            {
                                ActivityName = activityNameEntry.Text,
                                ActivityRegisterDate = DateTime.Today,
                                IsActive = true
                            };
                            newActivity.ActivityTypes =
                                activityTypesDictionary.FirstOrDefault(p =>
                                    p.Value == activityTypesPicker.SelectedItem.ToString()).Key;
                            if (_manager.InsertActivity(newActivity) > 0)
                            {
                                App.activityList = _manager.GetAllActivity();
                                await DisplayAlert("Başarı", "Aktivite ekleme işlemi başarılı bir şekilde sonuçlandı.",
                                    "Tamam");
                                await Navigation.PopAsync(true);
                            }
                            else
                            {
                                await DisplayAlert("Hata", "Ekleme işlemi sırasında hata oluştu.", "Tamam");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Hata", "Girilen aktivite adı zaten başka bir aktivite için kullanılıyor.", "Tamam");
                        }
                    }
                    else
                    {
                        if (!_manager.IsThereActivity(activityNameEntry.Text, activity.ActivityId))
                        {
                            var regulatedActivity = new Activity()
                            {
                                ActivityTypes = activityTypesDictionary.FirstOrDefault(p =>
                                    p.Value == activityTypesPicker.SelectedItem.ToString()).Key,
                                ActivityName = activityNameEntry.Text,
                                ActivityId = activity.ActivityId,
                                ActivityRegisterDate = activity.ActivityRegisterDate,
                                IsActive = isActiveStatusDictionary.FirstOrDefault(p => p.Value == activityIsActivePicker.SelectedItem.ToString()).Key
                            };

                            if (regulatedActivity.ActivityTypes == activity.ActivityTypes
                                && regulatedActivity.IsActive == activity.IsActive
                                && regulatedActivity.ActivityName.Equals(activity.ActivityName))
                            {
                                App.activityList = _manager.GetAllActivity();
                                await DisplayAlert("Başarı", "Aktivite güncelleme işlemi tamamlandı.", "Tamam");
                                await Navigation.PopAsync(true);
                            }
                            else
                            {
                                if (regulatedActivity.ActivityTypes != activity.ActivityTypes)
                                {
                                    var returned = await DisplayAlert("Dikkat",
                                        "Aktivitenin türünü değiştirdiniz. Devam ederseniz bu aktivitenin geçmişe yönelik tüm" +
                                        "verileri sıfırlanacaktır. Devam etmek istiyor musunuz?", "Devam", "İptal");
                                    if (returned==true)
                                    {
                                        var listActivityStatuses =
                                            _manager.GetAllActivityStatusesById(activity.ActivityId);
                                        foreach (var activityState in listActivityStatuses)
                                        {
                                            _manager.UpdateActivityStatus(activityState.ActivityStatusId,0);
                                        }

                                        if (_manager.UpdateActivity(regulatedActivity) > 0)
                                        {
                                            App.activityList = _manager.GetAllActivity();
                                            await DisplayAlert("Başarı", "Aktivite güncelleme işlemi tamamlandı.", "Tamam");
                                            await Navigation.PopAsync(true);
                                        }
                                        else
                                        {
                                            await DisplayAlert("Hata", "Güncelleme işlemi sırasında hata oluştu.", "Tamam");
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Uyarı",
                                            "Aktivite güncelleme işlemi kendi isteğiniz ile iptal edildi.", "Tamam");
                                    }
                                }
                                else
                                {
                                    if (_manager.UpdateActivity(regulatedActivity) > 0)
                                    {
                                        App.activityList = _manager.GetAllActivity();
                                        await DisplayAlert("Başarı", "Aktivite güncelleme işlemi tamamlandı.", "Tamam");
                                        await Navigation.PopAsync(true);
                                    }
                                    else
                                    {
                                        await DisplayAlert("Hata", "Güncelleme işlemi sırasında hata oluştu.", "Tamam");
                                    }
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Hata", "Girilen aktivite adı zaten başka bir aktivite için kullanılıyor.", "Tamam");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Hata", "Aktivite Adı girilmesi zorunludur.", "Tamam");
                }
            };



            Content = mainGrid;
        }
    }
}

/*
var activityTypesPickerView = new ViewCell
            {
                View = activityTypesPicker
            };
            //  tableSection.Add(activityNameEntryCell);
            //  tableSection.Add(activityTypesPickerView);
            var kaydetButtonView = new ViewCell
            {
                View = kaydetButton,
            };
            
                var activityIsActivePickerView = new ViewCell()
                {
                    View = activityIsActivePicker
                };
                
            //var activityNameEntryCell = new ExEntryCell()
            //{
            //    Placeholder = "Aktivite Adı"
            //};
 */
