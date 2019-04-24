using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ExMasterPage : MasterDetailPage
    {
        public ExMasterPage()
        {
            Master = new ExNavigationPage();
            Detail = new NavigationPage(new ActivityStatusPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }

        public async void ActivityPageGoster()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ActivityPage())
            {
                BarBackgroundColor = Color.DarkRed,
                // BackgroundColor = Color.LightGray
            };
        }

        public async void ActivityStatusPageGoster()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ActivityStatusPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }

        public async void ReportPageGoster()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ReportPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }
    }
}
