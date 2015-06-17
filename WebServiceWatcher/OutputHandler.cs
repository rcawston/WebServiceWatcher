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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebServiceWatcher
{
    /// <summary>
    /// Handles logfile, console, and email notifications
    /// </summary>
    static class OutputHandler
    {
        public enum MessageTypes : byte { Critical=0, Error=1, Warning=2, Message=3, Notice=4, Debug=5 }

        private static MessageTypes _emailThreshhold = 0;
        private static MessageTypes _logThreshhold = (MessageTypes) 4;

        /// <summary>
        /// Sets the log and email message priority/type threshholds
        /// Threshholds:
        /// Critical=0, Error=1, Warning=2, Message=3, Notice=4, Debug=5
        /// </summary>
        /// <param name="emailThreshhold">message priority required to trigger an email</param>
        /// <param name="logThreshhold">message priority required to trigger a log message</param>
        public static void SetThreshholds(int emailThreshhold, int logThreshhold)
        {
            _emailThreshhold = (MessageTypes) emailThreshhold;
            _logThreshhold = (MessageTypes) logThreshhold;
        }

        /// <summary>
        /// Message storage type.  Represents a single output message.
        /// </summary>
        public class MessageInfo
        {
            public DateTime Time { get; private set; }
            public MessageTypes MessageType { get; private set; }
            public string Message { get; private set; }
            public string Details { get; private set; }

            public MessageInfo(DateTime time, MessageTypes messageType, string message, string details)
            {
                Time = time;
                MessageType = messageType;
                Message = message;
                Details = details;
            }
        }

        private static List<MessageInfo> _messageStore;

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="messageType">message severity/type</param>
        /// <param name="message">message content/body</param>
        public static void WriteOut(MessageTypes messageType, string message)
        {
            WriteOut(messageType, message, null);
        }

        /// <summary>
        /// Logs a detailed message
        /// </summary>
        /// <param name="messageType">message severity/type</param>
        /// <param name="message">message content/body</param>
        /// <param name="details">message details/background</param>
        public static void WriteOut(MessageTypes messageType, string message, string details)
        {
            // init the messageStore if needed
            if (_messageStore == null)
                _messageStore = new List<MessageInfo>();

            // Store the message in the recent message store
            _messageStore.Add(new MessageInfo(DateTime.Now.Date, messageType, message, details));

            // Output the message to console in Debug Mode
            if (WebWatchLauncher.InDebugMode())
                Console.WriteLine("["+DateTime.Now+"]["+messageType+"] " + message);

            // Do not write to log if LogFile not specified
            if (WebWatchLauncher.GetLogFile() == null) return;
            // Do not write to the log if the message is below threshhold
            if (messageType > _logThreshhold) return;

            // Try writing to the log
            try
            {
                using (var w = File.AppendText(WebWatchLauncher.GetLogFile()))
                {
                    w.WriteLine("[" + DateTime.Now + "][" + messageType + "] " + message);
                }
            }
            catch (Exception)
            {
                // There was a problem writing to the log...
                if (WebWatchLauncher.InDebugMode())
                    Console.WriteLine("[" + DateTime.Now + "][critical] FAILED TO WRITE LOG FILE!");
            }
        }

        /// <summary>
        /// Flushes message storage and sends email notification if there have are messages above the threshhold
        /// </summary>
        public static void FlushWrites()
        {
            if (_messageStore == null) return;
            // scan for messages above threshhold
            MessageInfo lastMessage = null;
            foreach (var message in _messageStore.Where(message => message.MessageType <= _emailThreshhold))
            {
                lastMessage = message;
            }
            if (lastMessage != null)
            {
                // send email notification
                WriteOut(MessageTypes.Notice, "Sending email notification");
                WriteOut(MessageTypes.Message,
                    Util.Email.SendEmail("[WebServiceWatch] " + lastMessage.Message, lastMessage.Details)
                        ? "Email notification sent successfully."
                        : "Email notification could NOT be sent.");
            }
            // clear the message store
            _messageStore.Clear();
        }
    }
}
