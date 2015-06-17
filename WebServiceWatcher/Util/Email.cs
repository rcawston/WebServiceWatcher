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
using System.Net.Mail;

namespace WebServiceWatcher.Util
{
    /// <summary>
    /// Email processing handler
    /// </summary>
    class Email
    {
        public static bool SendEmail(string subject, string body)
        {
            var to = WebWatchLauncher.Configuration.ToEmail;
            var from = WebWatchLauncher.Configuration.FromEmail;
            var smtpserver = WebWatchLauncher.Configuration.SMTPServer;
            var port = WebWatchLauncher.Configuration.SMTPPort;

            // check for required options
            if (to == null || from == null || smtpserver == null)
            {
                return false;
            }
            var mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body
            };
            var client = new SmtpClient
            {
                Port = port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = smtpserver
            };
            client.Send(mail);
            return true;
        }
    }
}
