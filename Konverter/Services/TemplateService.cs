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

using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Konverter.Services;

public class TemplateService : ITemplateService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TemplateService> _logger;
    private readonly List<ImportTemplate> _importTemplates = new();
    private readonly List<ExportTemplate> _exportTemplates = new();

    public TemplateService(IConfiguration configuration, ILogger<TemplateService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public List<ExportTemplate> GetExportTemplates()
    {
        Refresh<ExportTemplate>();
        return _exportTemplates;
    }

    public List<ImportTemplate> GetImportTemplates()
    {
        Refresh<ImportTemplate>();
        return _importTemplates;
    }

    public ImportTemplate ImportTemplateFromFile(string filePath, ImportTemplate metadata)
    {
        metadata.Id = Guid.NewGuid();
        metadata.AddTime = DateTime.Now;
        var directory = Path.Combine(_configuration["ImportTemplateDirectory"], metadata.Id.ToString());

        if (filePath is null || File.Exists(filePath) is false)
        {
            _logger.LogWarning("Import template file {file} not found", filePath);
            return null;
        }

        Directory.CreateDirectory(directory);
        File.Copy(filePath, Path.Combine(directory, "import.template"));
        var metaJson = JsonSerializer.Serialize(metadata);
        File.WriteAllText(Path.Combine(directory, "metadata.json"), metaJson);
        return metadata;
    }

    public ExportTemplate ExportTemplateFromFile(string filePath, string indexPath, ExportTemplate metadata)
    {
        metadata.Id = Guid.NewGuid();
        metadata.AddTime = DateTime.Now;
        var directory = Path.Combine(_configuration["ExportTemplateDirectory"], metadata.Id.ToString());

        if (filePath is null || File.Exists(filePath) is false)
        {
            _logger.LogWarning("Import template file {file} not found", filePath);
            return null;
        }

        Directory.CreateDirectory(directory);
        File.Copy(filePath, Path.Combine(directory, "export.liquid"));

        if (metadata.HaveIndex)
        {
            if (indexPath is null || File.Exists(indexPath) is false)
            {
                _logger.LogError("Index template file {file} not found", indexPath);
                Directory.Delete(directory);
                return null;
            }
            File.Copy(indexPath, Path.Combine(directory, "index.liquid"));
        }

        var metaJson = JsonSerializer.Serialize(metadata);
        File.WriteAllText(Path.Combine(directory, "metadata.json"), metaJson);
        return metadata;
    }

    public string ReadImportTemplate(Guid id)
    {
        Refresh<ImportTemplate>();
        var conf = _importTemplates.FirstOrDefault(x => x.Id == id);
        if (conf is null)
        {
            _logger.LogError("Can't find import template with id {id}", id.ToString());
            return null;
        }
        var data = File.ReadAllText(Path.Combine(_configuration["ImportTemplateDirectory"], $"{id.ToString()}/import.template"));
        return data;
    }

    public (string, string) ReadExportTemplate(Guid id)
    {
        Refresh<ExportTemplate>();
        var conf = _exportTemplates.FirstOrDefault(x => x.Id == id);
        if (conf is null)
        {
            _logger.LogError("Can't find export template with id {id}", id.ToString());
            return (null, null);
        }
        var di = Path.Combine(_configuration["ExportTemplateDirectory"], $"{id.ToString()}");
        var ft = File.ReadAllText(Path.Combine(di, "export.liquid"));
        string it = null;
        if (conf.HaveIndex)
        {
            it = File.ReadAllText(Path.Combine(di, "index.liquid"));
        }
        return (ft, it);
    }

    #region Private Methods

    private void Refresh<T>() where T : IMetadata
    {
        var templateDirectory = typeof(T) == typeof(ImportTemplate) ?
            _configuration["ImportTemplateDirectory"] : _configuration["ExportTemplateDirectory"];
        var di = new DirectoryInfo(templateDirectory);
        var tsd = di.GetDirectories();

        if (typeof(T) == typeof(ImportTemplate))
        {
            _importTemplates.Clear();
        }
        else
        {
            _exportTemplates.Clear();
        }

        foreach (var td in tsd)
        {
            var fis = td.GetFiles();
            var conf = fis.FirstOrDefault(x => x.Name == "metadata.json");
            if (conf is null)
            {
                _logger.LogWarning("Can't find metadata.json in {path}", td.FullName);
                continue;
            }
            var stream = File.OpenRead(conf.FullName);
            var metadata = JsonSerializer.Deserialize<T>(stream);
            stream.Close();
            if (typeof(T) == typeof(ImportTemplate))
            {
                _importTemplates.Add(metadata as ImportTemplate);
            }
            else
            {
                _exportTemplates.Add(metadata as ExportTemplate);
            }
        }
    }

    #endregion
}
