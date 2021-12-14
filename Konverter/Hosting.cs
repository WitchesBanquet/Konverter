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

using Konverter.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Konverter;

public class Hosting : IHostedService
{
    private readonly ILogger<Hosting> _logger;
    private readonly ICommandLineService _commandLineService;

    public Hosting(
        ILogger<Hosting> logger,
        ICommandLineService commandLineService)
    {
        _logger = logger;
        _commandLineService = commandLineService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosting started");

        _commandLineService.Run();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosting stop called");
        AnsiConsole.Write(new Markup("[red]正在关闭..."));
        AnsiConsole.WriteLine();
        return Task.CompletedTask;
    }
}
