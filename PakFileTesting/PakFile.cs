using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace DNTools
{
    /// <summary>
    /// A simple Pak file parses
    /// </summary>
    class PakFile
    {
        public const string Identifier = "EyedentityGames Packing File 0.1";
        public uint FileCount { get; private set; }
        public List<PakFileHeader> Files { get; private set; }
        private uint HeaderFilesOffset { get; set; }
        //private byte[] PakData;
        private string PakFilePath;
        /// <summary>
        /// Start parsing the Pak file.
        /// </summary>
        /// <param name="path">The path to the Pak file.</param>
        public PakFile(string path)
        {
            try
            {
                PakFilePath = path;
                //PakData = File.ReadAllBytes(path);
                Files = new List<PakFileHeader>();

                using (var stream = new StreamReader(PakFilePath))
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
                        HeaderFilesOffset = br.ReadUInt32(); // Where the file headers are.
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
                Console.WriteLine($"Couldn't open the file {path.Split('\\').Last()} {Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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

            using (var fs = File.Open(PakFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
        /// Imports a file into a PAK file
        /// </summary>
        /// <param name="fileData">The bytes to import in the PAK.</param>
        /// <param name="pathInPak">The path containing the file name and where to import the file to.</param>
        /// <returns>A bool specifying whether or not the importing succeeded.</returns>
        public bool ImportFile(string pathInPak, byte[] fileData)
        {
            try
            {
                PakFileHeader existingFile = null;
                var compressedData = ZlibStream.CompressBuffer(fileData);

                if (Files.Exists(x => x.Path == pathInPak))
                    existingFile = Files.First(x => x.Path == pathInPak);
                else
                    FileCount++;

                var headerData = PakFileHeader.CreateFileHeaderData(pathInPak, (uint)compressedData.Length, (uint)fileData.Length, HeaderFilesOffset);
                using (var fs = File.Open(PakFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    //Copy the FileHeaders so we can overwrite it with the compressed file's data.
                    fs.Seek(HeaderFilesOffset, SeekOrigin.Begin);
                    byte[] shiftedData = new byte[fs.Length - HeaderFilesOffset];
                    fs.Read(shiftedData, 0, shiftedData.Length);

                    //Write the compressed files data at the end of ALL THE FILES DATA
                    fs.Seek(HeaderFilesOffset, SeekOrigin.Begin);
                    fs.Write(compressedData, 0, compressedData.Length);

                    //Write the copied file headers
                    fs.Write(shiftedData, 0, shiftedData.Length);

                    //Write the new file header
                    if (existingFile != null)
                        fs.Seek(existingFile.HeaderOffset + compressedData.Length, SeekOrigin.Begin);

                    fs.Write(headerData, 0, headerData.Length);

                    //Write the new/old file count
                    fs.Seek(0x104, SeekOrigin.Begin);
                    var fileCount = BitConverter.GetBytes(FileCount);
                    fs.Write(fileCount, 0, fileCount.Length);

                    //Write the new HeaderFileOffset (it will get changed if we added more files)
                    var headerFilesOffset = BitConverter.GetBytes(HeaderFilesOffset + compressedData.Length);
                    fs.Write(headerFilesOffset, 0, headerFilesOffset.Length);

                    //Add the new FileHeader to the Files list.
                    if (existingFile != null)
                    {
                        Files.Add(new PakFileHeader(
                            pathInPak,
                            (uint)fileData.Length,
                            (uint)compressedData.Length,
                            HeaderFilesOffset,
                            existingFile.HeaderOffset));
                        Files.Remove(existingFile);
                    }
                    else
                    {
                        Files.Add(new PakFileHeader(
                            pathInPak,
                            (uint)fileData.Length,
                            (uint)compressedData.Length,
                            HeaderFilesOffset,
                            HeaderFilesOffset + compressedData.Length + shiftedData.Length));
                    }
                }

                //Shift the HeaderFilesOffset by the new file margin.
                HeaderFilesOffset += (uint)compressedData.Length; 
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public static bool CreatePakFile(string saveDestination)
        {
            try
            {
                using(var fs = File.Open(saveDestination, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fs.Write(Encoding.ASCII.GetBytes(Identifier), 0, Identifier.Length); //File Identifier
                    fs.Write(new byte[0xE4], 0, 0xE4); //Null bytes
                    fs.Write(BitConverter.GetBytes((uint)0), 0, 4); //Files count initialized 0
                    fs.Write(BitConverter.GetBytes((uint)fs.Length + 0x2F4 + 4), 0, 4); //HeaderFileOffset
                    fs.Write(new byte[0x2F4], 0, 0x2F4);
                }
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
                return false;
            }
        }
    }
}
