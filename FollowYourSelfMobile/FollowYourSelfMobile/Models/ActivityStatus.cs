using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FollowYourSelfMobile.Models
{
    public class ActivityStatus
    {
        [PrimaryKey, AutoIncrement]
        public int ActivityStatusId { get; set; }

        public int ActivityId { get; set; }

        public double ActivityValue { get; set; }

        public DateTime Date { get; set; }
    }
}
