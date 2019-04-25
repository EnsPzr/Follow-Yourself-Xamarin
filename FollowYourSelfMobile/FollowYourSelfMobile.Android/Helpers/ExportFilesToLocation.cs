using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FollowYourSelfMobile.Droid.Helpers;
using FollowYourSelfMobile.Helpers;
using Java.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(ExportFilesToLocation))]
namespace FollowYourSelfMobile.Droid.Helpers
{
    public class ExportFilesToLocation : IExportFilesToLocation
    {
        public string GetFileLocation()
        {
            string root = null;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
            else
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            File myDir = new File(root + "/Kişisel Takip");
            if (!myDir.Exists())
                myDir.Mkdir();

            return root + "/Kişisel Takip/";
        }
    }
}