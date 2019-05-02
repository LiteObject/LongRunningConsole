using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LongRunningConsole
{
    public class AppConfig
    {
        public string ApplicationName { get; set; }

        public Collection<ServiceConfig> Services { get; set; }
    }
}
