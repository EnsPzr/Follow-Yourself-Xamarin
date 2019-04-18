using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FollowYourSelfMobile.Helpers
{
    public interface ISQLiteConnection
    {
        SQLiteConnection GetConnection();
    }
}
