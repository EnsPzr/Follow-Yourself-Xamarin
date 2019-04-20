using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Activity> GetAllActivity(DateTime? startdate=null, bool? isActive=null)
        {
            var query = _sqLiteConnection.Table<Activity>().AsQueryable();
            if (startdate != null)
            {
                query = query.Where(p => p.ActivityRegisterDate <= startdate);
            }

            if (isActive != null)
            {
                query = query.Where(p => p.IsActive == isActive);
            }

            return query.ToList();
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

        public Activity GetActivity(int id)
        {
            return _sqLiteConnection.Table<Activity>().FirstOrDefault(p => p.ActivityId == id);
        }


        public List<ActivityStatus> GetAllActivityStatuses(DateTime? date)
        {
            return _sqLiteConnection.Table<ActivityStatus>().Where(p => p.Date == date).ToList();
        }

        public List<ActivityStatus> GetAllActivityStatusesById(int? activityId)
        {
            return _sqLiteConnection.Table<ActivityStatus>().Where(p => p.ActivityId == activityId).ToList();
        }

        //varsa return true
        public bool IsThereActivityStatus(int activityId, DateTime? date)
        {
            return _sqLiteConnection.Table<ActivityStatus>()
                       .FirstOrDefault(p => p.ActivityId == activityId && p.Date == date) != null;
        }

        public void InsertActivityStatus(int activityId, DateTime? date)
        {
            _sqLiteConnection.Insert(new ActivityStatus()
            {
                ActivityId = activityId,
                Date = date.Value,
                ActivityValue = 0
            });
        }

        public void UpdateActivityStatus(int activityStatusId, double value)
        {
            var activityStatus = _sqLiteConnection.Table<ActivityStatus>()
                .FirstOrDefault(p => p.ActivityStatusId == activityStatusId);
            if (activityStatus != null)
            {
                activityStatus.ActivityValue = value;
                _sqLiteConnection.Update(activityStatus);
            }
        }
        
    }
}
