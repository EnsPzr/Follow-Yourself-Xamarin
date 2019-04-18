using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FollowYourSelfMobile.Models
{
    public class Activity
    {
        [PrimaryKey, AutoIncrement]
        public int ActivityId { get; set; }

        public string ActivityName { get; set; }

        public DateTime? ActivityRegisterDate { get; set; }

        public bool IsActive { get; set; } = true;

        public Enums.ActivityTypes ActivityTypes { get; set; }
    }
}
