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

using Konverter.Runner;
using Microsoft.Extensions.Hosting;

namespace Konverter.Services;

public class CommandLineService : ICommandLineService
{
    private readonly ILogger<CommandLineService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IEnumerable<IRunner> _runners;

    public CommandLineService(
        ILogger<CommandLineService> logger,
        IHostApplicationLifetime lifetime,
        IEnumerable<IRunner> runners)
    {
        _logger = logger;
        _lifetime = lifetime;
        _runners = runners;
    }

    public Task Run()
    {
        _logger.LogInformation("Command line service started");

        PrintHeading();

        while (true)
        {
            var val = PrintMainMenu();
            switch (val)
            {
                case Functions.NewSession:
                    _runners.FirstOrDefault(x => x.GetRunnerType() == RunnerType.NewSession)?.Run().Wait();
                    break;
                case Functions.AddNewImportTemplate:
                    _runners.FirstOrDefault(x => x.GetRunnerType() == RunnerType.AddNewImportTemplateRunner)?.Run().Wait();
                    break;
                case Functions.AddNewExportTemplate:
                    _runners.FirstOrDefault(x => x.GetRunnerType() == RunnerType.AddNewExportTemplateRunner)?.Run().Wait();
                    break;
                case Functions.Exit:
                    _lifetime.StopApplication();
                    return Task.CompletedTask;
                default:
                    // Never happens
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    #region Print Method

    private void PrintHeading()
    {
        _logger.LogInformation("Command line service printing heading");
        AnsiConsole.Write(new FigletText("Konverter").LeftAligned().Color(Color.Green));
        AnsiConsole.Write(new Markup("[green bold] Konverter - 通用文本样式转换器 [/]" +
                                     "[green]是一个使用[/]" +
                                     "[green bold] GPL-v3 [/]" +
                                     "[green]许可证的开源软件[/]"));
        AnsiConsole.WriteLine();
        _logger.LogInformation("Command line service printed heading");
    }

    private Functions PrintMainMenu()
    {
        _logger.LogInformation("Command line service printing main menu");

        var fs = Enum.GetValues<Functions>();
        _logger.LogInformation("Get {count} main menu items", fs.Length);

        var choice = new Dictionary<Functions, string>();
        foreach (var f in fs)
        {
            try
            {
                var (index, desc) = f.GetMenuInfo();
                choice.Add(f ,$"{index}. {desc}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in PrintMainMenu()");
                AnsiConsole.WriteException(e);
                return Functions.Exit;
            }
        }

        var s = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("请选择功能")
            .PageSize(10)
            .MoreChoicesText("向上或向下移动光标来查看更多选项")
            .AddChoices(choice.Values));
        var selectedFunction = choice.FirstOrDefault(x => x.Value == s).Key;
        _logger.LogInformation("User selected main menu item: {item}", selectedFunction.ToString());
        return selectedFunction;
    }

    #endregion
}
