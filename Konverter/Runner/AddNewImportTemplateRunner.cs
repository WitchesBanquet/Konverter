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

namespace Konverter.Runner;

public class AddNewImportTemplateRunner : IRunner
{
    private readonly ILogger<AddNewImportTemplateRunner> _logger;
    private readonly ITemplateService _templateService;

    public AddNewImportTemplateRunner(ILogger<AddNewImportTemplateRunner> logger, ITemplateService templateService)
    {
        _logger = logger;
        _templateService = templateService;
    }

    public RunnerType GetRunnerType()
    {
        return RunnerType.AddNewImportTemplateRunner;
    }

    public Task Run()
    {
        _logger.LogInformation("Start to add new import template");

        // Get file
        var fileCheck = false;
        var filePath = "";
        while (fileCheck is false)
        {
            filePath = AnsiConsole.Ask<string>("请输入模版文件路径(输入 %quit 退出)：");
            if (filePath.Trim().ToLower() == "%quit")
            {
                _logger.LogInformation("Import template operation canceled by user");
                return Task.CompletedTask;
            }

            if (!File.Exists(filePath))
            {
                continue;
            }

            _logger.LogInformation("File path is valid, import template original file path is {path}", filePath);
            fileCheck = true;
        }

        // Filename Template
        var ft = AnsiConsole.Ask<string>("请输入文件名称模版(输入 %quit 退出，即不从文件名读取信息)：");
        if (ft.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("No filename template");
            ft = "";
        }

        // Metadata
        var name = AnsiConsole.Ask<string>("请输入模版名称(输入 %quit 退出)：");
        if (name.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return Task.CompletedTask;
        }
        var description = AnsiConsole.Ask<string>("请输入模版描述(输入 %quit 退出)：");
        if (description.Trim().ToLower() == "%quit")
        {
            _logger.LogInformation("Import template operation canceled by user");
            return Task.CompletedTask;
        }

        // Build model and add
        var model = new ImportTemplate { Name = name, Description = description, FileNameTemplate = ft };
        var obj = _templateService.ImportTemplateFromFile(filePath, model);
        if (obj is null)
        {
            _logger.LogError("Failed to add new import template");
            AnsiConsole.Write(new Markup("[red]添加失败，请查看日志文件获取详情[/]"));
            AnsiConsole.WriteLine();
            return Task.CompletedTask;
        }

        // Finish check
        _logger.LogInformation("Add import template successfully, id is {id}", obj.Id);
        AnsiConsole.Write(new Markup($"[green]添加成功，模版 Id：{obj.Id}[/]"));
        return Task.CompletedTask;
    }
}
