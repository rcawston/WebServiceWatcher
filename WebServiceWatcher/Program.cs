/*
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace WebServiceWatcher
{
    /// <summary>
    /// Handles windows service install/removal and debug mode startup
    /// CLI args:
    /// /I - install service; /U - uninstall service; 
    /// /D - run in debug (console) mode
    /// /LOG:{logfile} - write output to log file
    /// /CONFIG:{configfile} - path to xml config file
    /// </summary>
    public class WebWatchLauncher : ServiceBase
    {
        private static bool _debugMode;
        private static string _logFile;
        private static string _configFile;
        private WebWatcher _webMonitor;

        public static bool InDebugMode() { return _debugMode; }
        public static string GetLogFile() { return _logFile; }

        public static Config Configuration { get; private set; }

        public WebWatchLauncher()
        {
            ServiceName = "WebServiceWatcher";
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            // load the xml config
            if (_configFile == null)
                _configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    @"WebServiceWatcher.xml");

            Configuration = new Config(_configFile);

            if (!string.IsNullOrEmpty(Configuration.LogFile))
                _logFile = Configuration.LogFile;

            OutputHandler.SetThreshholds(Configuration.EmailThreshhold, Configuration.LogThreshhold);

            // start the web watcher
            _webMonitor = new WebWatcher();

        }

        protected override void OnStop()
        {
            base.OnStop();
            _webMonitor.Stop();
        }

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var t in args)
                {
                    if (t.ToUpper().StartsWith("/LOG"))
                        _logFile = t.Substring("/LOG:".Length);
                    else if (t.ToUpper().StartsWith("/CONFIG"))
                        _configFile = t.Substring("/CONFIG:".Length);
                    switch (t.ToUpper())
                    {
                        case "/I":
                        case "/INSTALL":
                            Util.SelfServiceInstaller.InstallService();
                            return;
                        case "/U":
                        case "/UNINSTALL":
                            Util.SelfServiceInstaller.UninstallService();
                            return;
                        case "/D":
                        case "/DEBUG":
                            _debugMode = true;
                            break;
                    }
                }
            }
            if (_debugMode)
            {
                var service = new WebWatchLauncher();
                service.OnStart(null);
                Console.WriteLine("WebServiceWatcher running in console debug mode...");
                Console.WriteLine("<press any key to exit...>");
                Console.Read();
            }
            else
            {
                Run(new WebWatchLauncher());
            }
        }
    }
}
