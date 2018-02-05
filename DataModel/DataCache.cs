using System;
using System.Web;
using System.Web.Caching;
using System.Collections;

namespace DataModel
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class DataCache
    {
        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : new() 
        {
            try
            {
                var result = new T();
                
                object obj = HttpRuntime.Cache.Get(key);
                if (obj != null)
                    result = (T)((object)obj);

                return result;
            }
            catch
            {
                return new T();
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
        public static void Set<T>(string Name, T Value, int Hours = 0)
        {
            if (Value == null)
                Clear(Name);
            else
            {
                Clear(Name);
                HttpRuntime.Cache.Insert(Name, Value, null, DateTime.Now.AddHours(Hours), Cache.NoSlidingExpiration);
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
            try
            {
                var result = "";

                object obj = HttpRuntime.Cache.Get(key);
                if (obj != null)
                    result = (string)((object)obj);

                return result;
            }
            catch
            {
                return "";
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
        public static void Set(string Name, string Value, int Hours = 0)
        {
            if (Value == null)
                Clear(Name);
            else
            {
                Clear(Name);
                HttpRuntime.Cache.Insert(Name, Value, null, DateTime.Now.AddHours(Hours), Cache.NoSlidingExpiration);
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
            return HttpRuntime.Cache.Get(key) != null;
        }
        #endregion

        #region 清空
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="Name"></param>
        public static void Clear(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
        #endregion

        #region 清楚所有缓存
        /// <summary>
        /// 清楚所有缓存
        /// </summary>
        public static void ClearAll()
        {
            var list = new ArrayList();
            var cacheEnum = HttpRuntime.Cache.GetEnumerator();

            while (cacheEnum.MoveNext())
            {
                list.Add(cacheEnum.Key);
            }

            foreach (string key in list)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }
        #endregion
    }
}
