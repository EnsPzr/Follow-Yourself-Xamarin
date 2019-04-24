using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FollowYourSelfMobile.Droid
{
    [Activity(Label = "Kişisel Takip", Icon = "@drawable/apicon",Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            base.SetContentView(Resource.Layout.activity_splash);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(SimulateStartup);
            startupWork.Start();
        }
        async void SimulateStartup()
        {
            await Task.Delay(2000); // Simulate a bit of startup work.
            var mainIntent = new Intent(Application.Context, typeof(MainActivity));


            //if (Intent.Extras != null)
            //{
            //    if (Intent.Extras.KeySet().Count > 4)
            //    {
            //        foreach (var key in Intent.Extras.KeySet())
            //        {
            //            var value = Intent.Extras.GetString(key);
            //            mainIntent.PutExtra(key, value);
            //        }
            //    }

            //}


            StartActivity(mainIntent);

        }
    }
}