using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PakFileTesting.DNT
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
                    //Status = Convert.ToInt32(decimal.Divide(stream.Position, stream.Length) * 100);
                    int columnCount = reader.ReadUInt16();
                    uint rowCount = reader.ReadUInt32();
                    //Console.WriteLine($"Columns {columnCount}, Rows {rowCount}");
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

                    //using (var writer = new StreamWriter("debugData", true))
                    //{
                    //    foreach (var col in Columns)
                    //        writer.Write(col.ToString() + "\t");
                    //    writer.WriteLine();
                    //}

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

                        //if (Columns.Contains("_NameIDParam"))
                        //{
                        //    current["_NameIDParam"] = current["_NameIDParam"].ToString().Replace("{", "").Replace("}", "");
                        //    if (!current["_NameIDParam"].ToString().Contains(",") && Program.translator.ContainsKey(current["_NameIDParam"].ToString()))
                        //        current["_NameIDParam"] = Program.translator[current["_NameIDParam"].ToString()];
                        //    if (current["_NameIDParam"].ToString().Contains(","))
                        //    {
                        //        var values = current["_NameIDParam"].ToString();
                        //        current["_NameIDParam"] = "";
                        //        values.Split(',').ToList().ForEach(x =>
                        //        {
                        //            if (Program.translator.ContainsKey(x))
                        //            {
                        //                current["_NameIDParam"] += Program.translator[x] + ", ";
                        //            }
                        //        });
                        //        if (current["_NameIDParam"].ToString().Length > 0)
                        //            current["_NameIDParam"] = current["_NameIDParam"].ToString().Substring(0, current["_NameIDParam"].ToString().Length - 1);
                        //    }
                        //}

                        Rows.Add(current);

                        //using (var writer = new StreamWriter("debugData", true))
                        //    writer.WriteLine(Rows[Rows.Count - 1].ItemArray.Aggregate((x, y) => $"{x}\t{y}"));

                    }
                }
            }
        }
    }
}
