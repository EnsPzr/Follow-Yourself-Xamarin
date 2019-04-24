﻿using FollowYourSelfMobile.Ex;
using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Views
{
    public class ExNavigationPage : ContentPage
    {

        public ExNavigationPage()
        {
            this.Title = "Menü";

            #region title


            var titleLabel = new ExLabel
            {
                Text = "Menü",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                FontFamily = "Lobster-Regular"
            };

            //var titleGrid = new ExGrid()
            //{
            //    VerticalOptions = LayoutOptions.StartAndExpand,
            //    HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    BackgroundColor = Color.CornflowerBlue
            //};
            //titleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100)});
            //titleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)});

            //titleGrid.Children.Add(titleLabel, 0, 0);
            var titleStackLayout = new ExStackLayout()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 30, 0, 30),
                // BackgroundColor = Color.CornflowerBlue,
                BackgroundColor = Color.DarkRed,
                HeightRequest = 40
            };
            titleStackLayout.Children.Add(titleLabel);


            #endregion


            #region buttons

            var activityButton = new Exbutton
            {
                // BorderColor = Color.Black,
                TextColor = Color.White,
                BackgroundColor = Color.DeepSkyBlue,
                Text = "Aktivitelerim",
                CornerRadius = 10
            };
            activityButton.Clicked += async (sender, e) =>
            {
                App.exMasterPage.ActivityPageGoster();
            };
            var activityStatusButton = new Exbutton
            {
                TextColor = Color.White,
                BackgroundColor = Color.DeepSkyBlue,
                Text = "Günlük Takip",
                CornerRadius = 10
            };
            activityStatusButton.Clicked += async (sender, e) =>
            {
                //await new ExMasterPage().Detail.Navigation.PushAsync(new ActivityStatusPage());
                App.exMasterPage.ActivityStatusPageGoster();
            };

            var reportButton= new Exbutton()
            {
                TextColor = Color.White,
                BackgroundColor = Color.DeepSkyBlue,
                Text = "Rapor",
                CornerRadius = 10
            };
            reportButton.Clicked += async (sender, e) =>
            {
                App.exMasterPage.ReportPageGoster();
            };
            var buttonStackLayout = new ExStackLayout()
            {
                Padding = new Thickness(15, 15, 15, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                // BackgroundColor = Color.Black,
                Spacing = 7
            };
            buttonStackLayout.Children.Add(activityButton);
            buttonStackLayout.Children.Add(activityStatusButton);
            buttonStackLayout.Children.Add(reportButton);
            #endregion



            #region mainStackLayout

            var mainStackLayout = new ExStackLayout
            {
                //  BackgroundColor = Color.Red,
                Orientation = StackOrientation.Vertical
            };
            mainStackLayout.Children.Add(titleStackLayout);
            mainStackLayout.Children.Add(buttonStackLayout);

            #endregion


            Content = mainStackLayout;
        }
    }
}
