using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrimanAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadLogFileController : ControllerBase
    {
        private readonly ILogger<UploadLogFileController> _logger;

        [HttpPost]
        public JsonResult ReceiveUpload([FromForm] IFormCollection filesData)
        {
            var responseBodyMap = new Dictionary<string, string>();
            try
            {
                responseBodyMap["status"] = "OK";
                using (var fs = filesData.Files[0].OpenReadStream())
                {
                    var parser = new Models.LogParser(fs);
                    responseBodyMap["data"] = JsonSerializer.Serialize(new Models.LogEntryGroup("1.1.1.1", 12));
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
