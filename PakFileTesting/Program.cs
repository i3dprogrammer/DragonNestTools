using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PakFileTesting.DNT;
using System.Xml.Linq;

namespace PakFileTesting
{
    class Program
    {
        public static ulong totalSize = 0;
        public static int totalFiles = 0;

        private static ulong extractedSize = 0;
        private static int extractedFiles = 0;
        public static Dictionary<string, string> translator = new Dictionary<string, string>();
        static void Main(string[] args)
        {
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
            foreach (var file in Directory.GetFiles(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\resource\ext\"))
            {
                try
                {
                    if (file.EndsWith(".dnt"))
                        new DntFile(file);
                } catch (Exception)
                {
                    Console.WriteLine(file.Split('\\').Last());
                }
            }
            //dnt.WriteXml(new StreamWriter(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files\OldPatch\monstertable_nest.xml"));
        }

        static void LoadPaks()
        {
            var DNFolder = @"F:\Dragon Nest MuSh0 Version\Dragon Nest\";

            foreach (var pakPath in Directory.GetFiles(DNFolder))
            {
                if (pakPath.EndsWith(".pak"))
                {
                    totalSize = 0;
                    totalFiles = 0;
                    PakFile pak = new PakFile(pakPath);
                    extractedSize = 0;
                    extractedFiles = 0;

                    foreach (var pakFile in pak.Files)
                    {
                        try
                        {
                            FileInfo fInfo = new FileInfo(@"F:\Dragon Nest MuSh0 Version\Extracted PAK files" + pakFile.Path);
                            extractedFiles++;
                            extractedSize += pakFile.OriginalSize;

                            if (fInfo.Exists || !fInfo.FullName.Contains("uistring"))
                                continue;

                            if (!Directory.Exists(fInfo.DirectoryName))
                            {
                                Directory.CreateDirectory(fInfo.DirectoryName);
                            }

                            using (var writer = new StreamWriter(fInfo.FullName, false))
                            {
                                var data = pak.GetUncompressedPakFile(pakFile);
                                writer.Write(Encoding.ASCII.GetChars(data));
                            }
                            Console.WriteLine(fInfo.Name);

                            Console.WriteLine($"Extracted files {extractedFiles}/{totalFiles} from PAK {pakPath.Split('\\').Last()}");
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(pakFile.Path + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
