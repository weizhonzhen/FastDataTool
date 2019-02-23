using DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FastDataTool
{
    /// <summary>
    /// 生成chm
    /// </summary>
    public static class Chm
    {
        #region 创建Hhp Chm工程文件
        /// <summary>
        /// 创建Hhp Chm工程文件
        /// </summary>
        public static void CreateHhp(string path)
        {
            using (var stream = new FileStream(string.Format("{0}\\table.hhp", path), FileMode.Create))
            {
                using (var write = new StreamWriter(stream, Encoding.GetEncoding("GB18030")))
                {
                    var html = new StringBuilder();
                    write.WriteLine("[OPTIONS]");
                    write.WriteLine("Title=表结构");
                    write.WriteLine("Compatibility=1.1 or later");
                    write.WriteLine("Compiled file=table.chm");
                    write.WriteLine("Contents file=table.hhc");
                    write.WriteLine("Index file=table.hhk");
                    write.WriteLine("Default topic=main.html");
                    write.WriteLine("Display compile progress=NO");
                    write.WriteLine("Language=0x804 中文(中国)");
                    write.WriteLine("Default Window=Main");
                    write.WriteLine();
                    write.WriteLine("[WINDOWS]");
                    write.WriteLine("Main=table,\"table.hhc\",\"table.hhk\",,,,,,,0x20,180,0x104E, [80,60,720,540],0x0,0x0,,,,,0");
                    write.WriteLine();
                    write.WriteLine("[FILES]");
                    write.WriteLine("main.html");
                    write.WriteLine();
                }
            }
        }
        #endregion

        #region 创建hhc 目录
        /// <summary>
        /// 创建hhc 目录
        /// </summary>
        public static void CreateHhc(string path, List<ChmModel> list)
        {
            using (var hhcStream = new FileStream(string.Format("{0}\\table.hhc", path), FileMode.Create))
            {
                using (var write = new StreamWriter(hhcStream, Encoding.GetEncoding("GB18030")))
                {
                    write.WriteLine("<html><body>");
                    write.WriteLine("<head></head>");
                    write.WriteLine("<object type=\"text/site properties\"><param name=\"Window Styles\" value=\"0x800025\"></object>");

                    foreach (var item in list)
                    {
                        write.WriteLine("<ul>");
                        write.WriteLine("<li><object type=\"text/sitemap\">");
                        write.WriteLine("<param name=\"Name\" value=\"" + item.tabName + "\">");
                        write.WriteLine("<param name=\"Local\" value=\"" + path + "\\" + item.tabName + ".htm\">");
                        write.WriteLine("<param name=\"ImageNumber\" value=\"user\">");
                        write.WriteLine("</object></li>");
                        write.WriteLine("</ul>");

                        using (var htmlStream = new FileStream(string.Format("{0}\\" + item.tabName + ".htm", path), FileMode.Create))
                        {
                            using (var html = new StreamWriter(htmlStream, Encoding.GetEncoding("GB18030")))
                            {
                                var sb = new StringBuilder();
                                sb.Append("<html><head></head><body>");
                                sb.Append("<style>table,table tr th, table tr td {border:1px solid #000;} table { width:100%;background-color:#f6f6f6; min-height: 28px; line-height: 25px; text-align: center; border-collapse: collapse; padding:2px;}</style>");
                                sb.Append("<table><tr>");
                                sb.AppendFormat("<td>{0}</td>", item.tabName);
                                sb.AppendFormat("<td colspan='3'>{0}</td>", item.tabComments);
                                sb.Append("</tr>");

                                foreach (var temp in item.columns)
                                {
                                    sb.Append("<tr>");
                                    sb.AppendFormat("<td width='30%'>{0}</td>", temp.colName);
                                    sb.AppendFormat("<td width='40%'>{0}</td>", temp.colComments);
                                    sb.AppendFormat("<td width='20%'>{0}</td>", temp.showType);
                                    sb.AppendFormat("<td width='10%'>{0}</td>", temp.isNull);
                                    sb.Append("</tr>");
                                }

                                sb.Append("</table></body></html>");
                                html.Write(sb);
                            }
                        }
                    }

                    write.WriteLine("</body></html>");
                }
            }
        }
        #endregion

        #region 创建Hhk 索引
        /// <summary>
        /// 创建Hhk 索引
        /// </summary>
        public static void CreateHhk(string path, List<ChmModel> list)
        {
            using (var steam = new FileStream(string.Format("{0}\\table.hhk", path), FileMode.Create))
            {
                using (var write = new StreamWriter(steam, Encoding.GetEncoding("UTF-8")))
                {
                    write.WriteLine("<html>");
                    write.WriteLine("<head>");
                    write.WriteLine("</head>");
                    write.WriteLine("<body>");
                    write.WriteLine("<ul>");

                    foreach (var item in list)
                    {
                        write.WriteLine("<li><object type=\"text/sitemap\">");
                        write.WriteLine("<param name=\"Name\" value=\"" + item.tabName + "\">");
                        write.WriteLine("<param name=\"Local\" value=\"" + item.tabName + ".htm\">");
                        write.WriteLine("</object></li>");
                    }

                    write.WriteLine("</ul>");
                    write.WriteLine("</body>");
                    write.WriteLine("</html>");
                    write.WriteLine();
                }
            }
        }
        #endregion

        #region Compile
        /// <summary>
        /// Compile
        /// </summary>
        /// <param name="path"></param>
        public static void Compile(string path, List<ChmModel> list)
        {
            string chmFile = string.Format("{0}\\table.chm", path);
            using (var process = new Process())
            {
                var processInfo = new ProcessStartInfo();
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.FileName = string.Format("{0}\\htmlhelp\\hhc.exe", AppDomain.CurrentDomain.BaseDirectory);
                processInfo.Arguments = string.Format("{0}\\table.hhp", path);
                processInfo.UseShellExecute = false;
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit();
            }

            foreach (var item in list)
            {
                File.Delete(string.Format("{0}\\{1}.htm", path, item.tabName));
            }

            File.Delete(string.Format("{0}\\table.hhc", path));
            File.Delete(string.Format("{0}\\table.hhk", path));
            File.Delete(string.Format("{0}\\table.hhp", path));
        }
        #endregion
    }
}
