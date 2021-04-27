namespace TrimanAssessment.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

/// <summary>
/// Holds a bit of information about a single client IP in an IIS log file.
/// iLogParser exposes a list of these (see Interfaces/iLogParser and Models/LogParser).
/// </summary>
    public class ClientIPReport
    {
        private string clientIP = string.Empty;
        private string fqdn = string.Empty;
        private ulong calls = 0;
        private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIPReport"/> class.
        /// </summary>
        /// <param name="ip">
        /// The IP address for this set of log entries.
        /// </param>
        /// <param name="calls">
        /// The number of log entries in this set.
        /// </param>
        public ClientIPReport(string ip, ulong calls)
        {
            this.clientIP = ip;
            this.calls = calls;
            IPHostEntry? hostEntry = null;
            try
            {
                var ipAddress = IPAddress.Parse(this.clientIP);
                if (ipAddress.Equals(IPAddress.IPv6Loopback))
                {
                    hostEntry = new IPHostEntry();
                    hostEntry.HostName = "localhost";
                }
                else
                {
                    hostEntry = Dns.GetHostEntry(ipAddress);
                }
            }
            catch (System.Net.Sockets.SocketException exc)
            {
                hostEntry = null;
                this.errorMessage = exc.Message;
            }
            catch (Exception exc)
            {
                hostEntry = null;
                this.errorMessage = exc.Message;
                throw exc;
            }
            finally
            {
                if (hostEntry != null)
                {
                    this.fqdn = hostEntry.HostName;
                }
                else
                {
                    this.fqdn = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the client IP (string) for this set of log entries.
        /// </summary>
        public string ClientIP
        {
            get
            {
                return this.clientIP;
            }
        }

        /// <summary>
        /// Gets the FQDN (actually the hostname string) for this set of log entries.
        /// </summary>
        public string FQDN
        {
            get
            {
                return this.fqdn;
            }
        }

        /// <summary>
        /// Gets the number of log entries in this set.
        /// </summary>
        public ulong Calls
        {
            get
            {
                return this.calls;
            }
        }

        /// <summary>
        /// Gets the error message, if any, caused by exceptions in the constructor.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
        }

        /// <summary>
        /// Increments the <see cref="calls"/> member by 1.
        /// </summary>
        /// <returns>The value of <see cref="calls"/> after the increment.</returns>
        public ulong AddCall()
        {
            return this.calls++;
        }
    }
}
