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
using System.IO;
using System.Net;

namespace WebServiceWatcher.Util
{
    /// <summary>
    /// Queries a web server to verify connectivity/uptime
    /// </summary>
    class WebTestTool
    {
        /// <summary>
        /// Result from web server connectivity test
        /// </summary>
        public class WebTestToolResponse
        {
            public WebExceptionStatus ResponseStatus { get; private set; }
            public HttpStatusCode ResponseCode { get; private set; }
            public long Length { get; private set; }
            public long ReceivedLength { get; private set; }

            public WebTestToolResponse(WebExceptionStatus responseStatus, HttpStatusCode responseCode, long length, long receivedLength)
            {
                ResponseStatus = responseStatus; 
                ResponseCode = responseCode;
                Length = length;
                ReceivedLength = receivedLength;
            }
            public WebTestToolResponse(WebExceptionStatus responseStatus, HttpStatusCode responseCode)
                : this(responseStatus, responseCode, 0, 0) {}

            public WebTestToolResponse(WebExceptionStatus responseStatus)
                : this(responseStatus, HttpStatusCode.SeeOther, 0, 0) {}
        }

        /// <summary>
        /// Tests connectivity to the given URL
        /// </summary>
        /// <param name="url">URL to query</param>
        /// <param name="timeout">Maximum timeout in ms</param>
        /// <returns>Response as WebTestToolResponse</returns>
        public static WebTestToolResponse TestURL(string url, int timeout)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = timeout;
            // If required by the server, set the credentials.
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            try
            {
                var webResponse = (HttpWebResponse) webRequest.GetResponse();
                // Get the stream containing content returned by the server.
                var dataStream = webResponse.GetResponseStream();
                if (dataStream == null)
                    return new WebTestToolResponse(WebExceptionStatus.UnknownError);
                // Open the stream using a StreamReader for easy access.
                var reader = new StreamReader(dataStream);
                // Read the content. 
                var receivedContent = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Close();
                dataStream.Close();
                webResponse.Close();

                return new WebTestToolResponse(WebExceptionStatus.Success, webResponse.StatusCode, receivedContent.Length, webResponse.ContentLength);
            }
            catch (WebException e)
            {
                return e.Response == null ? new WebTestToolResponse(e.Status) : new WebTestToolResponse(e.Status, ((HttpWebResponse)e.Response).StatusCode);
            }
        }
    }
}

