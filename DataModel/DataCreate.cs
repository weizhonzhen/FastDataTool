using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DataModel
{
    public class DataCreate
    {
        #region oracle与sqlserver字段对应
        /// <summary>
        /// oracle与sqlserver字段对应
        /// </summary>
        /// <param name="targetDbType"></param>
        /// <param name="sourceDbType"></param>
        /// <param name="dbFildType"></param>
        /// <returns></returns>
        private static string MatchType(DataLink target, DataLink source, BaseColumn item)
        {
            var refValue = "";

            if (target.dbType == DataDbType.SqlServer && source.dbType == DataDbType.Oracle)
            {
                #region oracle to sqlserver
                switch (item.colType.ToLower())
                {
                    case "char":
                    case "varchar2":
                        return string.Format("varchar({0})", item.colLength); 
                    case "nchar":
                    case "nvarchar2":
                        return string.Format("nvarchar({0})", item.colLength);
                    case "date":
                        return "datetime";
                    case "long":
                        return "text";
                    case "bfile":
                    case "blob":
                    case "long raw":
                    case "raw":
                    case "nrowid":
                    case "binary":
                        return "image";
                    case "rowid":
                        return "uniqueidentifier";
                    case "number":
                    case "decimal":
                        return string.Format("decimal({0},{1})", item.precision, item.scale);
                    case "integer":
                        return "int";
                    default:
                        return item.colType;
                }
                #endregion
            }
            else if (target.dbType == DataDbType.Oracle && source.dbType == DataDbType.SqlServer)
            {
                #region sqlserver to oracle
                switch (item.colType.ToLower())
                {
                    case "bit":
                    case "int":
                    case "smallint":
                    case "tinyint":
                        return "integer";
                    case "datetime":
                    case "smalldatetime":
                        return "date";
                    case "decimal":
                    case "numeric":
                        return string.Format("decimal({0},{1})", item.precision, item.scale);
                    case "money":
                    case "smallmoney":
                    case "real":
                        return "real";
                    case "uniqueidentifier":
                        return string.Format("char({0})", item.colLength);
                    case "nchar":
                    case "nvarchar":
                        {
                            if (item.colLength > 4000)
                                return "clob";
                            else
                                return string.Format("nvarchar2({0})", item.precision); 
                        }
                    case "varchar":
                    case "char":
                        {
                            if (item.colLength > 4000)
                                return "clob";
                            else
                                return string.Format("varchar2({0})", item.precision);
                        }
                    case "text":
                    case "ntext":
                        return "clob";
                    case "binary":
                    case "varbinary":
                    case "image":
                        return "blob";
                    default:
                        return item.colType;
                }
                #endregion
            }
            else
                refValue = item.colType;

            return refValue;
        }
        #endregion

        #region sql创建表
        /// <summary>
        /// sql创建表
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tabName"></param>
        private static bool SqlCreateTable(DataLink target, DataLink source, SoureTable tab)
        {
            try
            {
                var count = 0;
                var sql = new StringBuilder();
                var list = DataSchema.ColumnList(source, tab.tabName);

                #region sql
                sql.AppendFormat("create table {0} (", String.IsNullOrEmpty(tab.tarGetTab) ? tab.tabName : tab.tarGetTab);
                foreach (var item in list)
                {
                    count++;
                    sql.AppendFormat("{0} {1} {2}{3}", item.colName, MatchType(target, source, item), item.isNull=="是" ? "" : "not null", list.Count == count ? "" : ",");
                }
                sql.Append(")");
                #endregion

                if (target.dbType == DataDbType.Oracle && source.dbType == DataDbType.SqlServer)
                {
                    #region oracle create table
                    using (var conn = new OracleConnection(target.connStr))
                    {
                        //创建表
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = sql.ToString();
                        cmd.ExecuteNonQuery();

                        //列备注
                        foreach (var item in list)
                        {
                            if (!String.IsNullOrEmpty(item.colComments))
                            {
                                cmd.CommandText = string.Format("comment on column {0}.{1} is '{2}'"
                                    , String.IsNullOrEmpty(tab.tarGetTab) ? tab.tabName : tab.tarGetTab, item.colName, item.colComments);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        conn.Close();
                    }
                    #endregion
                }
                else if (target.dbType == DataDbType.SqlServer && source.dbType == DataDbType.Oracle)
                {
                    #region sqlserver create table
                    using (var conn = new SqlConnection(target.connStr))
                    {
                        //创建表
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = sql.ToString();
                        cmd.ExecuteNonQuery();

                        //列备注
                        foreach (var item in list)
                        {
                            if (!String.IsNullOrEmpty(item.colComments))
                            {
                                cmd.CommandText = string.Format("exec sys.sp_addextendedproperty N'MS_Description',N'{0}',N'SCHEMA', N'dbo', N'TABLE',N'{1}',N'column',N'{2}'"
                                                    , item.colComments, String.IsNullOrEmpty(tab.tarGetTab) ? tab.tabName : tab.tarGetTab, item.colName);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        
                        conn.Close();
                    }
                    #endregion
                }

                //表备注
                DataSchema.UpdateTabComments(DataSchema.TableList(source, tab.tabName).FirstOrDefault(), target);

                log.SaveLog(string.Format("表名（{0}）创建成功", tab.tabName), "CreateTable"); 

                return true;
            }
            catch(Exception ex)
            {
                log.SaveLog(string.Format("表名（{0}）创建失败:{1}", tab.tabName, ex.ToString()), "CreateTable_exp");
                return false; 
            }
        }
        #endregion
        
        #region 复制表结构
        /// <summary>
        /// 复制表结构
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CopyTable(DataLink target, DataLink source, SoureTable tab)
        {
            var dbLinkName = "";
            if (DataDbLink.CreateDbLink(target, source, ref dbLinkName) == false)
                return SqlCreateTable(target, source, tab);
            else
            {
                #region 通过dblink复制表结构
                try
                {
                    if (target.dbType == DataDbType.Oracle)
                    {
                        #region copy oracle table
                        var sql = string.Format("create table {0} as select * from {1}@{2} where 1=2"
                                                , tab.tarGetTab, tab.tabName, dbLinkName);

                        using (var conn = new OracleConnection(target.connStr))
                        {
                            conn.Open();
                            var cmd = conn.CreateCommand();
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        #endregion
                    }
                    else if (target.dbType == DataDbType.SqlServer)
                    {
                        #region copy SqlServer table
                        var sql = string.Format("select * into {0} from {1}.{2}.dbo.{3} where 1=2",
                                                tab.tarGetTab,dbLinkName, source.serverValue, tab.tabName);

                        using (var conn = new SqlConnection(target.connStr))
                        {
                            conn.Open();
                            var cmd = conn.CreateCommand();
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        #endregion
                    }

                    DataDbLink.DeleteDbLink(target, source, dbLinkName);

                    log.SaveLog(string.Format("表名（{0}）复制成功", tab.tabName), "CreateTable"); 
                }
                catch(Exception ex)
                {
                    log.SaveLog(string.Format("表名（{0}）复制失败:{1}", tab.tabName, ex.ToString()), "CreateTable_exp");
                    DataDbLink.DeleteDbLink(target, source, dbLinkName);
                    return false;
                }
                #endregion
            }
            return true;
        }
        #endregion

        #region 创建索引
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static State CreateIndex(DataLink target, DataLink source, SoureTable tab)
        {
            var refValue = new State();

            var list = DataQuery.GetIndex(source, target, tab);

            if (target.dbType == DataDbType.SqlServer && list.Count > 0)
            {
                #region sqlserver
                using (var conn = new SqlConnection(target.connStr))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    foreach (var item in list)
                    {
                        try
                        {
                            cmd.CommandText = string.Format("create {0} index {1} on dbo.{3}({2})" , item.indexType, item.indexName, item.colName , tab.tarGetTab);
                            cmd.ExecuteNonQuery();
                            refValue.success++;
                            log.SaveLog(string.Format("表名（{0}）的索引({1}),创建成功", tab.tabName, item.indexName), "CreateIndex");
                            
                        }
                        catch(Exception ex)
                        {
                            log.SaveLog(string.Format("表名（{0}）的索引({1}),创建失败:{2}", tab.tabName, item.indexName, ex.ToString()), "CreateIndex_exp");
                            refValue.fail++;
                        }
                    }
                    conn.Close();
                }
                #endregion
            }
            else if (target.dbType == DataDbType.Oracle && list.Count > 0)
            {
                #region oracle
                using (var conn = new OracleConnection(target.connStr))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    foreach (var item in list)
                    {
                        try
                        {
                            cmd.CommandText = string.Format("create {0} index {1} on {2}({3})"
                                                            , item.indexType, item.indexName, tab.tarGetTab, item.colName);

                            cmd.ExecuteNonQuery();
                            refValue.success++;
                            log.SaveLog(string.Format("表名（{0}）的索引({1}),创建成功", tab.tabName, item.indexName), "CreateIndex");
                        }
                        catch(Exception ex)
                        {
                            log.SaveLog(string.Format("表名（{0}）的索引({1}),创建失败:{2}", tab.tabName, item.indexName, ex.ToString()), "CreateIndex_exp");
                            refValue.fail++;
                        }
                    }
                    conn.Close();
                }
                #endregion
            }

            return refValue;
        }
        #endregion

        #region 创建主键
        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static State CreateKey(DataLink target, DataLink source, SoureTable tab)
        {
            var refValue = new State();
            var key = DataQuery.GetKey(source, tab);

            if (target.dbType == DataDbType.SqlServer && key.colName != null && key.keyName != null)
            {
                #region sqlserver
                using (var conn = new SqlConnection(target.connStr))
                {
                    try
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("alter table {0} add constraint {1} primary key({2})"
                                                        , tab.tarGetTab, key.keyName, key.colName);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        refValue.success++;
                        log.SaveLog(string.Format("表名（{0}）的主键(({1}),创建成功", tab.tabName, key.keyName), "CreateKey");
                    }
                    catch(Exception ex)
                    {
                        log.SaveLog(string.Format("表名（{0}）的主键({1}),创建失败:{2}", tab.tabName, key.keyName, ex.ToString()), "CreateKey_exp");
                        refValue.fail++;
                    }
                }
                #endregion
            }
            else if (target.dbType == DataDbType.Oracle && key.colName != null && key.keyName != null)
            {
                #region oracle
                using (var conn = new OracleConnection(target.connStr))
                {
                    try
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("alter table {0} add constraint {1} primary key({2})"
                                                        , tab.tarGetTab, key.keyName, key.colName);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        refValue.success++;
                        log.SaveLog(string.Format("表名（{0}）的主键(({1}),创建成功", tab.tabName, key.keyName), "CreateKey");
                    }
                    catch(Exception ex)
                    {
                        log.SaveLog(string.Format("表名（{0}）的主键({1}),创建失败:{2}", tab.tabName, key.keyName, ex.ToString()), "CreateKey_exp");
                        refValue.fail++;
                    }
                }
                #endregion
            }

            return refValue;
        }
        #endregion

        #region 创建表空间
        /// <summary>
        /// 创建表空间
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static State CreateTableSpace(DataLink target, DataLink source, SoureTable tab)
        {


            return new State();
        }
        #endregion

        #region 创建tvps
        /// <summary>
        /// 创建tvps
        /// </summary>
        /// <param name="link"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        private static bool CreateTvps(DataLink target, SoureTable tab)
        {
            try
            {
                var sql = new StringBuilder();
                var list = DataSchema.ColumnList(target, tab.tarGetTab);

                using (var conn = new SqlConnection(target.connStr))
                {
                    sql.AppendFormat("create type {0} as table (", tab.tarGetTab);

                    foreach(var item in list)
                    {
                        sql.AppendFormat("{0} {1},", item.colName, item.showType);
                    }

                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql.ToString().Substring(0, sql.ToString().Length - 1) + ")";
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 删除tvps
        /// <summary>
        /// 删除tvps
        /// </summary>
        /// <param name="link"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        private static bool DeleteTvps(DataLink target, SoureTable tab)
        {
            try
            {
                using (var conn = new SqlConnection(target.connStr))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("drop type {0}", tab.tarGetTab);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
        
        #region 获取列类型
        /// <summary>
        /// 获取列类型
        /// </summary>
        /// <param name="list"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private static OracleDbType GetOracleDbType(string colType)
        {
            switch (colType.ToUpper().Trim())
            {
                case "BFILE":
                    return OracleDbType.BFile;
                case "REAL":
                    return OracleDbType.Double;
                case "LONG":
                    return OracleDbType.Long;
                case "DATE":
                    return OracleDbType.Date;
                case "NUMBER":
                    return OracleDbType.Decimal;
                case "VARCHAR2":
                    return OracleDbType.Varchar2;
                case "NVARCHAR2":
                    return OracleDbType.NVarchar2;
                case "RAW":
                    return OracleDbType.Raw;
                case "DECIMAL":
                    return OracleDbType.Decimal;
                case "INTEGER":
                    return OracleDbType.Int32;
                case "CHAR":
                    return OracleDbType.Char;
                case "NCHAR":
                    return OracleDbType.NChar;
                case "FLOAT":
                    return OracleDbType.Double;
                case "BLOB":
                    return OracleDbType.Blob;
                case "CLOB":
                    return OracleDbType.Clob;
                case "NCLOB":
                    return OracleDbType.NClob;
                default:
                    return OracleDbType.NVarchar2;
            }
        }
        #endregion

        #region 获取tvps语句
        /// <summary>
        /// 获取tvps语句
        /// </summary>
        /// <param name="link"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        private static string GetTvps(DataLink target, SoureTable tab)
        {
            var sql1 = new StringBuilder();
            var sql2 = new StringBuilder();
            var list = DataSchema.ColumnList(target, tab.tarGetTab);

            sql1.AppendFormat("insert into {0} (", tab.tarGetTab);
            sql2.Append("select ");
            foreach(var item in list)
            {
                sql1.AppendFormat("{0},", item.colName);
                sql2.AppendFormat("tb.{0},", item.colName);
            }
            sql1.Append(")");
            sql2.AppendFormat("from @{0} as tb", tab.tarGetTab);

            return string.Format("{0}{1}", sql1.ToString().Replace(",)", ") "), sql2.ToString().Replace(",from", " from"));
        }
        #endregion

        #region odp.net 特性 获取sql
        /// <summary>
        ///  odp.net 特性 获取sql
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="cmd"></param>
        /// <param name="link"></param>
        /// <param name="tab"></param>
        private static void GetCmdParam(DataTable dt, ref OracleCommand cmd, DataLink link, SoureTable tab)
        {
            var i = 0;
            var sql = new StringBuilder();
            var list = DataSchema.ColumnList(link, tab.tarGetTab);

            sql.AppendFormat("insert into {0} values(", tab.tarGetTab);
            
            foreach (var item in list)
            {
                sql.AppendFormat(":{0},", item.colName);
                var param = new OracleParameter(item.colName, GetOracleDbType(item.colType));
                param.Direction = ParameterDirection.Input;
                object[] pValue = new object[dt.Rows.Count];

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    var itemValue = dt.Rows[j][i];

                    if (itemValue == null)
                        itemValue = DBNull.Value;

                    pValue[j] = itemValue;
                }

                param.Value = pValue;
                cmd.Parameters.Add(param);
                i++;
            }
            sql.Append(")");
           
            cmd.CommandText = sql.ToString().Replace(",)", ")");
        }
        #endregion

        #region 迁移数据
        /// <summary>
        /// 迁移数据 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static State AddList(DataLink target, DataLink source, SoureTable tab)
        {
            try
            {
                var isDelTvps = false;
                var item = new State();
                var model = DataQuery.InitPageModel(source, tab);
                
                if (target.dbType == DataDbType.SqlServer && DataQuery.GetVersion(target.connStr) >= 10)
                {
                    CreateTvps(target, tab);
                    isDelTvps = true;
                }

                for (int i = 1; i <= model.pageCount; i++)
                {
                    model.pageId = i;
                    var temp = RunAddList(target, source, tab, model);
                    item.success += temp.success;
                    item.fail += temp.fail;
                }

                if (isDelTvps)
                    DeleteTvps(target, tab);

                return item;
            }
            catch
            {
                return new State();
            }
        }
        #endregion

        #region 迁移数据线程
        /// <summary>
        /// 迁移数据线程
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tab"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static State RunAddList(DataLink target,DataLink source,SoureTable tab,PageModel model)
        {
            var item = new State();
            var dt = DataQuery.GetPageList(source, ref model);

            if (target.dbType == DataDbType.SqlServer)
            {
                if (DataQuery.GetVersion(target.connStr) >= 10)
                {
                    #region tvps
                    using (var conn = new SqlConnection(target.connStr))
                    {
                        try
                        {
                            log.SaveLog(string.Format("开始插入sqlserver数据{0}", tab.tabName), "Insert");
                            conn.Open();
                            var cmd = conn.CreateCommand();
                            cmd.CommandText = GetTvps(target, tab);
                            var catParam = cmd.Parameters.AddWithValue(string.Format("@{0}",tab.tarGetTab), dt);
                            catParam.SqlDbType = SqlDbType.Structured;
                            catParam.TypeName = tab.tarGetTab;
                            if (cmd.ExecuteNonQuery() > 0)
                                item.success = model.total;
                            else
                                item.fail = model.total;
                            cmd.Dispose();
                            log.SaveLog(string.Format("结束插入sqlserver数据{0}", tab.tabName), "Insert");
                        }
                        catch(Exception ex)
                        {
                            log.SaveLog(string.Format("表名（{0}）:{1}", tab.tabName, ex.ToString()), "Insert_Exp");
                            item.fail = model.total;
                        }                        
                    }
                    #endregion
                }
                else
                {
                    #region SqlBulkCopy
                    using (var conn = new SqlConnection(target.connStr))
                    {
                        try
                        {
                            log.SaveLog(string.Format("开始插入sqlserver数据{0}", tab.tabName), "Insert");
                            conn.Open();
                            using (var bulk = new SqlBulkCopy(target.connStr, SqlBulkCopyOptions.UseInternalTransaction))
                            {
                                bulk.DestinationTableName = tab.tarGetTab;
                                bulk.BatchSize = dt.Rows.Count;
                                bulk.WriteToServer(dt);
                                item.success = model.total;
                            }
                            log.SaveLog(string.Format("结束插入sqlserver数据{0}", tab.tabName), "Insert"); 
                        }
                        catch(Exception ex)
                        {
                            log.SaveLog(string.Format("表名（{0}）:{1}", tab.tabName, ex.ToString()), "Insert_Exp"); 
                            item.fail = model.total;
                        }
                    }
                    #endregion
                }
            }
            else if (target.dbType == DataDbType.Oracle)
            {
                #region odp.net特性
                try
                {
                    using (var conn = new OracleConnection(target.connStr))
                    {
                        log.SaveLog(string.Format("开始插入oracle数据{0}", tab.tabName), "Insert");
                        conn.Open();
                        var cmd = conn.CreateCommand();

                        //关闭日志
                        cmd.CommandText = string.Format("alter table {0} nologging", tab.tarGetTab); 
                        cmd.ExecuteNonQuery();

                        cmd.ArrayBindCount = dt.Rows.Count;
                        cmd.BindByName = true;
                        GetCmdParam(dt, ref cmd, target, tab);
                        if (cmd.ExecuteNonQuery() > 0)
                            item.success = model.total;
                        else
                            item.fail = model.total;

                        //开起日志
                        cmd.CommandText = string.Format("alter table {0} logging", tab.tarGetTab);                        
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        log.SaveLog(string.Format("结束插入oracle数据{0}", tab.tabName), "Insert");                        
                    }
                }
                catch(Exception ex)
                {
                    log.SaveLog(string.Format("表名（{0}）:{1}", tab.tabName, ex.ToString()), "Insert_Exp");
                    
                    item.fail = model.total;
                }
                #endregion
            }

            dt.Dispose();
            return item;
        }
        #endregion
    }
}
