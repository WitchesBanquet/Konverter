// Konverter - Plain text file style converter
// Copyright (C) 2021 Witches Banquet
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License,
// or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

#region Setup Logging Path

var dllPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
if (dllPath is null)
{
    AnsiConsole.WriteException(new ArgumentNullException(nameof(dllPath), "Executing assembly path is null"));
    return;
}
var loggingFile = Path.Combine(dllPath, "Logs", $"log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log");

#endregion

#region Setup Serilog

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(loggingFile)
    .CreateLogger();

#endregion

#region Host Builder

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Hosting>();
});

builder.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
});

builder.UseSerilog();

#endregion

#region Run

var app = builder.Build();

await app.RunAsync();

#endregion
