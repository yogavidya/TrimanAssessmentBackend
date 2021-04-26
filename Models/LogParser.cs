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

        private readonly string cantParse = "Unable to parse file.";

        public List<ClientIPReport> ClientIPReports
        {
            get
            {
                return this.clientIPReports;
            }
        }

        public bool ParseStream(Stream fs)
        {
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
                } while (more);
            }
            catch (Exception)
            {
                this.status = LogParserStatus.Error;
            }

            fs.Position = initialPosition;
            return true;
        }

        protected bool ParseNextBlock(StreamReader sr)
        {
            if (sr.EndOfStream)
            {
                return false;
            }

            var headerText = this.ReadBlockHeader(sr);
            if (headerText == null)
            {
                throw new Exception(this.cantParse);
            }

            var matches = this.blockHeaderRegex.Matches(headerText, 0);
            if (matches.Count != 1)
            {
                throw new Exception(this.cantParse);
            }

            var clientIPIndex = Array.FindIndex<string>(
                matches[0].Groups[1].Value.Split(' '),
                s => s == "c-ip");
            if (clientIPIndex == -1)
            {
                throw new Exception(this.cantParse);
            }

            this.ReadBlockEntries(sr, clientIPIndex);
            return true;
        }

        protected void ReadBlockEntries(
            StreamReader sr,
            int clientIPIndex)
        {
            while ((sr.EndOfStream == false) && (sr.Peek() != '#'))
            {
                var line = sr.ReadLine();
                if (line != null)
                {
                    var clientIP = line.Split(' ')[clientIPIndex];
                    var listedEntryIndex = this.clientIPReports.FindIndex(entry => entry.ClientIP == clientIP);
                    if (listedEntryIndex == -1)
                    {
                        this.clientIPReports.Add(new ClientIPReport(clientIP, 1));
                    }
                    else
                    {
                        this.clientIPReports[listedEntryIndex].AddCall();
                    }
                    System.Diagnostics.Debug.WriteLine("Count: {0}", this.clientIPReports.Count);
                }
            }
        }

        protected String? ReadBlockHeader(StreamReader sr)
        {
            var result = new String("");
            int linesReadCount = 0;
            while(linesReadCount < 4)
            {
                if (sr.Peek() != '#') return null;
                var line = sr.ReadLine();
                if (line == null) return null;
                result = result + line + "\r\n";
                linesReadCount++;
            }
            return result;
        }

        protected bool ParseBlock()
        {
            return true;
        }

        protected bool ParseStreaming()
        {
            return true;
        }
        private LogParserStatus status = LogParserStatus.Empty;
        public LogParserStatus Status {
            get { return this.status; }
        }
    }
}
