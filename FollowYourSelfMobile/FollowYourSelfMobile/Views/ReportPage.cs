using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using FollowYourSelfMobile.Models;
using OfficeOpenXml;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using Permission = Android.Content.PM.Permission;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;

namespace FollowYourSelfMobile.Views
{
    public class ReportPage : ContentPage
    {
        private ExGrid mainGrid = new ExGrid();
        private ExGrid bodyGrid = new ExGrid()
        {
            Padding = new Thickness(15, 15, 15, 15),
            ColumnSpacing = 0,
            RowSpacing = 5
        };
        private Dictionary<Enums.ActivityTypes, string> activityTypesDictionary = new Dictionary<Enums.ActivityTypes, string>();
        private Exbutton backupButton = new Exbutton()
        {
            Text = "Yedek Al",
            BackgroundColor = Color.FromHex("#449D44"),
            TextColor = Color.White,
            CornerRadius = 10,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(0, 7, 0, 0)
        };

        private Exbutton restoreFromBackupButton = new Exbutton()
        {
            Text = "Yedekten Geri Yükle",
            BackgroundColor = Color.FromHex("#449D44"),
            TextColor = Color.White,
            CornerRadius = 10,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(0, 7, 0, 0)
        };
        private ExLabel activityIndicatorLabel = new ExLabel()
        {
            Text = "Aktiviteleriniz getiriliyor...",
            TextColor = Color.Black
        };
        private SQLiteManager _manager = new SQLiteManager();
        public ReportPage()
        {
            this.Title = "Rapor";
            foreach (var activityType in (Enums.ActivityTypes[])Enum.GetValues(typeof(Enums.ActivityTypes)))
            {
                activityTypesDictionary.Add(activityType, Enums.GetEnumDescription(activityType));
            }
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

            backupButton.Clicked += async (sender, e) =>
            {

                Device.BeginInvokeOnMainThread((async () =>
                {
                    backupButton.IsEnabled = false;
                    restoreFromBackupButton.IsEnabled = false;
                    //  App.exNavigationPage.AllButtonDisabled();
                    await CreateBackup();
                    backupButton.IsEnabled = true;
                    restoreFromBackupButton.IsEnabled = true;
                    //  App.exNavigationPage.AllButtonEnable();
                }));
            };
            restoreFromBackupButton.Clicked += async (sender, e) =>
            {
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                if (storageStatus != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
                    storageStatus = results[Plugin.Permissions.Abstractions.Permission.Storage];
                }

                if (storageStatus == PermissionStatus.Granted)
                {
                    var file = await CrossFilePicker.Current.PickFile(new string[]
                    {
                        ".pzr",
                        ".xlsx"
                    });
                    Device.BeginInvokeOnMainThread((async () =>
                    {
                        backupButton.IsEnabled = false;
                        restoreFromBackupButton.IsEnabled = false;
                        //   App.exNavigationPage.AllButtonDisabled();
                        await RestoreFromBackup(file);
                        backupButton.IsEnabled = true;
                        restoreFromBackupButton.IsEnabled = true;
                        // await CreateDisplay();
                        //  App.exNavigationPage.AllButtonEnable();
                    }));
                }
                else
                {
                    await DisplayAlert("Hata", "Dosya erişimine izin vermeniz gerekmektedir.", "Tamam");
                }
            };
            scrollView.Content = bodyGrid;
            mainGrid.Children.Add(frame);
            mainGrid.Children.Add(scrollView);
            Content = mainGrid;
            Device.BeginInvokeOnMainThread((async () => { await CreateDisplay(); }));
        }


