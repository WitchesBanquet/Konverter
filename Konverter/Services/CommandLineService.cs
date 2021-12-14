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

namespace Konverter.Services;

public class CommandLineService : ICommandLineService
{
    private readonly ILogger<CommandLineService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ITemplateService _templateService;

    public CommandLineService(
        ILogger<CommandLineService> logger,
        IHostApplicationLifetime lifetime,
        ITemplateService templateService)
    {
        _logger = logger;
        _lifetime = lifetime;
        _templateService = templateService;
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
                    throw new NotImplementedException();
                case Functions.AddNewImportTemplate:
                    AddNewImportTemplate();
                    break;
                case Functions.AddNewExportTemplate:
                    AddNewExportTemplate();
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

    #region Runner Method

    private void AddNewImportTemplate()
    {
        _logger.LogInformation("Start to add new import template");
        var fileCheck = false;
        var filePath = "";
        while (fileCheck is false)
        {
            filePath = AnsiConsole.Ask<string>("请输入模版文件路径(输入 %quit 退出)：");
            if (filePath.Trim().ToLower() == "%quit")
            {
                _logger.LogInformation("Import template operation canceled by user");
                return;
            }

            if (!File.Exists(filePath))
            {
                continue;
            }

            _logger.LogInformation("File path is valid, import template original file path is {path}", filePath);
            fileCheck = true;
        }
        var name = AnsiConsole.Ask<string>("请输入模版名称(输入 %quit 退出)：");
        if (name.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return;
        }
        var description = AnsiConsole.Ask<string>("请输入模版描述(输入 %quit 退出)：");
        if (description.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return;
        }

        var model = new ImportTemplate { Name = name, Description = description };
        var obj = _templateService.ImportTemplateFromFile(filePath, model);
        if (obj is null)
        {
            _logger.LogError("Failed to add new import template");
            AnsiConsole.Write(new Markup("[red]添加失败，请查看日志文件获取详情[/]"));
            AnsiConsole.WriteLine();
            return;
        }
        _logger.LogInformation("Add import template successfully, id is {id}", obj.Id);
        AnsiConsole.Write(new Markup($"[green]添加成功，模版 Id：{obj.Id}[/]"));
    }

    private void AddNewExportTemplate()
    {
        _logger.LogInformation("Start to add new export template");
        var fileCheck = false;
        var indexCheck = false;
        var filePath = "";
        var indexPath = "";
        while (fileCheck is false)
        {
            filePath = AnsiConsole.Ask<string>("请输入模版文件路径(输入 %quit 退出)：");
            if (filePath.Trim().ToLower() == "%quit")
            {
                _logger.LogInformation("Import template operation canceled by user");
                return;
            }

            if (!File.Exists(filePath))
            {
                continue;
            }

            _logger.LogInformation("File path is valid, export template original file path is {path}", filePath);
            fileCheck = true;
        }
        while (indexCheck is false)
        {
            indexPath = AnsiConsole.Ask<string>("请输入模版文件路径(输入 %quit 退出，即无 Index 模版)：");
            if (indexPath.Trim().ToLower() == "%quit")
            {
                _logger.LogInformation("No index template");
                continue;
            }

            if (!File.Exists(indexPath))
            {
                continue;
            }

            _logger.LogInformation("File path is valid, export index template original file path is {path}", filePath);
            indexCheck = true;
        }
        var name = AnsiConsole.Ask<string>("请输入模版名称(输入 %quit 退出)：");
        if (name.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return;
        }
        var description = AnsiConsole.Ask<string>("请输入模版描述(输入 %quit 退出)：");
        if (description.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return;
        }

        var model = new ExportTemplate() { Name = name, Description = description };
        var obj = _templateService.ExportTemplateFromFile(filePath, indexPath, model);
        if (obj is null)
        {
            _logger.LogError("Failed to add new export template");
            AnsiConsole.Write(new Markup("[red]添加失败，请查看日志文件获取详情[/]"));
            AnsiConsole.WriteLine();
            return;
        }
        _logger.LogInformation("Add export template successfully, id is {id}", obj.Id);
        AnsiConsole.Write(new Markup($"[green]添加成功，模版 Id：{obj.Id}[/]"));
    }

    #endregion

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
