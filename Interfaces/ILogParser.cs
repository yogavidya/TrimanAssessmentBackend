// <copyright file="ILogParser.cs" company="Salvatore Uras">
// Copyright (c) Salvatore Uras. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;


namespace TrimanAssessment.Interfaces
{
    public enum LogParserStatus
    {
        Empty,  // contains no log information (ParseStream never invoked)
        Running,
        Initialized, // successfully executed ParseStream()
        Error // contains no log information (ParseStream failed)
    }

    public interface ILogParser
    {
        List<Models.ClientIPReport> ClientIPReports
        {
            get;
        }

        void ParseStream(Stream fs);

        LogParserStatus Status
        {
            get
            {
                return LogParserStatus.Empty;
            }
        }

        ulong ParsedLines
        {
            get
            {
                return 0;
            }
        }
    }
}
