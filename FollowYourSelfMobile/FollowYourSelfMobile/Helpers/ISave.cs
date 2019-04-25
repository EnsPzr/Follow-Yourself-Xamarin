using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FollowYourSelfMobile.Helpers
{
    public interface ISave
    {
        Task Save(string filename, string contentType, MemoryStream stream);
    }
}
