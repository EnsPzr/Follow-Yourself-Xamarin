using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FollowYourSelfMobile.Ex;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class AppStartPage : ExContentPage
    {
        public AppStartPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            var mainAbsoluteLayout = new AbsoluteLayout();
            var mainStackLayout = new ExStackLayout()
            {
                BackgroundColor = Color.AliceBlue,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            var appNameLabel = new ExLabel()
            {
                Text = "Kişisel Takip",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.DarkRed
            };

            mainAbsoluteLayout.Children.Add(appNameLabel);
            //mainStackLayout.Children.Add(new ExEntry()
            //{
            //    Text = "asdsadasdsadsa"
            //});
            AbsoluteLayout.SetLayoutFlags(mainStackLayout, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(mainStackLayout, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            Content = mainAbsoluteLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Application.Current.MainPage = (App.exMasterPage);
        }
    }
}
