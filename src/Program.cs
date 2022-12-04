using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

using WhatTheDown;
using WhatTheDown.Downloaders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddGoogleDiagnosticsForAspNetCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var port = app.Configuration["PORT"];
if(!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.UseHttpsRedirection();

bool botStarted = false;
app.MapGet("/bot/start", () =>
{
    var response = "Bot is already running";
    if(!botStarted)
    {
        var botApiKey = app.Configuration["TGAPIKEY"] ?? throw new Exception("api key was not found");

        var httpClient = new HttpClient();
        var bot = Bot.Get(botApiKey)
                        .AddRedditDownloader(httpClient)
                        .AddTikTokDownloader(httpClient);
        botStarted = true;
        response = "Bot has been started";
    }
    
    return response;
}).WithName("StartBot");

app.Run();
