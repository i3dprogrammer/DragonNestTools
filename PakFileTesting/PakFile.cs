using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace DragonNestTools
{
    /// <summary>
    /// A simple Pak file parses
    /// </summary>
    class PakFile
    {
        public string Identifier = "EyedentityGames Packing File 0.1";
        public uint FileCount { get; private set; }
        public List<PakFileHeader> Files { get; private set; }
        private uint HeaderFilesOffset { get; set; }
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
                        FileCount = br.ReadUInt32();
                        HeaderFilesOffset = br.ReadUInt32(); //Where the files data is.
                        br.BaseStream.Position = HeaderFilesOffset; // Jumping to where the file header parsing starts.
                        for (int i = 0; i < FileCount; i++)
                        {
                            Files.Add(new PakFileHeader(br));
                        }
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

            using (var fs = File.Open(this.path, FileMode.Open, FileAccess.Read, FileShare.Read))
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
        /// <param name="fileData">The bytes to import in the PAK.</param>
        /// <param name="path">The path containing the file name and where to import the file to.</param>
        /// <returns>A bool specifying whether or not the importing succeeded.</returns>
        public bool ImportFile(string path, byte[] fileData)
        {
            PakFileHeader existingFile = null;
            var compressedData = ZlibStream.CompressBuffer(fileData);

            if (Files.Exists(x => x.Path == path))
                existingFile = Files.First(x => x.Path == path);
            else
                FileCount++;
            
            var pakData = File.ReadAllBytes(this.path); //Get PAK data
            byte[] buffer = null;

            if (existingFile == null)
                buffer = new byte[pakData.Length + compressedData.Length + 0x13C]; //Create a new buffer with extra space for the file data & the header!
            else
                buffer = new byte[pakData.Length + compressedData.Length]; //Create a new buffer with extra space for the file data only!

            pakData.CopyTo(buffer, 0);

            //Shift the file headers by compressed data length so we can add the compressed data before it starts.
            Array.Copy(buffer, HeaderFilesOffset, buffer, HeaderFilesOffset + compressedData.Length, pakData.Length - HeaderFilesOffset);

            //Add the compressed data at the OLD header files offset.
            Array.Copy(compressedData, 0, buffer, HeaderFilesOffset, compressedData.Length);

            using (var fs = new MemoryStream(buffer))
            {
                //Create the file header data
                var headerData = PakFileHeader.CreateFileHeader(path, (uint)compressedData.Length, (uint)fileData.Length, HeaderFilesOffset);

                if (existingFile == null)
                    fs.Seek(-0x13C, SeekOrigin.End); //Seek to the end of the Pak, so we can add the new header file.
                else
                    fs.Seek(existingFile.HeaderOffset + compressedData.Length, SeekOrigin.Begin); //Seek to the start of the old header file to overwrite.

                fs.Write(headerData, 0, headerData.Length); //Write the header file

                //Write the new/old file count
                fs.Seek(0x104, SeekOrigin.Begin);
                var fileCount = BitConverter.GetBytes(FileCount);
                fs.Write(fileCount, 0, fileCount.Length);

                //Write the new HeaderFileOffset (it will get changed if we added more files)
                var headerFilesOffset = BitConverter.GetBytes(HeaderFilesOffset + compressedData.Length);
                fs.Write(headerFilesOffset, 0, headerFilesOffset.Length);
            }

            File.WriteAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\Resource16\Resource18.pak", buffer);
            //Return true for now.
            return true;
        }
    }
}
