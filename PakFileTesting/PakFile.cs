using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakFileTesting
{
    class PakFileHeader
    {
        /// <summary>
        /// Full path of the file containing the file name.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The file name.
        /// </summary>
        public string Name
        {
            get
            {
                return Path.Split('\\').Last().Trim('\0');
            }
        }

        /// <summary>
        /// Uncompressed file size.
        /// </summary>
        public uint OriginalSize { get; set; }

        /// <summary>
        /// Compressed file size.
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// The file offset in the Pak file.
        /// </summary>
        public uint FileOffset { get; set; }

        private uint Unknown { get; set; }
        private int NullDataCount = 10;
        
        /// <summary>
        /// Parses the file header
        /// <para>Note that the reader has to be at the correct offset.</para>
        /// </summary>
        /// <param name="reader">BinaryReader to read the data from.</param>
        public PakFileHeader(BinaryReader reader)
        {
            Path = Encoding.ASCII.GetString(reader.ReadBytes(0x100)); //Path is always 0x100 bytes long
            Path = Path.Trim('\0').Split('\0').First(); //Trim the null bytes

            reader.ReadUInt32(); //Raw Size
            OriginalSize = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            FileOffset = reader.ReadUInt32();
            reader.ReadUInt32(); // Unknown
            reader.ReadBytes(sizeof(uint) * NullDataCount); // Padding
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
