using System;
using System.Collections.Generic;
using System.Text;
using FollowYourSelfMobile.Ex;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class AppStartPage : ExContentPage
    {
        public AppStartPage()
        {
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

            mainStackLayout.Children.Add(appNameLabel);
            //mainStackLayout.Children.Add(new ExEntry()
            //{
            //    Text = "asdsadasdsadsa"
            //});
            Content = mainStackLayout;
        }
    }
}
