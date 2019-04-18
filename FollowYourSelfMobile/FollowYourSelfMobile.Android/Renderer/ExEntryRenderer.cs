using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using FollowYourSelfMobile.Droid.Renderer;
using FollowYourSelfMobile.Ex;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExEntry), typeof(ExEntryRenderer))]
namespace FollowYourSelfMobile.Droid.Renderer
{
    public class ExEntryRenderer : EntryRenderer
    {
        //protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        //{
        //    base.OnElementChanged(e);

        //    if (Control != null)
        //    {
        //        GradientDrawable gd = new GradientDrawable();
        //        gd.SetColor(global::Android.Graphics.Color.Transparent);
        //        this.Control.SetBackgroundDrawable(gd);
        //        this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
        //        Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.Black));
        //    }
        //}
    }
}