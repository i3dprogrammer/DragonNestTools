﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNTools
{
    class PakFileHeader
    {
        /// <summary>
        /// Full path of the file containing the file name.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Raw file size.
        /// </summary>
        public uint RawSize { get; set; }

        /// <summary>
        /// Uncompressed file size.
        /// </summary>
        public uint OriginalSize { get; set; }

        /// <summary>
        /// Compressed file size.
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// File offset in the Pak file.
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// This header offset from beginning.
        /// </summary>
        public long HeaderOffset { get; set; }

        /// <summary>
        /// File name.
        /// </summary>
        public string Name
        {
            get
            {
                return Path.Split('\\').Last().Trim('\0');
            }
        }

        public bool MultipleNames { get; set; } = false;

        public uint Unknown { get; set; }
        private const int NullDataCount = 10;

        public PakFileHeader()
        {

        }

        /// <summary>
        /// Parses the file header
        /// <para>Note that the reader has to be at the correct offset.</para>
        /// </summary>
        /// <param name="reader">BinaryReader to read the data from.</param>
        public PakFileHeader(BinaryReader reader)
        {
            HeaderOffset = reader.BaseStream.Position;

            Path = Encoding.ASCII.GetString(reader.ReadBytes(0x100)); //Path is always 0x100 bytes long
            MultipleNames = (Path.Trim('\0').Split('\0').Count() != 1);
            Path = Path.Trim('\0').Split('\0').First(); //Trim the null bytes

            RawSize = reader.ReadUInt32();
            OriginalSize = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            FileOffset = reader.ReadUInt32();
            Unknown = reader.ReadUInt32(); // Unknown
            reader.ReadBytes(sizeof(uint) * NullDataCount); // Padding
        }

        public PakFileHeader(string path, uint rawSize, uint originalSize, uint compressedSize, uint fileOffset, long headerOffset)
        {
            Path = path;
            RawSize = rawSize;
            OriginalSize = originalSize;
            CompressedSize = compressedSize;
            FileOffset = fileOffset;
            HeaderOffset = headerOffset;
        }

        public static byte[] CreateFileHeaderData(string path, uint rawSize, uint originalSize, uint compressedSize, uint fileOffset)
        {
            byte[] fullBytes = new byte[0x100];
            var pathBytes = Encoding.ASCII.GetBytes(path);
            Array.Copy(pathBytes, fullBytes, pathBytes.Length);

            using (var writer = new MemoryStream())
            {
                writer.Write(fullBytes, 0, fullBytes.Length);
                writer.Write(BitConverter.GetBytes(rawSize), 0, 4);
                writer.Write(BitConverter.GetBytes(originalSize), 0, 4);
                writer.Write(BitConverter.GetBytes(compressedSize), 0, 4);
                writer.Write(BitConverter.GetBytes(fileOffset), 0, 4);
                writer.Write(new byte[4] { 0x00, 0x00, 0x00, 0x00 }, 0, 4);
                writer.Write(new byte[sizeof(uint) * NullDataCount], 0, 40);
                writer.Flush();

                return writer.ToArray();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
