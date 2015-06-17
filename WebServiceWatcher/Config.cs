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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WebServiceWatcher
{
    /// <summary>
    ///     Configuration loader
    /// </summary>
    public class Config
    {
        public readonly int Retries;
        public readonly int SMTPPort;
        public readonly int WebTimeout;
        public readonly int PollInterval;
        public readonly int LogThreshhold;
        public readonly int RestartTimeout;
        public readonly int EmailThreshhold;
        public readonly string TestURL;
        public readonly string LogFile;
        public readonly string ToEmail;
        public readonly string FromEmail;
        public readonly string SMTPServer;
        public readonly string ServiceName;

        public Config(string configFilename)
        {
            // sane defaults
            WebTimeout = 10000; // 10 seconds
            RestartTimeout = 30000; // 30 seconds
            PollInterval = 600000; // 10 minutes
            SMTPPort = 25;
            Retries = 3;
            TestURL = "http://127.0.0.1";
            ServiceName = "Apache2.4";
            EmailThreshhold = 1;
            LogThreshhold = 4;

            if (!File.Exists(configFilename)) return;

            XMLConfig xml;
            if ((xml = LoadXML(configFilename)) == null)
                return;

            if (xml.WebTimeout != -1)
                WebTimeout = xml.WebTimeout;
            if (xml.RestartTimeout != -1)
                RestartTimeout = xml.RestartTimeout;
            if (xml.PollInterval != -1)
                PollInterval = xml.PollInterval;
            if (xml.SMTPPort != -1)
                SMTPPort = xml.SMTPPort;
            if (xml.EmailThreshhold != -1)
                EmailThreshhold = xml.EmailThreshhold;
            if (xml.LogThreshhold != -1)
                LogThreshhold = xml.LogThreshhold;
            if (xml.Retries != -1)
                Retries = xml.Retries;
            if (xml.TestURL != null)
                TestURL = xml.TestURL;
            if (xml.ServiceName != null)
                ServiceName = xml.ServiceName;
            if (xml.FromEmail != null)
                FromEmail = xml.FromEmail;
            if (xml.ToEmail != null)
                ToEmail = xml.ToEmail;
            if (xml.SMTPServer != null)
                SMTPServer = xml.SMTPServer;
            if (xml.LogFile != null)
                LogFile = xml.LogFile;
        }

        /// <summary>
        ///     Loads/serializes the XML configuration
        /// </summary>
        /// <param name="configFilename"></param>
        /// <returns></returns>
        private static XMLConfig LoadXML(string configFilename)
        {
            var serializer = new XmlSerializer(typeof(XMLConfig));
            try
            {
                var reader = new FileStream(configFilename, FileMode.Open);
                return (XMLConfig)serializer.Deserialize(reader);
            }
            catch (Exception)
            {
                OutputHandler.WriteOut(OutputHandler.MessageTypes.Warning,
                    "XML configuration could not be loaded! Proceeding with defaults.");
            }
            return null;
        }

        /// <summary>
        ///     The XML Configuration File Schema
        /// </summary>
        [GeneratedCode("xsd", "4.0.30319.18020")]
        [Serializable]
        [DebuggerStepThrough]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", ElementName = "WebServiceWatcher", IsNullable = false)]
        public class XMLConfig
        {
            private string _fromEmailField;
            private int _pollIntervalField = -1;
            private int _restartTimeoutField = -1;
            private int _retriesField = -1;
            private string _serviceNameField;
            private int _smtpPortField = -1;
            private string _smtpServerField;
            private string _testURLField;
            private string _toEmailField;
            private int _webTimeoutField = -1;
            private int _emailThreshholdField = -1;
            private int _logThreshholdField = -1;
            private string _logFileField;

            /// <summary>
            /// message priority required to trigger an email
            /// Critical=0, Error=1, Warning=2, Message=3, Notice=4, Debug=5
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int EmailThreshhold
            {
                get { return _emailThreshholdField; }
                set { _emailThreshholdField = value; }
            }

            /// <summary>
            /// message priority required to trigger a log message
            /// Critical=0, Error=1, Warning=2, Message=3, Notice=4, Debug=5
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int LogThreshhold
            {
                get { return _logThreshholdField; }
                set { _logThreshholdField = value; }
            }

            /// <summary>
            ///     Timeout for web requests (miliseconds)
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int WebTimeout
            {
                get { return _webTimeoutField; }
                set { _webTimeoutField = value; }
            }

            /// <summary>
            ///     Interval at which to poll the web server (miliseconds)
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int PollInterval
            {
                get { return _pollIntervalField; }
                set { _pollIntervalField = value; }
            }

            /// <summary>
            ///     Number of request attempts before the web server is considered unreachable
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int Retries
            {
                get { return _retriesField; }
                set { _retriesField = value; }
            }

            /// <summary>
            ///     The URI to query
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string TestURL
            {
                get { return _testURLField; }
                set { _testURLField = value; }
            }

            /// <summary>
            ///     Log file path
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string LogFile
            {
                get { return _logFileField; }
                set { _logFileField = value; }
            }

            /// <summary>
            ///     SMTP server port number
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int SMTPPort
            {
                get { return _smtpPortField; }
                set { _smtpPortField = value; }
            }

            /// <summary>
            ///     Email address to which email will be sent
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string ToEmail
            {
                get { return _toEmailField; }
                set { _toEmailField = value; }
            }

            /// <summary>
            ///     Email address from which email will be sent
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string FromEmail
            {
                get { return _fromEmailField; }
                set { _fromEmailField = value; }
            }

            /// <summary>
            ///     SMTP server address
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string SMTPServer
            {
                get { return _smtpServerField; }
                set { _smtpServerField = value; }
            }

            /// <summary>
            ///     Name of the Windows Service that will be restarted
            ///     if the web server is unreachable
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public string ServiceName
            {
                get { return _serviceNameField; }
                set { _serviceNameField = value; }
            }

            /// <summary>
            ///     Timeout before the windows service restart is considered failed (miliseconds)
            /// </summary>
            [XmlElement(Form = XmlSchemaForm.Unqualified)]
            public int RestartTimeout
            {
                get { return _restartTimeoutField; }
                set { _restartTimeoutField = value; }
            }
        }
    }
}