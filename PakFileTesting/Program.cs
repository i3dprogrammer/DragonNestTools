using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DragonNestTools.DNT;
using System.Xml.Linq;

namespace DragonNestTools
{
    class Program
    {
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
    }
}
