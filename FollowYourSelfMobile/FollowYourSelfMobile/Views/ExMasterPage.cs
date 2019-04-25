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
            Master = App.exNavigationPage;
            Detail = new NavigationPage(new ActivityStatusPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }

        public async void ActivityPageShow()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ActivityPage())
            {
                BarBackgroundColor = Color.DarkRed,
                // BackgroundColor = Color.LightGray
            };
        }

        public async void ActivityStatusPageShow()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ActivityStatusPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }

        public async void ReportPageShow()
        {
            this.IsPresented = false;
            this.Detail = new NavigationPage(new ReportPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
        }
    }
}
