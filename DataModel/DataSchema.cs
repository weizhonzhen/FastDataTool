using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace DataModel
{
    /// <summary>
    /// 表结构
    /// </summary>
    public class DataSchema
    {
        #region 列缓存键
        /// <summary>
        /// 列缓存键
        /// </summary>
        /// <returns></returns>
        public static string GetColumnKey(DataLink link, string tableName)
        {
            return string.Format("{0}_{1}_{2}", link.dbType, tableName, link.hostName);
        }
        #endregion

        #region 获取表说明
        /// <summary>
        /// 获取表说明
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="strConn">连接串</param>
        /// <returns></returns>
        public static List<BaseTable> TableList(DataLink link, string tableName = "", bool isUpdate = false)
        {
            try
            {
                var list = new List<BaseTable>();
                var dt = new DataTable();

                //oracle 表信息
                if (link.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select a.table_name,comments from user_tables a inner join user_tab_comments b on a.TABLE_NAME=b.TABLE_NAME {0}"
                                    , (String.IsNullOrEmpty(tableName) || tableName == "loadColumnList" ? "" : string.Format(" and a.table_name='{0}'", tableName)));

                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        conn.Close();
                    }
                    #endregion
                }

                //sql server 表信息
                if (link.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    using (var conn = new SqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select name,(select top 1 value from sys.extended_properties where major_id=object_id(a.name) and minor_id=0) as value from sys.objects a where type='U' {0}"
                                                , (String.IsNullOrEmpty(tableName) || tableName == "loadColumnList" ? "" : string.Format(" and name='{0}'", tableName)));

                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        conn.Close();
                    }
                    #endregion
                }

                //mysql 表信息
                if (link.dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select table_name, table_comment from information_schema.TABLES where table_schema='{0}'{1} and table_type='BASE TABLE'"
                                                        , link.serverValue, String.IsNullOrEmpty(tableName) || tableName == "loadColumnList" ? "" : string.Format(" and name='{0}'", tableName));

                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);

                        conn.Close();
                    }
                    #endregion
                }

                foreach (DataRow item in dt.Rows)
                {
                    var table = new BaseTable();
                    table.tabComments = item.ItemArray[1] == DBNull.Value ? "" : item.ItemArray[1].ToString();
                    table.tabName = item.ItemArray[0] == DBNull.Value ? "" : item.ItemArray[0].ToString();
                    list.Add(table);

                    //预先加载列信息
                    if (tableName == "loadColumnList")
                    {
                        ColumnList(link, table.tabName, isUpdate);
                    }
                }

                return list;
            }
            catch
            {
                return new List<BaseTable>();
            }
        }
        #endregion

        #region 获取视图说明
        /// <summary>
        /// 获取视图说明
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="strConn">连接串</param>
        /// <returns></returns>
        public static List<BaseTable> ViewList(DataLink link, string tableName = "", bool isUpdate = false)
        {
            try
            {
                var list = new List<BaseTable>();
                var dt = new DataTable();

                //oracle 表信息
                if (link.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "select view_name from user_views";
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        conn.Close();
                    }
                    #endregion
                }

                //sql server 表信息
                if (link.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    using (var conn = new SqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "select name from sys.views";

                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        conn.Close();
                    }
                    #endregion
                }

                //mysql 表信息
                if (link.dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.views WHERE TABLE_SCHEMA = '{0}'", link.serverValue);

                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);

                        conn.Close();
                    }
                    #endregion
                }

                foreach (DataRow item in dt.Rows)
                {
                    var table = new BaseTable();
                    table.tabComments = "";
                    table.tabName = item.ItemArray[0] == DBNull.Value ? "" : item.ItemArray[0].ToString();
                    list.Add(table);

                    //预先加载列信息
                    if (tableName == "loadColumnList")
                    {
                        ColumnList(link, table.tabName, isUpdate);
                    }
                }
                return list;
            }
            catch
            {
                return new List<BaseTable>();
            }
        }
        #endregion

        #region 获取列的信息
        /// <summary>
        /// 获取列的信息
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="strConn">连接串</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static List<BaseColumn> ColumnList(DataLink link, string tableName, bool isUpdate = false)
        {
            try
            {
                var key = GetColumnKey(link, tableName);
                if (AppCache.ExistsTableColumn(key) && !isUpdate)
                    return AppCache.GetTableColumn(key);

                //脱机
                if (!AppCache.GetLineState(link))
                {
                    var templist = new List<BaseColumn>();
                    templist.Add(new BaseColumn { colName = "数据库脱机" });
                    return templist;
                }

                var list = new List<BaseColumn>();
                var dt = new DataTable();

                //oracle 列信息
                if (link.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(link.connStr))
                    {
                        tableName = tableName.ToUpper();
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"select a.column_name,data_type,data_length,b.comments,
                                            (select count(0) from user_cons_columns aa, user_constraints bb
                                                where aa.constraint_name = bb.constraint_name and bb.constraint_type = 'P' and bb.table_name = '"
                                            + tableName + @"' and aa.column_name=a.column_name),(select count(0) from user_ind_columns t,user_indexes i 
                                            where t.index_name = i.index_name and t.table_name = i.table_name and t.table_name = '"
                                            + tableName + @"' and t.column_name=a.column_name),nullable,data_precision,data_scale
                                            from user_tab_columns a inner join user_col_comments b
                                            on a.table_name='" + tableName +
                                            "' and a.table_name=b.table_name and a.column_name=b.column_name order by a.column_id asc";
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                        conn.Close();
                    }
                    #endregion
                }

                if (link.dbType == DataDbType.SqlServer)
                {
                    #region sql server
                    using (var conn = new SqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"select a.name,(select top 1 name from sys.systypes c where a.xtype=c.xtype) as type ,
                                        length,b.value,(select count(0) from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where TABLE_NAME='"
                                        + tableName + @"' and COLUMN_NAME=a.name),
                                        (SELECT count(0) FROM sysindexes aa JOIN sysindexkeys bb ON aa.id=bb.id AND aa.indid=bb.indid 
                                         JOIN sysobjects cc ON bb.id=cc.id  JOIN syscolumns dd ON bb.id=dd.id AND bb.colid=dd.colid 
                                         WHERE aa.indid NOT IN(0,255) AND cc.name='" + tableName + @"' and dd.name=a.name),isnullable,prec,scale
                                        from syscolumns a left join sys.extended_properties b 
                                        on major_id = id and minor_id = colid and b.name ='MS_Description' 
                                        where a.id=object_id('" + tableName + "') order by a.colid asc";
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                        conn.Close();
                    }
                    #endregion
                }

                if (link.dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"select column_name,data_type,character_maximum_length,column_comment,
                                            (select count(0) from INFORMATION_SCHEMA.KEY_COLUMN_USAGE a where TABLE_SCHEMA='" + link.serverValue
                                            + "' and TABLE_NAME='" + tableName + @"' and constraint_name='PRIMARY' and c.column_name=a.column_name),
                                            (SELECT count(0) from information_schema.statistics a where table_schema = '"
                                            + link.serverValue + "' and table_name = '" + tableName + @"' and c.column_name=a.column_name),
                                            is_nullable,numeric_precision,numeric_scale,column_type from information_schema.columns c where table_name='"
                                            + tableName + "'  order by ordinal_position asc";
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                        conn.Close();
                    }
                    #endregion
                }

                foreach (DataRow item in dt.Rows)
                {
                    var column = new BaseColumn();
                    column.colName = (item.ItemArray[0] == DBNull.Value ? "" : item.ItemArray[0].ToString());
                    column.colType = item.ItemArray[1] == DBNull.Value ? "" : item.ItemArray[1].ToString();
                    column.colLength = item.ItemArray[2] == DBNull.Value ? 0 : decimal.Parse(item.ItemArray[2].ToString());
                    column.colComments = item.ItemArray[3] == DBNull.Value ? "" : item.ItemArray[3].ToString();
                    column.isKey = item.ItemArray[4].ToString() != "0" ? true : false;
                    column.isNull = (item.ItemArray[6].ToString().ToUpper().Trim() == "Y" || item.ItemArray[6].ToString().ToUpper().Trim() == "YES" || item.ItemArray[6].ToString().ToUpper().Trim() == "1") ? "是" : "否";

                    column.precision = item.ItemArray[7] == DBNull.Value ? 0 : int.Parse(item.ItemArray[7].ToString());
                    column.scale = item.ItemArray[8] == DBNull.Value ? 0 : int.Parse(item.ItemArray[8].ToString());
                    column.isIndex = item.ItemArray[5].ToString() != "0" ? true : false;

                    if (link.dbType == DataDbType.MySql)
                        column.showType = item.ItemArray[9] == DBNull.Value ? GetShowColType(column) : item.ItemArray[9].ToString();
                    else
                        column.showType = GetShowColType(column);

                    column.showColName = string.Format("{0}{1}{2}", column.isKey ? "(主键) " : "", column.isIndex ? "(索引) " : "", column.colName);
                    column.showTypeColName = string.Format("{0}  [{1}]", column.colName, column.showType);

                    list.Add(column);
                }

                AppCache.SetOnLine(link);
                AppCache.SetTableColumn(list, key);
                return list;
            }
            catch
            {
                AppCache.SetOffLine(link);
                return new List<BaseColumn>();
            }
        }
        #endregion 

        #region 获取显示列类型
        /// <summary>
        /// 获取显示列类型
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetShowColType(BaseColumn column)
        {
            switch (column.colType.ToLower().Trim())
            {
                case "char":
                case "varchar":
                case "varchar2":
                    return string.Format("{0}({1})", column.colType, column.colLength == -1 ? "max" : column.colLength.ToString());
                case "nchar":
                case "nvarchar":
                case "nvarchar2":
                    return string.Format("{0}({1})", column.colType, column.colLength == -1 ? "max" : (column.colLength/2).ToString());
                case "decimal":
                case "numeric":
                case "number":
                    if (column.precision == 0 && column.scale == 0)
                        return column.colType;
                    else
                        return string.Format("{0}({1},{2})", column.colType, column.precision, column.scale);
                default:
                    return column.colType;
            }
        }
        #endregion

        #region 连接检测
        /// <summary>
        /// 连接检测
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dbConn">连接串</param>
        public static bool CheckLink(string dbType, string dbConn)
        {
            try
            {
                var refValue = false;
                if (dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(dbConn))
                    {
                        try
                        {
                            conn.Open();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    #endregion
                }
                else if (dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    using (var conn = new SqlConnection(dbConn))
                    {
                        try
                        {
                            conn.Open();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                    #endregion
                }
                else if (dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(dbConn))
                    {
                        try
                        {
                            conn.Open();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    #endregion
                }

                return refValue;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 修改表说明
        /// <summary>
        /// 修改表说明
        /// </summary>
        public static void UpdateTabComments(BaseTable item, DataLink link)
        {
            try
            {
                var dt = new DataTable();
                var sql = new StringBuilder();

                if (link.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    sql.AppendFormat(@"exec sys.sp_updateextendedproperty N'MS_Description',N'{0}',N'SCHEMA', N'dbo', N'TABLE',N'{1}'", item.tabComments, item.tabName);

                    using (var conn = new SqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select count(0) from sys.extended_properties where object_id('{0}')=major_id and minor_id=0", item.tabName);
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);

                        if (Int32.Parse(dt.Rows[0][0].ToString()) >= 1)
                        {
                            cmd.CommandText = sql.ToString();
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = sql.ToString().Replace("sp_updateextendedproperty", "sp_addextendedproperty");
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    #endregion
                }
                else if (link.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("Comment on table {0} is '{1}'", item.tabName, item.tabComments);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
                else if (link.dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("alter table {0} comment '{1}'", item.tabName, item.tabComments);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.SaveLog(ex.ToString(), "UpdateTabComments");
            }
        }
        #endregion

        #region 修改列说明
        /// <summary>
        /// 修改列说明
        /// </summary>
        public static void UpdateColComments(BaseColumn colItem, BaseTable tabItem, DataLink link)
        {
            try
            {
                var dt = new DataTable();
                var sql = new StringBuilder();
                if (link.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    sql.AppendFormat(@"exec sys.sp_updateextendedproperty N'MS_Description',N'{0}',N'SCHEMA', N'dbo', N'TABLE',N'{1}',N'column',N'{2}'"
                                         , colItem.colComments, tabItem.tabName, colItem.colName);

                    using (var conn = new SqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format(@"select count(0) from syscolumns where id = object_id('{0}') and name='{1}'
                                                        and exists(select 1 from sys.extended_properties where object_id('{0}')=major_id and colid=minor_id)"
                                                        , tabItem.tabName, colItem.colName);
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);

                        if (Int32.Parse(dt.Rows[0][0].ToString()) >= 1)
                        {
                            cmd.CommandText = sql.ToString();
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = sql.ToString().Replace("sp_updateextendedproperty", "sp_addextendedproperty");
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    #endregion
                }
                else if (link.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("Comment on column {0}.{1} is '{2}'", tabItem.tabName, colItem.colName, colItem.colComments);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
                else if (link.dbType == DataDbType.MySql)
                {
                    #region mysql
                    using (var conn = new MySqlConnection(link.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        if (colItem.isKey)
                            cmd.CommandText = string.Format("update columns set column_comment ='{0}' where table_schema='{1}' and table_name='{2}' and clumn_name='{3}'"
                                                            , colItem.colComments, link.serverValue, tabItem.tabName, colItem.colComments);
                        else
                            cmd.CommandText = string.Format("alter table {0} modify {1} {2} comment '{3}'"
                                                            , tabItem.tabName, colItem.colName, colItem.showType, colItem.colComments);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion
                }
            }
            catch (Exception ex) { log.SaveLog(ex.ToString(), "UpdateColComments"); }
        }
        #endregion
    }
}
