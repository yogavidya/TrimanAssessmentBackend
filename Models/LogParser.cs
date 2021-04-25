using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TrimanAssessment.Models
{
    public class LogParser
    {
        private LogParser() { } // ensure no parameterless instance creation
        public LogParser(Stream logStream)
        {
            logFileStream = logStream;
            var initialPosition = logFileStream.Position;
            logFileStream.Position = 0;
            if((this.logFileEncoding = detectEncoding(logStream)) == null)
                throw new Exception("Unable to detect encoding for this file.");
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(logFileStream);
                logFileText = reader.ReadToEnd();
            }
            catch (IOException exc) {
                throw new Exception(String.Format("{0}: read error on file.", exc.GetType().Name));
            }
            catch (OutOfMemoryException exc) {
                throw new Exception(String.Format("{0}: out of memory.", exc.Message));
            }
            catch (Exception exc)
            {
                throw (exc);
            }
            var pattern = @"^#Software: .*?$\n#Version: .*?$\n#Date: .*?$\n#Fields: (?<fields>[\w*\-*\(*\)* ]*)";
            Regex re = new Regex(pattern,
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matches = re.Matches(logFileText,0);
            var clientIPIndex = Array.FindIndex<String>(
                matches[0].Groups[1].Value.Split(' '), 
                s => s == "c-ip");
            // end
            logFileStream.Position = initialPosition;
        }

        protected bool ParseBlock()
        {
            return true;
        }

        protected bool ParseStreaming()
        {
            return true;
        }

        protected Encoding detectEncoding(Stream logStream)
        {
            var initialPosition = logStream.Position;
            Encoding enc = null;
            try
            {
                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(logStream);
                cdet.DataEnd();
                if (cdet.Charset == null)
                {
                    throw (new Exception("Unknown encoding."));
                }
                enc = Encoding.GetEncoding(cdet.Charset);
            }
            catch (Exception exc)
            {
                enc = null;
            }
            finally
            {
                logStream.Position = initialPosition;
            }
            return enc;
        }

        private Stream logFileStream = null;
        private Encoding logFileEncoding = null;
        private String logFileText = null;
    }
}
