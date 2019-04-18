using System;
using System.Collections.Generic;
using System.Text;
using FollowYourSelfMobile.Models;
using SQLite;
using Xamarin.Forms;

namespace FollowYourSelfMobile.Helpers
{
    public class SQLiteManager
    {
        private SQLiteConnection _sqLiteConnection;

        public SQLiteManager()
        {
            _sqLiteConnection = DependencyService.Get<ISQLiteConnection>().GetConnection();
            _sqLiteConnection.CreateTable<Activity>();
            _sqLiteConnection.CreateTable<ActivityStatus>();
        }

        public List<Activity> GetAllActivity()
        {
            return _sqLiteConnection.Table<Activity>().ToList();
        }

        public int InsertActivity(Activity newActivity)
        {
            return _sqLiteConnection.Insert(newActivity);
        }

        public int UpdateActivity(Activity regulatedActivity)
        {
            return _sqLiteConnection.Update(regulatedActivity);
        }

        public bool IsThereActivity(string activityName, int? activityId)
        {
            if (activityId == null)
            {
                var activity = _sqLiteConnection.Table<Activity>().FirstOrDefault(p => p.ActivityName.Equals(activityName));
                return activity != null;
            }
            else
            {
                var activity = _sqLiteConnection.Table<Activity>().FirstOrDefault(p => p.ActivityId != activityId
                                                                                       && p.ActivityName.Equals(activityName));
                return activity != null;
            }
        }
    }
}
