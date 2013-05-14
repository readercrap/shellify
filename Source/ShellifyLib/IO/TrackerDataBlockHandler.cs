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
using System.Runtime.InteropServices;
using System.Text;
using Shellify.IO;
using Shellify.ExtraData;
using Shellify.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Shellify.IO
{
    public class TrackerDataBlockHandler : ExtraDataBlockHandler<TrackerDataBlock>
    {
        public const int ExactBlockSize = 0x60;
        public const int MinimumLength = 0x58;

        public int Length { get; set; }
        public const int MachineIDLength = 16;

        public TrackerDataBlockHandler(TrackerDataBlock item, ShellLinkFile context)
            : base(item, context)
        {
        }

        public override int ComputedSize
        {
            get
            {
                int result = base.ComputedSize +
                             Marshal.SizeOf(Length) +
                             Marshal.SizeOf(Item.Version) +
                             MachineIDLength +
                             Marshal.SizeOf(typeof(Guid)) * 4; 
                return result;
            }
        }

        public override void ReadFrom(System.IO.BinaryReader reader)
        {
            base.ReadFrom(reader);

            FormatChecker.CheckExpression(() => BlockSize == ExactBlockSize);
            
            Length = reader.ReadInt32();
            FormatChecker.CheckExpression(() => Length >= MinimumLength);

            Item.Version = reader.ReadInt32();
            Item.MachineID = reader.ReadASCIIZF(Encoding.Default, MachineIDLength); // 16 bytes, 0 fill
            Item.Droid = new Guid[] { new Guid(reader.ReadBytes(16)), new Guid(reader.ReadBytes(16)) };
            Item.Uuid = new Uuid(Item.Droid[1].ToString());
            Item.DroidBirth = new Guid[] { new Guid(reader.ReadBytes(16)), new Guid(reader.ReadBytes(16)) };
            Item.UuidBirth = new Uuid(Item.DroidBirth[1].ToString());
        }

        public override void WriteTo(System.IO.BinaryWriter writer)
        {
            base.WriteTo(writer);

            FormatChecker.CheckExpression(() => Item.MachineID == null || Item.MachineID.Length <= MachineIDLength);
            FormatChecker.CheckExpression(() => BlockSize == ExactBlockSize);
            //FormatChecker.CheckExpression(() => Item.Droid != null && Item.Droid.Length == 2);
           // FormatChecker.CheckExpression(() => Item.DroidBirth != null && Item.DroidBirth.Length == 2);

            Length = ComputedSize - base.ComputedSize;
            FormatChecker.CheckExpression(() => Length >= MinimumLength);

            writer.Write(Length);
            writer.Write(Item.Version);
            writer.WriteASCIIZF(Item.MachineID, Encoding.Default, MachineIDLength);
            //foreach (Guid guid in Item.Droid) writer.Write(guid.ToByteArray());
            //foreach (Guid guid in Item.DroidBirth) writer.Write(guid.ToByteArray());
        }

    }
}
