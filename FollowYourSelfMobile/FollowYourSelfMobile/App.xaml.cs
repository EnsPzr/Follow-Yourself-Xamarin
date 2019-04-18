using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content.Res;
using FollowYourSelfMobile.Components;
using FollowYourSelfMobile.Ex;
using FollowYourSelfMobile.Helpers;
using FollowYourSelfMobile.Models;
using FollowYourSelfMobile.Views;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FollowYourSelfMobile
{
    public partial class App : Application
    {
        SQLiteManager _manager = new SQLiteManager();
        public static string DbName = "FollowYourSelfDB.db3";
        public static ExMasterPage exMasterPage = new ExMasterPage();
        public static List<Activity> activityList;

        public static DataTemplate ActivityPageDataTemplate;
        public App()
        {
            InitializeComponent();
            ActivityPageDataTemplate = new DataTemplate(() =>
            {
                var textCell = new ExTextCell();
                textCell.SetBinding(TextCell.TextProperty, new Binding("ActivityName"));
                textCell.SetBinding(TextCell.DetailProperty, new Binding("IsActive",converter: new IsActiveConverter())); //converter eklenecek
                textCell.SetBinding(TextCell.CommandParameterProperty, new Binding("ActivityId"));
                textCell.SetBinding(TextCell.CommandParameterProperty, new Binding("ActivityTypes"));
                return textCell;
            });
            //MainPage = new AppStartPage();
            //Thread.Sleep(2000);
            //var activityList = new List<Activity>()
            //{
            //    new Activity()
            //    {
            //        ActivityName = "deneme 1",
            //        ActivityRegisterDate = DateTime.Now,
            //        ActivityTypes = Enums.ActivityTypes.TamSayi,
            //        IsActive = true
            //    },
            //    new Activity()
            //    {
            //        ActivityName = "deneme 2",
            //        ActivityRegisterDate = DateTime.Now,
            //        ActivityTypes = Enums.ActivityTypes.TamSayi,
            //        IsActive = false
            //    },
            //    new Activity()
            //    {
            //        ActivityName = "deneme 3",
            //        ActivityRegisterDate = DateTime.Now,
            //        ActivityTypes = Enums.ActivityTypes.TamSayi,
            //        IsActive = true
            //    }
            //};
            //foreach (var activity in activityList)
            //{
            //    //_manager.InsertActivity(activity);
            //}
            MainPage = (exMasterPage);

        }

        protected override void OnStart()
        {
            GetAllActivities();
            //MainPage = new AppStartPage();
            //Thread.Sleep(2000);
            //MainPage = exMasterPage;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void GetAllActivities()
        {
            activityList = _manager.GetAllActivity();
        }
    }
}
