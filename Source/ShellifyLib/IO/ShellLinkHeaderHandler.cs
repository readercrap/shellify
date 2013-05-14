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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shellify.Core;

namespace Shellify.IO
{
	public class ShellLinkHeaderHandler : IBinaryReadable, IBinaryWriteable
	{
        public const int ExactHeaderSize = 0x4C;

		private ShellLinkHeader Item { get; set; }
		private int HeaderSize { get; set; }
		private const int ReservedSize = 10;

        public ShellLinkHeaderHandler(ShellLinkHeader item)
		{
			this.Item = item;
		}
		
		public void ReadFrom(BinaryReader reader)
		{
            HeaderSize = reader.ReadInt32();
            FormatChecker.CheckExpression(() => HeaderSize == ExactHeaderSize);
			
			Item.Guid = new Guid(reader.ReadBytes(16));
            FormatChecker.CheckExpression(() => Item.Guid.ToString().Equals(ShellLinkHeader.LNKGuid) );

			Item.LinkFlags = (LinkFlags) (reader.ReadInt32());
			Item.FileAttributes = (FileAttributes) (reader.ReadInt32());

            try
            {
                Item.CreationTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
            }
            catch (Exception) { }

            try
            {
                Item.AccessTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
            }
            catch (Exception) { }

            try
            {
                Item.WriteTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
            }
            catch (Exception) { }

			Item.FileSize = reader.ReadInt32();
			Item.IconIndex = reader.ReadInt32();
			
			Item.ShowCommand = (ShowCommand) reader.ReadInt32();
			Item.HotKey = (Keys) reader.ReadInt16();
			
			reader.ReadBytes(ReservedSize); // Reserved
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			HeaderSize = Marshal.SizeOf(HeaderSize) +
			Marshal.SizeOf(Item.Guid) +
			Marshal.SizeOf(Item.FileSize) +
			Marshal.SizeOf(Item.IconIndex) +
			1 * Marshal.SizeOf(typeof(short)) +
			3 * Marshal.SizeOf(typeof(int)) +
			3 * Marshal.SizeOf(typeof(long)) +
			ReservedSize;

            FormatChecker.CheckExpression(() => HeaderSize == ExactHeaderSize);
            FormatChecker.CheckExpression(() => Item.Guid.ToString().Equals(ShellLinkHeader.LNKGuid));
            
            writer.Write(HeaderSize);
			writer.Write(Item.Guid.ToByteArray());
			writer.Write((int) Item.LinkFlags);
			writer.Write((int) Item.FileAttributes);
			
			writer.Write(Item.CreationTime.ToFileTime());
			writer.Write(Item.AccessTime.ToFileTime());
			writer.Write(Item.WriteTime.ToFileTime());
			
			writer.Write(Item.FileSize);
			writer.Write(Item.IconIndex);
			
			writer.Write((int) Item.ShowCommand);
			writer.Write((short) Item.HotKey);
			
			byte[] reserved = new byte[ReservedSize];
			writer.Write(reserved); // Reserved
		}
		
	}
}
