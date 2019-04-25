using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Environment = System.Environment;

[assembly: Dependency(typeof(SaveFileAndroid))]

namespace FollowYourSelfMobile.Droid.Helpers
{
    public class SaveFileAndroid :ISave
    {
        public async Task Save(string filename, string contentType, MemoryStream stream)
        {
            var root = "";
            //Get the root path in android device.
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
            else
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            var myDir = new Java.IO.File(root + "/Kişisel Takip");
            var result=myDir.Mkdir();
            //Directory.CreateDirectory(myDir.Path);
            var file = new Java.IO.File(myDir, filename);

            //if (file.Exists()) file.Delete();

            var outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();
        }
    }
}