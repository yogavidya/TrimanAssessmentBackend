using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TrimanAssessment.Models
{
    public class ClientIPReport
    {
        private ClientIPReport() { }
        public ClientIPReport( string IP, ulong calls)
        {
            _clientIP = IP;
            _calls = calls;
            IPHostEntry? hostEntry = null;
            try
            {
                hostEntry = Dns.GetHostEntry(IPAddress.Parse(_clientIP));
            }
            catch (Exception exc)
            {
                hostEntry = null;
                _errorMessage = exc.Message;
            }
            finally
            {
                if (hostEntry != null)
                {
                    _fqdn = hostEntry.HostName;
                } else
                {
                    _fqdn = "";
                }
            }
        }

        public ulong AddCall()
        {
            return this._calls++;
        }

        protected string _clientIP = "";
        public string ClientIP { 
            get {
                return _clientIP;
            } 
        }
        protected string _fqdn = "";
        public string FQDN
        {
            get
            {
                return _fqdn;
            }
        }
        protected ulong _calls = 0;
        public ulong Calls
        {
            get
            {
                return _calls;
            }
        }
        protected string _errorMessage = "";
        public string ErrorMessage { get { return _errorMessage; } }
    }
}
