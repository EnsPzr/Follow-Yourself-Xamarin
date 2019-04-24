using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using FollowYourSelfMobile.Droid.Renderer;
using FollowYourSelfMobile.Ex;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExNumericEntry), typeof(ExNumericEntryRenderer))]
namespace FollowYourSelfMobile.Droid.Renderer
{
    public class ExNumericEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                this.Control.KeyListener = DigitsKeyListener.GetInstance("1234567890.");
            }
        }
    }
}