using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DNTools.DNT;
using System.Xml.Linq;

namespace DNTools
{
    class Program
    {
        public static Dictionary<string, string> translator = new Dictionary<string, string>();
        public const string LogPath = "DiffFiles.txt";

        static void Log(string text)
        {
            using (var writer = new StreamWriter(LogPath, true))
                writer.WriteLine(text + Environment.NewLine);
        }

        static void TestZlibAlgorithm(string fileName, byte[] data, Func<byte[], byte[]> DecompressAglorithm, Func<byte[], byte[]> CompressAglorithm)
        {
            var folderName = fileName.Split('.').First() + "\\";


            string gameCompressedFilePath = folderName + fileName;
            string decompressedPath = folderName + fileName.Split('.').First() + "_r." + fileName.Split('.').Last();
            string fakeCompressedPath = folderName + fileName.Split('.').First() + "_f." + fileName.Split('.').Last();

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);


            Console.WriteLine("Writing File!");
            File.WriteAllBytes(gameCompressedFilePath, data);
            Console.WriteLine("Decompressing!");
            File.WriteAllBytes(decompressedPath, DecompressAglorithm(File.ReadAllBytes(gameCompressedFilePath)));
            Console.WriteLine("Compressing!");
            File.WriteAllBytes(fakeCompressedPath, CompressAglorithm(File.ReadAllBytes(decompressedPath)));
            Console.WriteLine("Finished!");
        }

