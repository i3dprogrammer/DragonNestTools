using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace PakFileTesting
{
    /// <summary>
    /// A simple Pak file parses
    /// </summary>
    class PakFile
    {
        public string Identifier = "EyedentityGames Packing File 0.1";
        public uint FileCount { get; set; }
        public List<PakFileHeader> Files { get; internal set; }
        private uint FilesOffset { get; set; }
        private string path { get; set; }

        /// <summary>
        /// Start parsing the Pak file.
        /// </summary>
        /// <param name="path">The path to the Pak file.</param>
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

        /// <summary>
        /// Extracts a file from the PAK file.
        /// </summary>
        /// <param name="file">PakFileHeader to get the file size and offset from.</param>
        /// <returns>A byte array containing the uncompressed data.</returns>
        public byte[] ExtractFile(PakFileHeader file)
        {
            if (!Files.Contains(file))
                return new byte[0];

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

        /// <summary>
        /// Imports a file into a PAK file.
        /// </summary>
        /// <param name="data">The bytes to import in the PAK.</param>
        /// <param name="path">The path containing the file name and where to import the file to.</param>
        /// <returns>A bool specifying whether or not the importing succeeded.</returns>
        public bool ImportFile(string path, byte[] data)
        {
            return false;
        }
    }
}
