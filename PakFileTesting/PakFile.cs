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
        public string Path { get; set; }
        public string Name
        {
            get
            {
                return Path.Split('\\').Last().Trim('\0');
            }
        }
        public uint OriginalSize { get; set; }
        public uint CompressedSize { get; set; }
        public uint FileOffset { get; set; }
        private uint Unknown { get; set; }
        private int NullDataCount = 10;
        
        public PakFileHeader(BinaryReader reader)
        {
            Path = Encoding.ASCII.GetString(reader.ReadBytes(0x100));
            Path = Path.Trim('\0').Split('\0').First();

            reader.ReadUInt32(); //Raw Size
            OriginalSize = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            FileOffset = reader.ReadUInt32();
            reader.ReadUInt32(); // Unknown
            reader.ReadBytes(sizeof(uint) * NullDataCount); // Padding

            Program.totalSize += OriginalSize;
            Program.totalFiles += 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
