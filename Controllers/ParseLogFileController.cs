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
        private readonly ILogger<ParseLogFileController> _logger;
        private readonly ILogParser _logParser;

        public ParseLogFileController(ILogParser logParser)
        {
            _logParser = logParser;
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
                    _logParser.ParseStream(fs);
                    var JsonSourceData = new List<Dictionary<string, object>>();
                    _logParser.ClientIPReports.ForEach(entry => {
                        var entryData = new Dictionary<string, object>()
                        {
                            { "ip", entry.ClientIP },
                            { "fqdn", entry.FQDN },
                            { "calls", entry.Calls },
                        };
                        JsonSourceData.Add(entryData);
                    });
                    responseBodyMap["data"] = JsonSourceData;
                    return new JsonResult(responseBodyMap);
                }
            }
            catch(Exception exc) {
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;
                responseBodyMap["status"] = "Server error";
                responseBodyMap["message"] = exc.Message;
                return new JsonResult(responseBodyMap);
            }
        }
    }
}
