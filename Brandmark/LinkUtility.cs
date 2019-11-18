using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brandmark
{
    class LinkUtility
    {

        public static void openLink(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

    }
}
