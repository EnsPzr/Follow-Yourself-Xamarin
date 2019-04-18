using System;
using System.Collections.Generic;
using System.Text;
using FollowYourSelfMobile.Ex;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ActivityStatusPage : ContentPage
    {
        public ActivityStatusPage()
        {
            this.Title = "Günlük Aktivite İlerlemelerim";
            Content = new ExLabel()
            {
                Text = "aktivite durumları sayfası"
            };
        }
    }
}
