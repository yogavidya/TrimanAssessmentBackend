using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

public enum LogParserStatus {
    Empty,  // contains no log information (ParseStream never invoked) 
    Initialized, // successfully executed ParseStream()
    Error // contains no log information (ParseStream failed) 
};

namespace TrimanAssessment.Interfaces
{
    public interface ILogParser
    {
        List<Models.ClientIPReport> ClientIPReports
        {
            get;
        }

        bool ParseStream(Stream fs);

        LogParserStatus Status
        {
            get {
                return LogParserStatus.Empty;
            }
        }
    }
}
