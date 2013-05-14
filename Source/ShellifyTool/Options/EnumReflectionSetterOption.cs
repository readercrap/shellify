/*
    Shellify, .NET implementation of Shell Link (.LNK) Binary File Format
    Copyright (C) 2010 Sebastien LEBRETON

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Shellify.Tool.Commands;
using System.Globalization;
using System.Reflection;
using Shellify.Core;

namespace Shellify.Tool.Options
{
    class EnumReflectionSetterOption : ReflectionSetterOption
    {

        Type EnumType { get; set; }

        public EnumReflectionSetterOption(string tag, string description, IList<Command> applies, Type enumType, params string[] propertyPath)
            : base(tag, description, applies, propertyPath)
        {
            PropertyPath = propertyPath;
            EnumType = enumType;
        }

        public override object ChangeType(object source, Type targetType)
        {
            return System.Enum.Parse(targetType, source.ToString());
        }

        public override void Execute(ShellLinkFile context)
        {
            Argument = Argument.Replace(" ",string.Empty);
            base.Execute(context);
        }

    }
}