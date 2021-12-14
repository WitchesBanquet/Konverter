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

namespace Konverter.Enums;

public enum Functions
{
    [MenuItem(1, "开始新的操作")]
    NewSession,

    [MenuItem(2, "添加新的导入模版")]
    AddNewImportTemplate,

    [MenuItem(3, "添加新的导出模版")]
    AddNewExportTemplate,

    [MenuItem(4, "退出")]
    Exit
}
