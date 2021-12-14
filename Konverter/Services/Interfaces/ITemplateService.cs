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

namespace Konverter.Services.Interfaces;

public interface ITemplateService
{
    List<ExportTemplate> GetExportTemplates();
    List<ImportTemplate> GetImportTemplates();
    ImportTemplate ImportTemplateFromFile(string filePath, ImportTemplate metadata);
    ExportTemplate ExportTemplateFromFile(string filePath, string indexTemplate, ExportTemplate metadata);
    string ReadImportTemplate(Guid id);
    (string, string) ReadExportTemplate(Guid id);
}
