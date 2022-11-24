using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WhatTheDown
{
    public class WhatTheDown
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        public WhatTheDown(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _log = loggerFactory.CreateLogger<WhatTheDown>();
            _config = config;

        }

        [Function("Start Bot")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            var botApiKey = _config["TGAPIKEY"] ?? throw new Exception("api key was not found :(");

            var bot = new Bot(botApiKey).AddRedditDownloader();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Bot has been started");
            return response;
        }
    }
}