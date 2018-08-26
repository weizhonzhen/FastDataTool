using Aoite.LevelDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataModel
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public static class DataCache
    {
        private readonly static string path = string.Format("{0}Db", AppDomain.CurrentDomain.BaseDirectory);
        private readonly static Options options = new Options { CreateIfMissing = true };

        #region 转成字节流
        /// <summary>
        /// 转成字节流
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] ToByte(this object value)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                return stream.GetBuffer();
            }
        }
        #endregion

        #region 流转成对象
        /// <summary>
        /// 流转成对象
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T ToModel<T>(this byte[] value) where T : class, new()
        {
            using (var stream = new MemoryStream(value))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as T;
            }
        }

        public static object ToModel(this byte[] value)
        {
            using (var stream = new MemoryStream(value))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <summary>
        /// get value
        /// </summary>
        public static T Get<T>(string key) where T : class, new()
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                var item = db.Get(key);

                if (item == null)
                    return new T();
                else
                    return db.Get(key).ByteArray.ToModel<T>();
            }
        }
        #endregion

        #region 设置值
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static void Set<T>(string key, T value)
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                db.Set(key, value.ToByte());
            }
        }
        #endregion
        
        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                return db.Get(key);
            }
        }
        #endregion

        #region 设置值
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static void Set(string key, string value)
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                db.Set(key, value);
            }
        }
        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                return db.Get(key) != null;
            }
        }
        #endregion

        #region 清空
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="Name"></param>
        public static void Remove(string key)
        {
            CheckPath();
            using (var db = new LDB(path))
            {
                db.Remove(key);
            }
        }
        #endregion

        /// <summary>
        /// check path
        /// </summary>
        private static void CheckPath()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
