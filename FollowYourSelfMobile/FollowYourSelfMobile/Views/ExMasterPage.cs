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
            this.Detail = new NavigationPage(new ActivityPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
            this.IsPresented = false;
        }

        public async void ActivityStatusPageGoster()
        {
            this.Detail = new NavigationPage(new ActivityStatusPage())
            {
                BarBackgroundColor = Color.DarkRed
            };
            this.IsPresented = false;
        }
    }
}
