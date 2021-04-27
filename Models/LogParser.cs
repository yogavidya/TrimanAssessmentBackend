// <copyright file="LogParser.cs" company="Salvatore Uras">
// Copyright (c) Salvatore Uras. All rights reserved.
// </copyright>

namespace TrimanAssessment.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using TrimanAssessment.Interfaces;

    /// <summary>
    /// Implements the <see cref="Interfaces.ILogParser"/> interface.
    /// This is the actual service implementation in the API.
    /// </summary>
    public class LogParser : Interfaces.ILogParser
    {
        private readonly List<ClientIPReport> clientIPReports = new List<ClientIPReport>();

        private readonly Regex blockHeaderRegex =
            new Regex(
                string.Format(
                @"^{0}\n{1}\n{2}\n{3}",
                "#Software: .*?$",
                "#Version: .*?$",
                "#Date: .*?$",
                @"#Fields: (?<fields>[\w*\-*\(*\)* ]*)"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private LogParserStatus status = LogParserStatus.Empty;

        private ulong parsedLines = 0;

        /// <inheritdoc/>
        public LogParserStatus Status
        {
            get { return this.status; }
        }

        /// <inheritdoc/>
        public ulong ParsedLines
        {
            get
            {
                return this.parsedLines;
            }
        }

        /// <inheritdoc/>
        public List<ClientIPReport> ClientIPReports
        {
            get
            {
                return this.clientIPReports;
            }
        }

        /// <inheritdoc/>
        public void ParseStream(Stream fs)
        {
            this.status = LogParserStatus.Running;
            var initialPosition = fs.Position;
            fs.Position = 0;
            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(fs, true);
                bool more;
                do
                {
                    more = this.ParseNextBlock(reader);
                }
                while (more);
                this.status = LogParserStatus.Initialized;
            }
            catch (Exception exc)
            {
                this.status = LogParserStatus.Error;
                throw new Exception(exc.Message);
            }

            fs.Position = initialPosition;
        }

        /// <summary>
        /// Parses the next entry block in the log file and updates <see cref="clientIPReports"/> accordingly.
        /// </summary>
        /// <param name="sr">A StreamReader for the log file.</param>
        /// <returns>bool true if there was a block to parse, else false.</returns>
        protected bool ParseNextBlock(StreamReader sr)
        {
            if (sr.EndOfStream)
            {
                return false;
            }

            var headerText = this.ReadBlockHeader(sr);
            if (headerText == null)
            {
                this.ThrowCantParse();
            }

            var matches = this.blockHeaderRegex.Matches(headerText, 0);
            if (matches.Count != 1)
            {
                this.ThrowCantParse();
            }

            var clientIPIndex = Array.FindIndex<string>(
                matches[0].Groups[1].Value.Split(' '),
                s => s == "c-ip");
            if (clientIPIndex == -1)
            {
                this.ThrowCantParse();
            }

            this.ReadBlockEntries(sr, clientIPIndex);
            return true;
        }

        /// <summary>
        /// Throws an Exception reporting a parse failure and the number of successfully read lines before the error.
        /// Used by <see cref="ParseNextBlock(StreamReader)" and <see cref="ReadBlockEntries(StreamReader, int)"/>.
        /// </summary>
        protected void ThrowCantParse()
        {
            throw new Exception(string.Format("Unable to parse file after line {0}.", this.ParsedLines));
        }

        /// <summary>
        /// Reads and parses the actual log entries after a block header.
        /// </summary>
        /// <param name="sr">The StreamReader for a log file.</param>
        /// <param name="clientIPIndex">The index for the client_ip in a space delimited log entry data line.</param>
        protected void ReadBlockEntries(
            StreamReader sr,
            int clientIPIndex)
        {
            while ((sr.EndOfStream == false) && (sr.Peek() != '#'))
            {
                var line = sr.ReadLine();
                if (line != null)
                {
                    var tokens = line.Split(' ');
                    if (tokens.Length <= clientIPIndex)
                    {
                        this.ThrowCantParse();
                    }

                    var clientIP = tokens[clientIPIndex];

                    // Obscure bug, sometimes ::1 in file appears in read line as ::1%0
                    if (clientIP == "::1%0")
                    {
                        clientIP = "::1"; // Quick'n'dirty fix
                    }

                    var listedEntryIndex = this.clientIPReports.FindIndex(entry => entry.ClientIP == clientIP);
                    if (listedEntryIndex == -1)
                    {
                        try
                        {
                            var newClientReport = new ClientIPReport(clientIP, 1);
                            this.clientIPReports.Add(newClientReport);
                        }
                        catch (Exception)
                        {
                            this.ThrowCantParse();
                        }
                    }
                    else
                    {
                        this.clientIPReports[listedEntryIndex].AddCall();
                    }

                    this.parsedLines++;
                }
            }
        }

        /// <summary>
        /// Reads the lines of a log file block header. 
        /// Used by <see cref="ParseNextBlock(StreamReader)"/>.
        /// </summary>
        /// <param name="sr">The StreamReader for this log file.</param>
        /// <returns>The header lines in a single string, or null if format not matching.</returns>
        protected string? ReadBlockHeader(StreamReader sr)
        {
            var result = new string(string.Empty);
            int linesReadCount = 0;
            while (linesReadCount < 4)
            {
                if (sr.Peek() != '#')
                {
                    return null;
                }

                var line = sr.ReadLine();
                if (line == null)
                {
                    return null;
                }

                result = result + line + "\r\n";
                linesReadCount++;
            }
            this.parsedLines += (ulong)linesReadCount;
            return result;
        }
    }
}
