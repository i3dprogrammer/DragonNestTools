using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace PakFileTesting
{
    class PakFile
    {
        public string Identifier = "EyedentityGames Packing File 0.1";
        public uint FileCount { get; set; }
        public List<PakFileHeader> Files { get; internal set; }
        private uint FilesOffset { get; set; }
        private string path { get; set; }

        public PakFile(string path)
        {
            this.path = path;
            try
            {
                Files = new List<PakFileHeader>();

                using (var stream = new StreamReader(path))
                {
                    using (var br = new BinaryReader(stream.BaseStream))
                    {

                        if (Encoding.UTF8.GetString(br.ReadBytes(0x20)) != Identifier) //If the first 20 bytes doesn't match our file identifier.
                        {
                            Console.WriteLine("Wrong Identifier detected, aborting the parse.");
                            return;
                        }

                        br.BaseStream.Position = 0x104; // Skipping the unknown data
                        uint FileCount = br.ReadUInt32();
                        uint TableOffsetPosition = br.ReadUInt32(); //Where the files data is.
                        br.BaseStream.Position = TableOffsetPosition; // Jumping to where the file parsing starts.
                        for (int i = 0; i < FileCount; i++)
                        {
                            Files.Add(new PakFileHeader(br));
                        }

                        Console.WriteLine($"There's {FileCount} files in this PAK file!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't open the file {path.Split('\\').Last()} {Environment.NewLine}{ex.Message}");
            }
        }

        public byte[] ExtractFile(PakFileHeader file)
        {
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new BinaryReader(fs))
                {
                    if (file.FileOffset + file.CompressedSize > fs.Length)
                        return new byte[0];
                    fs.Position = file.FileOffset; //Moves the position to the start of the file
                    var data = reader.ReadBytes(Convert.ToInt32(file.CompressedSize)); //Read the compressed data from the stream
                    return ZlibStream.UncompressBuffer(data);
                }
            }
        }

        public bool ImportFile(byte[] data)
        {

        }
    }
}
