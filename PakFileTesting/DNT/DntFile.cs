using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNTools.DNT
{
    class DntFile : DataTable
    {
        public DntFile(string path) : base()
        {
            TableName = path.Split('\\').Last().Split('.').First();
            using (var stream = new StreamReader(path).BaseStream)
            {
                Columns.Add(new DataColumn("RowId", typeof(uint)));
                stream.Position = 4L;

                using (var reader = new BinaryReader(stream))
                {
                    int columnCount = reader.ReadUInt16();
                    uint rowCount = reader.ReadUInt32();
                    for (int i = 0; i < columnCount; i++)
                    {
                        uint length = reader.ReadUInt16();
                        var name = new string(reader.ReadChars((int)length));
                        switch (reader.ReadByte())
                        {
                            case 1:
                                Columns.Add(new DataColumn(name, typeof(string)));
                                break;
                            case 2:
                                Columns.Add(new DataColumn(name, typeof(bool)));
                                break;
                            case 3:
                                Columns.Add(new DataColumn(name, typeof(int)));
                                break;
                            case 4:
                                Columns.Add(new DataColumn(name, typeof(float)));
                                break;
                            case 5:
                                Columns.Add(new DataColumn(name, typeof(double)));
                                break;
                            default:
                                throw new FormatException("stream is not in the correct format");
                        }
                    }

                    try
                    {
                        for (int i = 0; (ulong)i < (ulong)rowCount; i++)
                        {
                            DataRow current = NewRow();
                            var pos = reader.BaseStream.Position;
                            for (int j = 0; j <= columnCount; j++)
                            {
                                if (Columns[j].DataType == typeof(uint))
                                    current[Columns[j].ColumnName] = reader.ReadUInt32();
                                if (Columns[j].DataType == typeof(string))
                                {
                                    var length = reader.ReadInt16();
                                    if (length == 0x3F && (new string[] { "_ImmuneReduceTime", "_ImmunePercent" }).Contains(Columns[j].ColumnName))
                                    {
                                        //Console.WriteLine(i + " " + pos);
                                        while (true)
                                        {
                                            var bits = Encoding.ASCII.GetString(reader.ReadBytes(0x01));
                                            if (!new string[] { "1", "0", ";" }.Contains(bits))
                                                break;
                                            current[Columns[j].ColumnName] += bits;
                                        }
                                        reader.BaseStream.Position -= 1;
                                    }
                                    else
                                    {
                                        if (current[Columns[j].ColumnName].ToString() == "")
                                            current[Columns[j].ColumnName] = Encoding.ASCII.GetString(reader.ReadBytes(length));
                                    }
                                }
                                if (Columns[j].DataType == typeof(bool))
                                    current[Columns[j].ColumnName] = reader.ReadInt32();
                                if (Columns[j].DataType == typeof(int))
                                    current[Columns[j].ColumnName] = reader.ReadInt32();
                                if (Columns[j].DataType == typeof(float))
                                    current[Columns[j].ColumnName] = reader.ReadSingle();
                                if (Columns[j].DataType == typeof(double))
                                    current[Columns[j].ColumnName] = reader.ReadSingle();
                            }

                            Rows.Add(current);
                        }
                    }
                    catch { }

                    if(Rows.Count != rowCount)
                        Console.WriteLine($"{Rows.Count}/{rowCount}");
                }
            }
        }


        /// <summary>
        /// Exports the table as CSV readable format.
        /// </summary>
        /// <param name="path">Where to export, the full path containing the file name with the extension e.g. "table.csv"</param>
        public void ExportAsCSV(string path)
        {
            if (File.Exists(path))
                throw new Exception($"{path} already exists, exporting CSV failed.");

            string columns = Columns.Cast<DataColumn>().Select(x => x.ColumnName).Aggregate((x, y) => x + "," + y);

            using(var writer = new StreamWriter(path))
            {
                writer.WriteLine(columns);

                Rows.Cast<DataRow>().ToList().ForEach(z =>
                {
                    writer.WriteLine(z.ItemArray.Select(x => x.ToString().Replace(",", ";")).Aggregate((x, y) => x + "," + y));
                });
            }
        }
    }
}
