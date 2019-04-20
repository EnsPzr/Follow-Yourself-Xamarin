using System;
using System.Collections.Generic;
using System.Text;
using FollowYourSelfMobile.Models;

namespace FollowYourSelfMobile.ViewModel
{
    public class ActivityStatusViewModel
    {
        public int ActivityStatusId { get; set; }

        public double ActivityValue { get; set; }

        public string ActivityName { get; set; }

        public Enums.ActivityTypes ActivityTypes { get; set; }

    }
}
