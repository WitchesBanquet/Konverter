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

using Microsoft.Extensions.Hosting;

namespace Konverter;

public class Hosting : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<Hosting> _logger;

    public Hosting(IHostApplicationLifetime lifetime, ILogger<Hosting> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosting start called");
        _logger.LogInformation("Hosting starting");

        _lifetime.ApplicationStarted.Register(OnStarted);
        _lifetime.ApplicationStopping.Register(OnStopping);
        _lifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosting stop called");
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("Hosting started");
    }

    private void OnStopping()
    {
        _logger.LogInformation("Hosting stopping");
    }

    private void OnStopped()
    {
        _logger.LogInformation("Hosting stopped");
    }
}
