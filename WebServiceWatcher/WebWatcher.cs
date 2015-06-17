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
using System.Net;
using System.Timers;
using WebServiceWatcher.Util;

namespace WebServiceWatcher
{
    /// <summary>
    /// Periodically tests a web server's availability
    /// Also loads the application configuration
    /// </summary>
    class WebWatcher
    {
        private readonly Timer _timer;

        /// <summary>
        /// Starts the web watcher
        /// </summary>
        public WebWatcher()
        {
            // start the main event loop
            _timer = new Timer(WebWatchLauncher.Configuration.PollInterval);
            _timer.Elapsed += DoWebTest;
            _timer.Start();
        }

        /// <summary>
        /// Stops the web watcher event loop
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Determines if web server is available;
        /// If it is not, the web server service will be restarted.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void DoWebTest(object source, ElapsedEventArgs e)
        {
            OutputHandler.WriteOut(OutputHandler.MessageTypes.Notice, "WebMonitor Timer Triggered at " + e.SignalTime);

            // try up to MaxRetries times, spaced by 10 seconds each try
            var loops = 0;
            while (loops < WebWatchLauncher.Configuration.Retries)
            {
                OutputHandler.WriteOut(OutputHandler.MessageTypes.Notice, "Testing for web server response [" + (loops + 1) + "/" + WebWatchLauncher.Configuration.Retries + "].");
                var result = WebTestTool.TestURL(WebWatchLauncher.Configuration.TestURL, WebWatchLauncher.Configuration.WebTimeout);
                if (result.ResponseStatus == WebExceptionStatus.Success && result.ResponseCode == HttpStatusCode.OK)
                {
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Message, "Response okay! Length: " + result.ReceivedLength + "; Expected: " + result.ReceivedLength);
                    OutputHandler.FlushWrites();
                    return;
                }
                if (result.ResponseStatus == WebExceptionStatus.Timeout)
                {
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Warning, "Response timed out!");
                }
                else
                {
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Warning,
                        "Response failure! ResponseStatus: " + result.ResponseStatus + "; ResponseCode: " +
                        result.ResponseCode);
                }
                loops++;
                // sleep 10 seconds if we are going to loop again
                if (loops < WebWatchLauncher.Configuration.Retries)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }

            // the server is unreachable.
            OutputHandler.WriteOut(OutputHandler.MessageTypes.Error, "Web server failed to respond after " + WebWatchLauncher.Configuration.Retries + " attempts.");
            OutputHandler.WriteOut(OutputHandler.MessageTypes.Notice, "Restarting Service...");

            if (!NamedServiceControl.Restart(WebWatchLauncher.Configuration.ServiceName, WebWatchLauncher.Configuration.RestartTimeout))
            {
                OutputHandler.WriteOut(OutputHandler.MessageTypes.Error, "Failed to Restart Service!",
                    "URL '" + WebWatchLauncher.Configuration.TestURL + "' failed to respond after 3 attempts" + "\n" +
                    "Service '" + WebWatchLauncher.Configuration.ServiceName + "' could not be restarted!" + "\n\n\n" +
                    "ATTENTION REQUIRED!");
                OutputHandler.FlushWrites();
                return;
            }
            OutputHandler.WriteOut(OutputHandler.MessageTypes.Message, "Service restarted successfully.");
            OutputHandler.WriteOut(OutputHandler.MessageTypes.Notice, "Sleeping for 2 seconds before verifying connectivity...");

            // test again to verify connectivity
            loops = 0;
            while (loops < WebWatchLauncher.Configuration.Retries)
            {
                System.Threading.Thread.Sleep(2000);
                OutputHandler.WriteOut(OutputHandler.MessageTypes.Notice, "Testing for web server response [" + (loops + 1) + "/" + WebWatchLauncher.Configuration.Retries + "].");

                var result = WebTestTool.TestURL(WebWatchLauncher.Configuration.TestURL, WebWatchLauncher.Configuration.WebTimeout);
                if (result.ResponseStatus == WebExceptionStatus.Success && result.ResponseCode == HttpStatusCode.OK)
                {
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Message, "Response okay! Length: " + result.ReceivedLength + "; Expected: " + result.ReceivedLength);
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Error, "Service Restart Was Required!",
                        "URL '" + WebWatchLauncher.Configuration.TestURL + "' failed to respond after " + WebWatchLauncher.Configuration.Retries + " attempts" + "\n" +
                        "Service '" + WebWatchLauncher.Configuration.ServiceName + "' was restarted." + "\n\n\n" +
                        "Response was okay after service restart!"
                        );
                    OutputHandler.FlushWrites();
                    return;
                }
                if (result.ResponseStatus == WebExceptionStatus.Timeout)
                {
                    OutputHandler.WriteOut(OutputHandler.MessageTypes.Warning, "Response timed out!");
                    return;
                }
                OutputHandler.WriteOut(OutputHandler.MessageTypes.Warning, "Response failure! ResponseStatus: " + result.ResponseStatus + "; ResponseCode: " + result.ResponseCode);
                loops++;
            }

            OutputHandler.WriteOut(OutputHandler.MessageTypes.Error, "Web server is offline!",
                "URL '" + WebWatchLauncher.Configuration.TestURL + "' failed to respond after 3 attempts" + "\n" +
                "Service '" + WebWatchLauncher.Configuration.ServiceName + "' was restarted!" + "\n\n\n" +
                "Response still FAILED after service restart!" + "\n\n\n" +
                "ATTENTION REQUIRED!"
            );
            OutputHandler.FlushWrites();
        }
    }
}
