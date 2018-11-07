﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DNTools.DNT;
using System.Xml.Linq;
using Ionic.Zlib;

namespace DNTools
{
    class Program
    {
        public static Dictionary<string, string> translator = new Dictionary<string, string>();
        static void Main(string[] args)
        {

            //pak.ImportFile(@"\resource\sound\magdonia\test.fu",
            //    File.ReadAllBytes(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource\sound\magdonia\test.fu"));

            foreach (var f in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource", "*.*", SearchOption.AllDirectories))
            {
                var pakSingle = new PakFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\Resource01-ClerJap-Single.pak");
                var pathSingle = f.Replace(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest", "");
                pakSingle.ImportFile(pathSingle, File.ReadAllBytes(f));
                Console.WriteLine($"Imported {pathSingle}");
            }

            Console.WriteLine("Single file importing finished.");
            Console.WriteLine("Starting mutlifile importing.");

            var pakMulti = new PakFile(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\Resource01-ClerJap-Multi.pak");

            foreach (var f in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest\resource", "*.*", SearchOption.AllDirectories))
            {
                var pathMulti = f.Replace(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\ImportingTest", "");
                pakMulti.ImportFile(pathMulti, File.ReadAllBytes(f));
                Console.WriteLine($"Imported {pathMulti}");
            }

            Console.WriteLine("Multifile importing finished.");

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
