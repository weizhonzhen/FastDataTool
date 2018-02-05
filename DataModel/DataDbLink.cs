using System;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DataModel
{
    /// <summary>
    /// dblink
    /// </summary>
    public class DataDbLink
    {
        #region 创建dbLink
        /// <summary>
        /// 创建dbLink
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CreateDbLink(DataLink target, DataLink source, ref string dbLinkName)
        {
            try
            {
                dbLinkName = "db" + Guid.NewGuid().ToString().Replace("-", "");
                if (target.dbType == DataDbType.SqlServer && source.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver to sqlserver
                    using (var conn = new SqlConnection(target.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("exec sp_addlinkedserver '{0}','','SQLOLEDB','{1},{2}'", dbLinkName, source.hostName, source.port);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("exec sp_addlinkedsrvlogin '{0}','false',null,'{1}','{2}'", dbLinkName, source.userName, source.userPwd);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                    #endregion
                }
                else if (target.dbType == DataDbType.Oracle && source.dbType == DataDbType.Oracle)
                {
                    #region oracle to oracle
                    using (var conn = new OracleConnection(target.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("create database link {0} connect to {1} identified by {2} using '{3}:{4}/{5}'"
                                                        , dbLinkName, source.userName, source.userPwd, source.hostName, source.port, source.serverValue);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                    #endregion
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 删除dbLink
        /// <summary>
        /// 删除dbLink
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="dbLinkName"></param>
        /// <returns></returns>
        public static bool DeleteDbLink(DataLink target, DataLink source, string dbLinkName)
        {
            try
            {
                if (target.dbType == DataDbType.SqlServer && source.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver to sqlserver
                    using (var conn = new SqlConnection(target.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("exec sp_dropserver '{0}','droplogins' ", dbLinkName);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
                else if (target.dbType == DataDbType.Oracle && source.dbType == DataDbType.Oracle)
                {
                    #region oracle to oracle
                    using (var conn = new OracleConnection(target.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("drop database link {0}", dbLinkName);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
