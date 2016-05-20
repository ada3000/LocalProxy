using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalProxy
{
    public static class Config
    {
        public static int Port
        {
            get { return int.Parse(ConfigurationManager.AppSettings["port"]); }
        }

        public static bool EnableSsl
        {
            get { return ConfigurationManager.AppSettings["enableSsl"].ToLower() == "true"; }
        }
    }
}