        static void Main(string[] args)
        {

            var path = @"F:\Dragon Nest MuSh0 Version\Dragon Nest\Resource03.pak";

            var pak = new PakFile(path);
            //var pak = PakFile.CreatePakFile(path);

            pak.ImportFile(
                @"\resource\uistring\uistring.xml",
                File.ReadAllBytes(@"C:\Users\ahmed\source\repos\PakFileTesting\PakFileTesting\bin\Debug\uistring\uistring_r.xml"),
                File.ReadAllBytes(@"C:\Users\ahmed\source\repos\PakFileTesting\PakFileTesting\bin\Debug\uistring\uistring_checksum.xml"));

            //PakFile.FillNulls(path, 450 * 1024 * 1024);

            //foreach (var file in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Dragon Nest"))
            //{
            //    if (!file.EndsWith(".pak"))
            //        continue;
            //    var pak = new PakFile(file);


            //    if (file.Contains("ui"))
            //    {
            //        foreach (var pakHeader in pak.Files)
            //        {
            //            Console.WriteLine(pakHeader.Path);
            //        }
            //    }

            //    foreach (var pakHeader in pak.Files.Where(x => x.Path.Contains("uistring.xml")))
            //        Console.WriteLine($"{pakHeader.Name}\t{pakHeader.RawSize}\t{pakHeader.CompressedSize}\t{pakHeader.OriginalSize}\t{file.Split('\\').Last()}");
            //}


            //var data = File.ReadAllBytes("uistring_r.xml");


            ////for (int i = 0; i < 160; i++)
            ////{
            ////    pak.ImportFile($@"\resource\filler\uistring{i}.xml", data);
            ////    Console.WriteLine(i);
            ////}

            //pak.ImportFile(@"\resource\uistring\uistring.xml", data);

            //PakFile.FillNulls(path, 450 * 1024 * 1024);

            //var pak = new PakFile(@"F:\Dragon Nest MuSh0 Version\Dragon Nest\Resource03.pak");

            //pak.ImportFile("\\resource\\uistring\\uistring.xml\0xml\0social_2.ani", File.ReadAllBytes(@"uistring_r.xml"));

            //

            //File.WriteAllBytes("halfgolem_stone_c.dds", pak.ExtractFile(@"\resource\char\monster\halfgolem\halfgolem_stone.dds"));
            //File.WriteAllBytes("n2451_re_ray_mf_c.xml", pak.ExtractFile(@"\resource\script\talk_npc_ger\n2451_re_ray_mf.xml"));

            //Decompress("sea_dragon.ani", "sea_dragon_d.ani");
            //Compress("sea_dragon_d.ani", "sea_dragon_c.ani");

            //int i = 0;
            //pak.Files.ForEach(x =>
            //{
            //    var data = pak.ExtractFile(x);
            //    var importedData = (PakFile.ZlibCompress(PakFile.ZlibDecompress(data)));
            //    try
            //    {
            //        bool equal = (data.Length != importedData.Length);
            //        Console.WriteLine($"{data.Length} to {importedData.Length}");
            //        if (!equal)
            //            Log(x.Path);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message); ;
            //    }
            //    Console.WriteLine($"{i++}/{pak.FileCount}");
            //});

            //int i = 0;
            //pak.Files.Where(x => x.Path.Contains("uistring.xml")).ToList().ForEach(x =>
            //{
            //    File.WriteAllBytes(i++ + x.Path.Split('\\').Last(), pak.ExtractFile(x));
            //});

            //var file = pak.ExtractFile(@"\resource\uistring\uistring.xml");

            //Console.WriteLine(pak.Files.Single(x => x.Path == @"\resource\uistring\uistring.xml").CompressedSize);

            //File.WriteAllBytes(@"C:\Users\ahmed\source\repos\PakFileTesting\PakFileTesting\bin\Debug\uistring\uistring_checksum.xml", pak.ExtractFileChecksum(@"\resource\uistring\uistring.xml"));

            //TestZlibAlgorithm("uistring.xml", file, PakFile.ZlibBetterDecompress, PakFile.ZlibBetterCompress);

            //int counter = 0;
            //int i = 0;

            //while(counter < 50)
            //{
            //    var file = pak.Files.ElementAt(i++);
            //    if (!file.MultipleNames)
            //    {
            //        Console.WriteLine(file.Path + "\t" + file.Unknown + "\t" + file.OriginalSize + "\t" + file.RawSize + "\t" + file.CompressedSize);
            //        TestZlibAlgorithm(file.Name, pak.ExtractFile(file), PakFile.ZlibBetterDecompress, PakFile.ZlibBetterCompress);
            //        counter++;
            //    }
            //}

            //File.WriteAllBytes("sea_dragon_r.ani", PakFile.ZlibBetterDecompress(File.ReadAllBytes("sea_dragon.ani")));
            //File.WriteAllBytes("sea_dragon_f.ani", PakFile.ZlibBetterCompress(File.ReadAllBytes("sea_dragon_r.ani")));
            //Console.WriteLine(file.MultipleNames);
            //var f = new ZInputStream();
            //File.WriteAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\CompareUIStrings\fakeUIString.xml",
            //    ZlibStream.UncompressBuffer(File.ReadAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\CompareUIStrings\fakeUIStringCompressed.xml")));

            return;

            //pak.ImportFile(@"\resource\sound\magdonia\test.fu",
            //    File.ReadAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource\sound\magdonia\test.fu"));

            //foreach (var f in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource", "*.*", SearchOption.AllDirectories))
            //{
            //    var pakSingle = new PakFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\Resource01-ClerJap-Single.pak");
            //    var pathSingle = f.Replace(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest", "");
            //    pakSingle.ImportFile(pathSingle, File.ReadAllBytes(f));
            //    Console.WriteLine($"Imported {pathSingle}");
            //}

            //Console.WriteLine("Single file importing finished.");
            //Console.WriteLine("Starting mutlifile importing.");

            //var pakMulti = new PakFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\Resource01-ClerJap-Multi.pak");

            //foreach (var f in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource", "*.*", SearchOption.AllDirectories))
            //{
            //    var pathMulti = f.Replace(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest", "");
            //    pakMulti.ImportFile(pathMulti, File.ReadAllBytes(f));
            //    Console.WriteLine($"Imported {pathMulti}");
            //}

            //Console.WriteLine("Multifile importing finished.");

            //pak.ImportFile(@"\resource\char\monster\academic_tower\testerino.act",
            //    File.ReadAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\Resource16\resource\char\monster\academic_tower\testerino.act"));

            //pak.Files.ToList().ForEach(x =>
            //{
            //    if (x.Path.Contains(@"\char\monster\academic_tower"))
            //        Console.WriteLine(x.Path);
            //});

            //var file = pak.Files.First(x => x.Path.Contains("testerino"));
            //var data = pak.ExtractFile(file);
            //using (var fs = File.OpenWrite(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\Resource16\resource\char\monster\academic_tower\test.act"))
            //    fs.Write(data, 0, data.Length);

            //pak.Files.ForEach(x =>
            //{
            //    FileInfo info = new FileInfo(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\Resource1" + x.Path);
            //    if (!info.Directory.Exists)
            //        Directory.CreateDirectory(info.Directory.FullName);

            //    var data = pak.ExtractFile(x);
            //    using (var fs = File.OpenWrite(info.FullName))
            //        fs.Write(data, 0, data.Length);
            //});

            //Console.WriteLine($"{pak.Files[0].RawSize}, {pak.Files[0].OriginalSize}, {pak.Files[0].CompressedSize}");
            //var doc = XDocument.Load(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\uistring\uistring.xml");
            //doc.Root.Descendants("message").ToList().ForEach(x =>
            //{
            //    translator.Add(x.Attribute("mid").Value, x.Value);
            //});
            //Console.WriteLine("Loaded translation files!");

            //var dnt = new DntFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\ext\monstertable_nest.dnt");
            //var dnt = new DntFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\ext\itemdroptable_labyrinth.dnt");
            //var dnt = new DntFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\ext\monstertable.dnt");
            //Console.WriteLine("Saved monstertable!");

            //foreach (var file in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\ext\"))
            //{
            //    try
            //    {
            //        if (file.EndsWith(".dnt"))
            //            new DntFile(file);
            //    } catch (Exception)
            //    {
            //        Console.WriteLine(file.Split('\\').Last());
            //    }
            //}

            //dnt.WriteXml(new StreamWriter(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\monstertable_nest.xml"));
        }
    }
}
