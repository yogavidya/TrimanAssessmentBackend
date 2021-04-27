// <copyright file="ILogParser.cs" company="Salvatore Uras">
// Copyright (c) Salvatore Uras. All rights reserved.
// </copyright>

namespace TrimanAssessment.Interfaces
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Status descriptions for a <see cref="ILogParser"/> service.
    /// </summary>
    public enum LogParserStatus
    {
        /// <summary>
        /// Service contains no log information (<see cref="ILogParser.ParseStream(Stream)"/> never invoked).
        /// </summary>
        Empty,

        /// <summary>
        /// Service is executing <see cref="ILogParser.ParseStream(Stream)"/>.
        /// </summary>
        Running,

        /// <summary>
        /// Service successfully executed <see cref="ILogParser.ParseStream(Stream)"/> and contains information.
        /// (See ILogParser.ClientIPReports).
        /// </summary>
        Initialized,

        /// <summary>
        /// Service contains no log information (<see cref="ILogParser.ParseStream(Stream)"/> failed).
        /// </summary>
        Error,
    }

    /// <summary>
    /// The interface exposed by a LogParser service.
    /// </summary>
    public interface ILogParser
    {
        /// <summary>
        /// Gets the list of parsed reports (<see cref="Models.ClientIPReport"/>).
        /// </summary>
        List<Models.ClientIPReport> ClientIPReports
        {
            get;
        }

        /// <summary>
        /// Gets the status of the LogParser service (<see cref="LogParserStatus"/>).
        /// </summary>
        LogParserStatus Status
        {
            get
            {
                return LogParserStatus.Empty;
            }
        }

        /// <summary>
        /// Gets the number of lines parsed by the service (<see cref="ILogParser.ParseStream(Stream)"/>).
        /// </summary>
        ulong ParsedLines
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Parses an IIS log file Stream, retrieving aggregate information on entries (<see cref="Models.ClientIPReport"/>).
        /// </summary>
        /// <param name="fs">An IIS log file Stream.</param>
        void ParseStream(Stream fs);
    }
}
