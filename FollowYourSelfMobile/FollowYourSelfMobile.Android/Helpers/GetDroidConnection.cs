using System;
using System.Collections.Generic;
using System.IO;
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
using SQLite;
using Xamarin.Forms;
using Environment = System.Environment;


[assembly: Dependency(typeof(GetDroidConnection))]
namespace FollowYourSelfMobile.Droid.Helpers
{
    public class GetDroidConnection : ISQLiteConnection
    {
        public SQLiteConnection GetConnection()
        {
            var documentsDirectoryPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsDirectoryPath, App.DbName);
            //if (!File.Exists(path))
            //{
            //    using (var binaryReader = new BinaryReader(Android.App.Application.Context.Assets.Open(App.DbName)))
            //    {
            //        using (var binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create)))
            //        {
            //            byte[] buffer = new byte[2048];
            //            int length = 0;
            //            while ((length = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
            //            {
            //                binaryWriter.Write(buffer, 0, length);
            //            }
            //        }
            //    }
            //}
            var conn = new SQLiteConnection(path, false);
            return conn;
        }
    }
}