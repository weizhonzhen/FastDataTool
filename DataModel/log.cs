using System;
using System.IO;

namespace DataModel
{
    public class log
    {
        private readonly static object obj = new object();

        #region 写日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="content"></param>
        public static void SaveLog(string content,string fileName)
        {
            if (!Directory.Exists(string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory)))
                Directory.CreateDirectory(string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory));

            var path = string.Format("{0}log\\{1}_{2}.txt", AppDomain.CurrentDomain.BaseDirectory, fileName, DateTime.Now.ToString("yyyy-MM"));

            lock (obj)
            {
                if (!File.Exists(path))
                {
                    using (var fs = File.Create(path)) { }
                }
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter m_streamWriter = new StreamWriter(fs);
                    m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);

                    m_streamWriter.WriteLine(string.Format("[{0}]{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), content));
                    
                    m_streamWriter.Flush();
                    m_streamWriter.Close();
                    fs.Close();
                }
            }
        }
        #endregion
    }
}
