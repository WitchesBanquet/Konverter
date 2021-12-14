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

namespace Konverter.Utilities;

public static class EnumMenuItemExtension
{
    public static (int, string) GetMenuInfo(this Enum e)
    {
        var fi = e.GetType().GetField(e.ToString());
        if (fi is null)
        {
            throw new ArgumentNullException(nameof(fi), "Enum field info is null.");
        }
        if (fi?.GetCustomAttribute(typeof(MenuItemAttribute), false) is MenuItemAttribute attribute)
        {
            return (attribute.Index, attribute.Description);
        }
        throw new ArgumentNullException(nameof(attribute),
            $"Can not get MenuItemAttribute on field value {e.ToString()} in type {e.GetType()}");
    }
}