        private async Task CreateBackup()
        {
            activityIndicatorLabel.Text = "Yedek Oluşturuluyor...";
            this.IsBusy = true;
            await Task.Delay(1000);
            try
            {
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                if (storageStatus != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
                    storageStatus = results[Plugin.Permissions.Abstractions.Permission.Storage];
                }
                if (storageStatus == PermissionStatus.Granted)
                {
                    //var path = DependencyService.Get<IExportFilesToLocation>().GetFileLocation() + "kisiselTakipYedek" + DateTime.Now.ToString("ddMMyyHmmss") + ".xlsx";

                    using (var excel = new ExcelPackage())
                    {
                        excel.Workbook.Worksheets.Add("AllActivities");
                        var excelWorksheet = excel.Workbook.Worksheets["AllActivities"];
                        excelWorksheet.Cells["A1"].Value = "ActivityId";
                        excelWorksheet.Cells["B1"].Value = "ActivityName";
                        excelWorksheet.Cells["C1"].Value = "ActivityRegisterDate";
                        excelWorksheet.Cells["D1"].Value = "IsActive";
                        excelWorksheet.Cells["E1"].Value = "ActivityTypes";
                        var allActivities = _manager.GetAllActivity();
                        var rowIndex = 2;
                        foreach (var activity in allActivities)
                        {
                            excelWorksheet.Cells[rowIndex, 1].Value = activity.ActivityId;
                            excelWorksheet.Cells[rowIndex, 2].Value = activity.ActivityName;
                            excelWorksheet.Cells[rowIndex, 3].Value = activity.ActivityRegisterDate.ToString();
                            excelWorksheet.Cells[rowIndex, 4].Value = activity.IsActive.ToString();
                            excelWorksheet.Cells[rowIndex, 5].Value = activity.ActivityTypes;
                            rowIndex++;
                        }
                        excel.Workbook.Worksheets.Add("AllActivityStatus");
                        excelWorksheet = excel.Workbook.Worksheets["AllActivityStatus"];
                        excelWorksheet.Cells["A1"].Value = "ActivityStatusId";
                        excelWorksheet.Cells["B1"].Value = "ActivityId";
                        excelWorksheet.Cells["C1"].Value = "ActivityValue";
                        excelWorksheet.Cells["D1"].Value = "Date";
                        rowIndex = 2;
                        foreach (var activity in allActivities)
                        {
                            var allActivityStatus = _manager.GetAllActivityStatusesById(activity.ActivityId);
                            foreach (var activityStatus in allActivityStatus)
                            {
                                excelWorksheet.Cells[rowIndex, 1].Value = activityStatus.ActivityStatusId;
                                excelWorksheet.Cells[rowIndex, 2].Value = activityStatus.ActivityId;
                                excelWorksheet.Cells[rowIndex, 3].Value = activityStatus.ActivityValue.ToString();
                                excelWorksheet.Cells[rowIndex, 4].Value = activityStatus.Date.ToString();
                                rowIndex++;
                            }
                        }
                        MemoryStream stream = new MemoryStream(excel.GetAsByteArray("YawArkadasBuNeKadarGizliBirSifre.1!!!!asdasasd"));
                        await DependencyService.Get<ISave>().Save("KisiselTakipBackup_" + DateTime.Now.ToString("ddMMyyHmmss") + ".xlsx", "application/msexcel", stream);
                    }
                    await DisplayAlert("Başarılı", "Yedek kişisel takip klasörüne başarı ile oluşturuldu. Telefonunuzu sıfırlamadan önce yedeği saklarsanız, sıfırladıktan sonra" +
                                                   " kaldığınız yerden devam edebilirsiniz.", "Tamam");
                }
                else
                {
                    await DisplayAlert("Hata", "Dosya erişimine izin vermediniz.", "Tamam");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await Task.Delay(1380);
            this.IsBusy = false;
        }
        private async Task CreateDisplay()
        {
            activityIndicatorLabel.Text = "Rapor Oluşturuluyor...";
            //bodyGrid.Children.Clear();
            backupButton.IsEnabled = false;
            restoreFromBackupButton.IsEnabled = false;
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


            bodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            bodyGrid.Children.Add(backupButton, 0, rowNumber);
            Grid.SetColumnSpan(backupButton, 5);
            //  bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            rowNumber++;
            await Task.Delay(300);
            bodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            bodyGrid.Children.Add(restoreFromBackupButton, 0, rowNumber);
            Grid.SetColumnSpan(restoreFromBackupButton, 5);
            // bodyGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            await Task.Delay(1380);
            backupButton.IsEnabled = true;
            restoreFromBackupButton.IsEnabled = true;
            this.IsBusy = false;
        }

        private async Task RestoreFromBackup(FileData file)
        {
            if (file != null)
            {
                activityIndicatorLabel.Text = "Yedekten Geri Yükleniyor...";
                this.IsBusy = true;
                await Task.Delay(1000);
                var lastPoint = file.FileName.LastIndexOf(".");
                if (lastPoint != -1)
                {
                    var fileExtention = file.FileName.Substring(lastPoint);
                    if (fileExtention.Equals(".pzr") || fileExtention.Equals(".xlsx"))
                    {
                        //File.Move(file.FilePath, file.FilePath.Substring(0, lastPoint) + ".xlsx");
                        var template = new FileInfo(file.FilePath);
                        ExcelPackage pck = null;
                        try
                        {
                            pck = new ExcelPackage(template, "YawArkadasBuNeKadarGizliBirSifre.1!!!!asdasasd");
                        }
                        catch (Exception e)
                        {
                            // ignored
                        }

                        if (pck != null)
                        {
                            var ws = pck.Workbook.Worksheets[0];
                            var ws2 = pck.Workbook.Worksheets[1];
                            if (ws != null && ws2 != null)
                            {
                                var allActivities = new List<Activity>();
                                var allActivityStatus = new List<ActivityStatus>();
                                if (ws.Cells["A1"].Text.Length != 0 && ws.Cells["B1"].Text.Length != 0 &&
                                    ws.Cells["C1"].Text.Length != 0 && ws.Cells["D1"].Text.Length != 0 && ws.Cells["E1"].Text.Length != 0
                                    && ws2.Cells["A1"].Text.Length != 0 && ws2.Cells["B1"].Text.Length != 0
                                    && ws2.Cells["C1"].Text.Length != 0 && ws2.Cells["D1"].Text.Length != 0)
                                {
                                    if (ws.Cells["A1"].Value.Equals("ActivityId") &&
                                        ws.Cells["B1"].Value.Equals("ActivityName") &&
                                        ws.Cells["C1"].Value.Equals("ActivityRegisterDate") &&
                                        ws.Cells["D1"].Value.Equals("IsActive") &&
                                        ws.Cells["E1"].Value.Equals("ActivityTypes") &&
                                        ws2.Cells["A1"].Value.Equals("ActivityStatusId") &&
                                        ws2.Cells["B1"].Value.Equals("ActivityId") &&
                                        ws2.Cells["C1"].Value.Equals("ActivityValue") &&
                                        ws2.Cells["D1"].Value.Equals("Date"))
                                    {
                                        var userResult = false;
                                        var dataError = false;
                                        for (var i = 2; i < ws.Dimension.End.Row + 1; i++)
                                        {
                                            var activityId = 0;
                                            var activityRegisterDate = new DateTime();
                                            var isActive = false;
                                            if (int.TryParse(ws.Cells[i, 1].Value.ToString(), out activityId)
                                                && DateTime.TryParse(ws.Cells[i, 3].Value.ToString(),
                                                    out activityRegisterDate)
                                                && bool.TryParse(ws.Cells[i, 4].Value.ToString(), out isActive)
                                                && activityTypesDictionary.FirstOrDefault(p =>
                                                    p.Key == (Enums.ActivityTypes)Enum.Parse(
                                                        typeof(Enums.ActivityTypes),
                                                        ws.Cells[i, 5].Value.ToString())).Value != null)
                                            {
                                                allActivities.Add(new Activity
                                                {
                                                    ActivityName = ws.Cells[i, 2].Value.ToString(),
                                                    ActivityRegisterDate = new DateTime(activityRegisterDate.Year, activityRegisterDate.Month, activityRegisterDate.Day),
                                                    ActivityTypes = activityTypesDictionary.FirstOrDefault(p =>
                                                        p.Key == (Enums.ActivityTypes)Enum.Parse(
                                                            typeof(Enums.ActivityTypes),
                                                            ws.Cells[i, 5].Value.ToString())).Key,
                                                    IsActive = isActive,
                                                    ActivityId = activityId
                                                });
                                            }
                                            else
                                            {
                                                this.IsBusy = false;
                                                dataError = true;
                                                userResult = await DisplayAlert("Hata",
                                                    "Veri bütünlüğünde hatalar mevcut. Sadece kurtarılabilen aktivitelerin yüklenmesini ister misiniz?",
                                                    "Evet", "Hayır");
                                                if (userResult)
                                                {
                                                    this.IsBusy = true;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (dataError && userResult == false)
                                        {
                                            this.IsBusy = false;
                                            await DisplayAlert("Hata",
                                                "Geri yükleme işlemi kullanıcı talebiyle iptal edildi.", "Tamam");
                                        }
                                        else
                                        {
                                            this.IsBusy = true;
                                            for (var i = 2; i <= ws2.Dimension.End.Row; i++)
                                            {
                                                var activityStatusId = 0;
                                                var activityId = 0;
                                                var activityValue = 0.0;
                                                var date = DateTime.Today;
                                                if (int.TryParse(ws2.Cells[i, 1].Value.ToString(),
                                                        out activityStatusId) &&
                                                    int.TryParse(ws2.Cells[i, 2].Value.ToString(), out activityId) &&
                                                    double.TryParse(ws2.Cells[i, 3].Value.ToString(),
                                                        out activityValue) &&
                                                    DateTime.TryParse(ws2.Cells[i, 4].Value.ToString(), out date))
                                                {
                                                    allActivityStatus.Add(new ActivityStatus
                                                    {
                                                        ActivityId = activityId,
                                                        Date = new DateTime(date.Year, date.Month, date.Day),
                                                        ActivityValue = activityValue,
                                                        ActivityStatusId = activityStatusId
                                                    });
                                                }
                                            }
                                        }

                                        if ((dataError == false) || (dataError && userResult == true))
                                        {
                                            this.IsBusy = true;
                                            foreach (var activity in allActivities)
                                            {
                                                if (_manager.IsThereActivity(activity.ActivityName,
                                                    null))
                                                {
                                                    this.IsBusy = false;
                                                    var userUpdateResult = await DisplayAlert("Uyarı",
                                                        activity.ActivityName +
                                                        " ismindeki aktivite zaten sistemde eklidir." +
                                                        " Aktivite durumlarını güncellemek ister misiniz?", "Evet",
                                                        "Hayır");
                                                    this.IsBusy = true;
                                                    if (userUpdateResult)
                                                    {
                                                        var activityId = _manager.GetActivityId(activity.ActivityName);
                                                        activity.ActivityId = activityId;
                                                        _manager.UpdateActivity(activity);
                                                        foreach (var activityStatu in allActivityStatus.Where(p => p.ActivityId == activity.ActivityId))
                                                        {
                                                            if (_manager.IsThereActivityStatus(activityId,
                                                                activityStatu.Date))
                                                            {
                                                                var activityStatusId =
                                                                    _manager.GetActivityStatusId(activityId,
                                                                        activityStatu.Date);
                                                                _manager.UpdateActivityStatus(activityStatusId, activityStatu.ActivityValue);
                                                            }
                                                            else
                                                            {
                                                                _manager.InsertActivityStatus(activityId, activityStatu.Date);
                                                                var activityStatusId =
                                                                    _manager.GetActivityStatusId(activityId,
                                                                        activityStatu.Date);
                                                                _manager.UpdateActivityStatus(activityStatusId, activityStatu.ActivityValue);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    _manager.InsertActivity(new Activity
                                                    {
                                                        ActivityName = activity.ActivityName,
                                                        ActivityRegisterDate = activity.ActivityRegisterDate,
                                                        ActivityTypes = activity.ActivityTypes,
                                                        IsActive = activity.IsActive
                                                    });
                                                    var activityId = _manager.GetActivityId(activity.ActivityName);
                                                    foreach (var activityStatu in allActivityStatus.Where(p => p.ActivityId == activity.ActivityId))
                                                    {
                                                        _manager.InsertActivityStatus(activityId, activityStatu.Date);
                                                        var activityStatusId =
                                                            _manager.GetActivityStatusId(activityId,
                                                                activityStatu.Date);
                                                        _manager.UpdateActivityStatus(activityStatusId, activityStatu.ActivityValue);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        this.IsBusy = false;
                                        await DisplayAlert("Hata", "Dosya yapısında hata var. " +
                                                                   "Dilerseniz incelenmesi için enespazar4342@gmail.com adresine gönderebilirsiniz.", "Tamam");
                                    }
                                }
                                else
                                {
                                    this.IsBusy = false;
                                    await DisplayAlert("Hata", "Dosya yapısında hata var. " +
                                                               "Dilerseniz incelenmesi için enespazar4342@gmail.com adresine gönderebilirsiniz.", "Tamam");
                                }
                            }
                        }
                        else
                        {
                            this.IsBusy = false;
                            await DisplayAlert("Hata", "Dosya açılması sırasında hata oluştu.", "Tamam");
                        }
                    }
                    else
                    {
                        this.IsBusy = false;
                        await DisplayAlert("Hata", "Sadece excel dosyası seçilmelidir.", "Tamam");
                    }
                }
                else
                {
                    this.IsBusy = false;
                    await DisplayAlert("Hata", "Sadece excel dosyası seçilmelidir.", "Tamam");
                }
            }
            else
            {
                this.IsBusy = false;
                await DisplayAlert("Hata", "Lütfen Yedekleme Dosyasınız Seçiniz.", "Tamam");
            }
            await Task.Delay(1380);
            this.IsBusy = false;
        }
    }
}
