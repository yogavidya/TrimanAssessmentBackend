using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrimanAssessment.Interfaces;
using TrimanAssessment.Models;

namespace TrimanAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParseLogFileController : ControllerBase
    {
        private readonly ILogger<ParseLogFileController> logger;
        private readonly ILogParser logParser;

        public ParseLogFileController(ILogParser logParser)
        {
            this.logParser = logParser;
        }

        [HttpGet]
        public JsonResult PollPendingParseOperation()
        {
            var jsonSourceData = new Dictionary<string, object>();
            switch (this.logParser.Status)
            {
                case LogParserStatus.Empty:
                    jsonSourceData["status"] = "empty";
                    break;
                case LogParserStatus.Running:
                    jsonSourceData["status"] = "running";
                    break;
                case LogParserStatus.Error:
                    jsonSourceData["status"] = "error";
                    break;
                case LogParserStatus.Initialized:
                    jsonSourceData["status"] = "initialized";
                    break;
            }

            jsonSourceData["parsedLines"] = this.logParser.ParsedLines;
            return new JsonResult(jsonSourceData);
        }

        [HttpPost]
        public JsonResult ParseUploadedLogFile([FromForm] IFormCollection filesData)
        {
            var responseBodyMap = new Dictionary<string, object>();
            try
            {
                responseBodyMap["status"] = "OK";
                using (var fs = filesData.Files[0].OpenReadStream())
                {
                    this.logParser.ParseStream(fs);
                    var jsonSourceData = new List<Dictionary<string, object>>();
                    this.logParser.ClientIPReports.ForEach(entry =>
                    {
                        var entryData = new Dictionary<string, object>()
                        {
                            { "ip", entry.ClientIP },
                            { "fqdn", entry.FQDN },
                            { "calls", entry.Calls },
                        };
                        jsonSourceData.Add(entryData);
                    });
                    responseBodyMap["status"] = "OK";
                    responseBodyMap["data"] = jsonSourceData;
                    return new JsonResult(responseBodyMap);
                }
            }
            catch (Exception exc)
            {
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;
                responseBodyMap["status"] = "Server error";
                responseBodyMap["errorMessage"] = exc.Message;
                return new JsonResult(responseBodyMap);
            }
        }
    }
}
