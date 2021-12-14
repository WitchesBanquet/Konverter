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

namespace Konverter.Entities;

public class ImportTemplate : IMetadata
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime AddTime { get; set; }
    public string FileNameTemplate { get; set; }
}
