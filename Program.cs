using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace EU4BAC
{
    class Program
    {
        static readonly int Height = 2048;
        static readonly int Width = 2048;
        static readonly int Reso = 64;
        static readonly int PixelPerProvince = 32;
        static readonly Color Tree = Color.FromArgb(100, 100, 100);
        static Color Farmlands = Color.FromArgb(179, 255, 64);
        static string Dir = AppDomain.CurrentDomain.BaseDirectory;
        static List<List<Color>> cMatrix = new List<List<Color>>();
        static void CreateProvinces()
        {
            Bitmap ret = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < Width; i += PixelPerProvince)
            {
                cMatrix.Add(new List<Color>());
                for (int j = 0; j < Height; j += PixelPerProvince)
                {
                    Color c = Color.FromArgb((i + j) % 255 + 1, 3 * (i / PixelPerProvince) + 1, 3 * (j / PixelPerProvince) + 1);
                    for (int k1 = 0; k1 < PixelPerProvince; k1++)
                    {
                        for (int k2 = 0; k2 < PixelPerProvince; k2++)
                        {
                            ret.SetPixel(i + k1, j + k2, c);
                        }
                    }
                    cMatrix[cMatrix.Count-1].Add(c);
                }
            }
            ret.Save("provinces.bmp", ImageFormat.Bmp);
        }
        static void CreateTerrain()
        {
            Bitmap raw = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j ++)
                {
                    raw.SetPixel(i, j, Farmlands);
                }
            }
            Bitmap ret = raw.Clone(new Rectangle(0,0,Width,Height),PixelFormat.Format8bppIndexed);            
            ret.Save("terrain.bmp", ImageFormat.Bmp);
        }
        static void CreateTrees()
        {
            Bitmap raw = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    raw.SetPixel(i, j, Tree);
                }
            }
            Bitmap ret = raw.Clone(new Rectangle(0, 0, Width, Height), PixelFormat.Format8bppIndexed);

            ret.Save("trees.bmp", ImageFormat.Bmp);
        }
        static void CreateRivers()
        {
            Bitmap raw = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            int hg = 960;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    raw.SetPixel(i, j, Color.FromArgb(255,255,255));
                }
            }
            for (int i = 0; i < Width; i++)
            {
                raw.SetPixel(i, hg, Color.FromArgb(0, 225, 255));
            }
            raw.SetPixel(0, 960, Color.FromArgb(0, 255, 0));
            Bitmap ret = raw.Clone(new Rectangle(0, 0, Width, Height), PixelFormat.Format8bppIndexed);

            ret.Save("rivers.bmp", ImageFormat.Bmp);
        }
        static void OpenPicture(string pn)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(pn)
            {
                UseShellExecute = true
            };
            p.Start();
        }
        static string ToId(int x, int y)
        {
            var codes =new string[] { "ZER", "ONE", "TWO", "THR", "FO", "FIV", "SIX", "SEV", "EIG", "NIN" };
            var b = x.ToString().PadLeft(2, '0');
            var c = y.ToString().PadLeft(2, '0');
            return codes[b[0] - 48] + codes[b[1] - 48] + codes[c[0] - 48] + codes[c[1] - 48];
        }
        static void CreateDefinitions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("province;red;green;blue;x;x");
            int t = 1;
            for (int i = 0; i < Reso; i++)
            {
                for (int j = 0; j < Reso; j++)
                {
                    var u = cMatrix[i][j];
                    sb.AppendLine($"{t};{u.R};{u.G};{u.B};{ToId(i,j)};x");
                    t++;
                }
            }
            FileStream fs = new FileStream("definitions.csv", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        static void CreateHeight()
        {
            Bitmap raw = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    raw.SetPixel(i, j, Color.FromArgb(100, 100, 100));
                }
            }
            Bitmap ret = raw.Clone(new Rectangle(0, 0, Width, Height), PixelFormat.Format8bppIndexed);
            ret.Save("heightmap.bmp");
        }
        static void CreateNormalMap()
        {
            Bitmap ret = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < Width; i ++)
            {
                for (int j = 0; j < Height; j ++)
                {
                    ret.SetPixel(i, j, Color.FromArgb(150, 150, 150));
                }
            }
            ret.Save("world_normal.bmp", ImageFormat.Bmp);
        }
        private static void CreateHistory()
        {
            List<Tuple<int, int>> diffs = new List<Tuple<int, int>>();
            var inputDir = "E:\\fmod\\parsed\\mini_frames\\";
            var outputDir = "history_new\\";
            int t = 2;
            var startDate = new DateTime(1444, 11, 11);
            List<List<int>> matrix = new List<List<int>>();
            foreach (var item in Directory.GetFiles(inputDir))
            {
                matrix.Add(new List<int>());
                Bitmap original = (Bitmap)Image.FromFile(item);
                for (int i = 0; i < 4096; i++)
                {
                    var px = original.GetPixel(i/64,i%64);
                    if (px.R >= 128)
                    {
                        matrix[matrix.Count - 1].Add(1);
                    }
                    else if (px.R <= 128)
                    {
                        matrix[matrix.Count - 1].Add(0);
                    }
                    else throw new Exception("Something wrong!");
                }
            }
            for (int i = 0; i < 4096; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(
@"
is_city = yes

add_core = A01
owner = A01
controller = A01

base_tax = 1
base_production = 1
base_manpower = 1

culture = test_culture
religion = test_religion

trade_goods = grain

");
                var formatStr = "yyyy.MM.dd";
                var filename = $"{i+1} - {ToId(i /64, i % 64)}.txt";
                for (int j = 0; j < matrix.Count; j++)
                {
                    var nd = startDate.AddMonths(j+1);
                    string owner = "AO1";
                    if (matrix[j][i] == 1)
                    {
                        owner = "AO2";
                    }
                    if (j > 0 && matrix[j][i] != matrix[j - 1][i])
                    {
                        sb.AppendLine($"{nd.ToString(formatStr)}={{owner={owner} controller={owner} }}");
                    }
                }
                FileStream fs = new FileStream(outputDir + filename,FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(sb.ToString());
                sw.Flush();
                sw.Close();
                fs.Close();
                Console.WriteLine(i);
            }
        }
        static void Main(string[] args)
        {
            CreateHistory();
        }

        private static void ScaleFrames()
        {
            var inputDir = "E:\\fmod\\parsed\\frames\\";
            int t = 1;
            foreach (var item in Directory.GetFiles(inputDir))
            {
                Bitmap original = (Bitmap)Image.FromFile(item);
                Bitmap resized = new Bitmap(original, new Size(64,64));
                resized.Save(item.Replace("frames","mini_frames"));
                Console.WriteLine(t++);
            }
        }

        private static void CreateOriginalHistory()
        {
            var pth = "history\\";
            int t = 1;
            for (int i = 0; i < Reso; i++)
            {
                for (int j = 0; j < Reso; j++)
                {
                    var wt = @"
is_city = yes

add_core = A01
owner = A01
controller = A01

base_tax = 1
base_production = 1
base_manpower = 1

culture = test_culture
religion = test_religion

trade_goods = grain
";
                    var fname = $"{t}-{ToId(i, j)}.txt";
                    using (FileStream fs = new FileStream(pth + fname, FileMode.OpenOrCreate)) {
                        using(StreamWriter sw=new StreamWriter(fs))
                        {
                            sw.Write(wt);
                            sw.Flush();
                        }
                    }
                    t++;
                }
            }
        }
    }
}
