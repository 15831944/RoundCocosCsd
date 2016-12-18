using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MidiToPlist
{
    class Program
    {
        static ArrayList compatable = new ArrayList();
        static void Main(string[] args)
        {
            string filepath = "D:\\GameDO\\Round\\Round\\bin\\p011_bonfire.csd";
            //获取程序所在的位置
            string nowpath = AppDomain.CurrentDomain.BaseDirectory;
            compatable = ReadCompatable(nowpath + "RoundConfig.txt");
            //string text2 = (string)ReadFile(filepath, ReadType.ReadToEnd);
            //text2 = ReplaceStr(text2);
            //CreateFille("1.csd", Path.GetDirectoryName(filepath), text2);
            if (args.Length >= 1)
            {
                //获取拖入的文件
                foreach (string f in args)
                {
                    filepath = f;
                    //获取路径，名称，还有后缀
                    string path = Path.GetDirectoryName(filepath);
                    string filename = Path.GetFileName(filepath);
                    string Extension = Path.GetExtension(filepath);

                    if (Extension == ".csd")
                    {
                        string text = (string)ReadFile(filepath, ReadType.ReadToEnd);
                        text = ReplaceStr(text);
                        CreateFille(filename, Path.GetDirectoryName(filepath), text);
                    }
                    else
                    {
                        Console.WriteLine("这个文件是:" + filename);
                        Console.WriteLine("请放入csd文件。。");
                        Console.ReadLine();
                    }
                }
            }
        }

        static void CreateFille(string filename, string path, string str)
        {
            //创建backup文件夹
            if (!Directory.Exists(path + "//backup"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path + "/backup");
                directoryInfo.Create();
            }

            string t = Regex.Replace(System.DateTime.Now.ToString(), "/|:| ", "-");
            DirectoryInfo directoryInfo2 = new DirectoryInfo(path + "/backup/" + t);
            directoryInfo2.Create();

            string backpath = path + "/backup/" + t + "/" + filename;
            //复制老文件到backup中
            File.Copy(filename, backpath,true);

            //不存在文件
            if (!File.Exists(filename))
            {
                FileStream fs1 = new FileStream(filename, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);

                //开始写入值
                //sw.WriteLine("<!--  filename:" + filename + "  -->");
                sw.Write(str);
                sw.Close();
                fs1.Close();
            }
            //存在文件
            else
            {
                FileStream fs = new FileStream(filename, FileMode.Truncate, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                
                //开始写入值
                //sr.WriteLine("<!--  filename:" + filename + "  -->");
                sr.Write(str);
                sr.Close();
                fs.Close();
            }
        }

        public enum ReadType
        {
            ReadLine,
            ReadToEnd
        }
        static public Object ReadFile(string filepath, ReadType type)
        {
            Object readedText = new Object();
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            string text;
            if (type == ReadType.ReadLine)
            {
                //逐行读取
                ArrayList textline = new ArrayList();
                while ((text = sr.ReadLine()) != null)
                {
                    textline.Add(textline);
                }

                readedText = textline;
            }
            else
            {    
                //直接读取
                text = sr.ReadToEnd();
                readedText = text;
            }
            sr.Close();
            sr.Dispose();

            return readedText;
        }

        //读取配置表
        static ArrayList ReadCompatable(string filepath)
        {
            ArrayList dic = new ArrayList();
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            string line;
            //逐行读取
            while ((line = sr.ReadLine()) != null)
            {
                //如果碰见注释符号，则忽略本行
                if (line.Contains("//") || line == "")
                    continue;
                line = Regex.Unescape(line);
                dic.Add(line);
            }
            sr.Close();
            sr.Dispose();

            return dic;
        }

        static string ReplaceStr(string str)
        {
            string replaceStr = str;
            foreach (string regexstr in compatable)
            {
                Regex reg = new Regex(regexstr, RegexOptions.IgnoreCase);
                MatchCollection mac = reg.Matches(replaceStr);

                int offset = 0;
                foreach (Match m in mac)
                {
                    string[] f = new string[m.Groups.Count];
                    string[] complecrf = new string[m.Groups.Count];

                    for (int i = 0; i < f.Length; i++)
                    {
                        f[i] = m.Groups[i].Value;
                        if (i != 0)
                        {
                            float num = float.Parse(f[i]);
                            if(num>0)
                                complecrf[i] = ((int)(num + 0.5f)).ToString();
                            else
                                complecrf[i] = ((int)(num - 0.5f)).ToString();

                            replaceStr = replaceStr.Remove(m.Groups[i].Index - offset, m.Groups[i].Length);
                            replaceStr = replaceStr.Insert(m.Groups[i].Index - offset, complecrf[i]);

                            offset += m.Groups[i].Length - complecrf[i].Length;
                        }
                        else
                        {
                            complecrf[i] = f[i];
                        }     
                    }
                }
            }
            return replaceStr;
        }
    }
}
