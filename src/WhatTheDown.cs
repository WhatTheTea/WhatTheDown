using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WhatTheDown.Downloaders;
namespace WhatTheDown;
public class WhatTheDown
{
    private readonly ILogger _log;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    public WhatTheDown(ILoggerFactory loggerFactory, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _log = loggerFactory.CreateLogger<WhatTheDown>();
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    [Function("BotStart")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        var botApiKey = _config["TGAPIKEY"] ?? throw new Exception("api key was not found");

        var httpClient = _httpClientFactory.CreateClient();

        var bot = Bot.Get(botApiKey)
                     .AddRedditDownloader(httpClient)
                     .AddTikTokDownloader(httpClient);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Bot has been started");
        return response;
    }
}