namespace TrimanAssessment.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class ClientIPReport
    {
        private string clientIP = string.Empty;
        private string fqdn = string.Empty;
        private ulong calls = 0;
        private string errorMessage = string.Empty;

        public ClientIPReport( string ip, ulong calls)
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
                } else
                {
                    this.fqdn = string.Empty;
                }
            }
        }

        public ulong AddCall()
        {
            return this.calls++;
        }

        public string ClientIP
        {
            get
            {
                return this.clientIP;
            }
        }

        public string FQDN
        {
            get
            {
                return this.fqdn;
            }
        }

        public ulong Calls
        {
            get
            {
                return this.calls;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
        }
    }
}
