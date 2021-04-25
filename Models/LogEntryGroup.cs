using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TrimanAssessment.Models
{
    public struct LogEntryGroup
    {
        public LogEntryGroup( string IP, ulong calls)
        {
            ClientIP = IP;
            Calls = calls;
            IPHostEntry hostEntry = null;
            ErrorMessage = "";
            try
            {
                hostEntry = Dns.GetHostEntry(IP);
            }
            catch (Exception exc)
            {
                hostEntry = null;
                ErrorMessage = exc.Message;
            }
            finally
            {
                if (hostEntry != null)
                {
                    FQDN = hostEntry.HostName;
                } else
                {
                    FQDN = "";
                }
            }
        }
        public string ClientIP { get; }
        public string FQDN { get; }

        public ulong Calls { get; }

        public string ErrorMessage { get; }
    }
}
