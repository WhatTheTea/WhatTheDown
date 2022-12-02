using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WhatTheDown.Downloaders;
namespace WhatTheDown;
public class WhatTheDown
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    public WhatTheDown(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;

        var botApiKey = _config["TGAPIKEY"] ?? throw new Exception("api key was not found");

        var httpClient = _httpClientFactory.CreateClient();

        var bot = Bot.Get(botApiKey)
                     .AddRedditDownloader(httpClient)
                     .AddTikTokDownloader(httpClient);

        Console.WriteLine("Bot has been started");
    }
}