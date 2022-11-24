using System;
using System.Text.Json;

using Telegram.Bot;

using Microsoft.Extensions.Configuration;

using WhatTheDown;
using System.Diagnostics;

/*var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var secrets = config.Providers.First();
secrets.TryGet("TGAPIkey", out string? botApiKey);*/
var botApiKey = Environment.GetEnvironmentVariable("TGAPIKEY");
_ = botApiKey ?? throw new ArgumentNullException("api key was not found :(");
_ = Environment.GetEnvironmentVariable("PORT");

var bot = new Bot(botApiKey).AddRedditDownloader();

Console.WriteLine("Bot has started");

Process.GetCurrentProcess().WaitForExit();