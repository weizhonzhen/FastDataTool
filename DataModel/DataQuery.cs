using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DataModel
{
    public class DataQuery
    {
        #region 获取主键
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static TableKey GetKey(DataLink source, SoureTable tab)
        {
            try
            {
                var key = new TableKey();
                var sql = new StringBuilder();
                var dt = new DataTable();

                if (source.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    using (var conn = new SqlConnection(source.connStr))
                    {
                        sql.AppendFormat(@"select b.column_name from information_schema.table_constraints a
                                            inner join information_schema.constraint_column_usage b on a.constraint_name = b.constraint_name
                                            where a.constraint_type = 'PRIMARY KEY' and a.table_name = '{0}'", tab.tabName);
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = sql.ToString();
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                        conn.Close();
                    }
                    #endregion
                }
                else if (source.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(source.connStr))
                    {
                        sql.AppendFormat(@"select col.column_name from user_constraints con, user_cons_columns col 
                                        where con.constraint_name = col.constraint_name and con.constraint_type='P' 
                                        and col.table_name = '{0}'", tab.tabName.ToUpper());

                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = sql.ToString();
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                        conn.Close();
                    }
                    #endregion
                }

                if (dt.Rows.Count != 0)
                {
                    key.keyName = string.Format("pk_{0}_", tab.tabName);
                    
                    foreach (DataRow item in dt.Rows)
                    {
                        key.colName = string.Format("{0}{1},", key.colName, item.ItemArray[0].ToString());
                        key.keyName = string.Format("{0}{1}_", key.keyName, item.ItemArray[0].ToString().Substring(0, 3));
                    }
                    key.colName = key.colName.Substring(0, key.colName.Length - 1);
                    key.keyName = key.keyName.Substring(0, key.keyName.Length - 1);
                }

                return key;
            }
            catch
            {
                return new TableKey();
            }
        }
        #endregion

        #region 获取索引
        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static List<TableIndex> GetIndex(DataLink source,DataLink target, SoureTable tab)
        {
            try
            {
                var list = new List<TableIndex>();
                var dt = new DataTable();
                var dtCol = new DataTable();
                var count = 0;

                if (source.dbType == DataDbType.SqlServer)
                {
                    #region sqlserver
                    using (var conn = new SqlConnection(source.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select name,type_desc from sys.indexes where type_desc!='HEAP' and object_id=object_id('{0}') and is_primary_key=0", tab.tabName);
                        
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();

                        //索引名
                        foreach (DataRow item in dt.Rows)
                        {
                            count++;
                            var index = new TableIndex();
                            index.indexName = item.ItemArray[0].ToString();

                            if (source.dbType == target.dbType)
                                index.indexType = item.ItemArray[1].ToString();
                            else
                                index.indexType = item.ItemArray[1].ToString().Replace("NONCLUSTERED", "").Replace("CLUSTERED", "");

                            cmd.CommandText = string.Format(@"select d.name from sysindexes a join sysindexkeys b on a.id=b.id and a.indid=b.indid 
                                                join syscolumns d on b.id=d.id AND b.colid=d.colid where a.indid not in(0,255) 
                                                and a.name='{0}' order by d.colid asc", index.indexName);

                            var sad = new SqlDataAdapter(cmd);
                            dtCol.Clear();
                            sad.Fill(dtCol);

                            //索引列
                            foreach (DataRow col in dtCol.Rows)
                            {
                                index.colName = string.Format("{0}{1},", index.colName, col.ItemArray[0].ToString());
                            }
                            index.indexName = string.Format("index_{0}_{1}", tab.tabName, count);
                            index.colName = index.colName.Substring(0, index.colName.Length - 1);
                            list.Add(index);
                        }

                        conn.Close();
                    }
                    #endregion
                }
                else if (source.dbType == DataDbType.Oracle)
                {
                    #region oracle
                    using (var conn = new OracleConnection(source.connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = string.Format("select index_name , index_type from user_indexes where index_type!='LOB' and index_type!='FUNCTION-BASED NORMAL' and table_name='{0}'", tab.tabName.ToUpper());
                        
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();

                        //索引名
                        foreach (DataRow item in dt.Rows)
                        {
                            count++;
                            var index = new TableIndex();
                            index.indexName = item.ItemArray[0].ToString().ToUpper();

                            if (source.dbType == target.dbType)
                                index.indexType = item.ItemArray[1].ToString().Replace("NORMAL", "");
                            else
                                index.indexType = item.ItemArray[1].ToString().Replace("NORMAL", "").Replace("BITMAP", "");

                            cmd.CommandText = string.Format("select column_name from user_ind_columns where index_name='{0}' order by column_position asc", index.indexName);
                            
                            var oad = new OracleDataAdapter(cmd);
                            dtCol.Clear();
                            oad.Fill(dtCol);

                            //索引列
                            foreach (DataRow col in dtCol.Rows)
                            {
                                index.colName = string.Format("{0}{1},", index.colName, col.ItemArray[0].ToString());
                            }
                            index.indexName = string.Format("index_{0}_{1}", tab.tabName, count);
                            index.colName = index.colName.Substring(0, index.colName.Length - 1);
                            list.Add(index);
                        }

                        conn.Close();
                    }
                    #endregion
                }

                return list;
            }
            catch
            {
                return new List<TableIndex>();
            }
        }
        #endregion
        
        #region 获取serversql数据库版本
        /// <summary>
        /// 获取serversql数据库版本
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static int GetVersion(string connStr)
        {
            try
            {
                var refValue = 0;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var dt = new DataTable();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select SERVERPROPERTY('productversion')";
                    var rd = cmd.ExecuteReader();
                    dt.Load(rd);
                    rd.Close();

                    refValue = Int32.Parse(dt.Rows[0][0].ToString().Split('.')[0]);

                    conn.Close();
                }
                return refValue;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region 是否序列
        /// <summary>
        /// 是否序列
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsSequence(DataLink source, DataLink target)
        {
            if (source.dbType == DataDbType.SqlServer && target.dbType == DataDbType.Oracle)
                return GetVersion(source.connStr) >= 11;
            else if (source.dbType == target.dbType && target.dbType == DataDbType.SqlServer)
            {
                if (GetVersion(source.connStr) >= 11 && GetVersion(target.connStr) >= 11)
                    return true;
                else
                    return false;
            }
            else if (source.dbType == DataDbType.Oracle && target.dbType == DataDbType.SqlServer)
                return GetVersion(source.connStr) >= 11;
            else
                return true;
        }
        #endregion

        #region 初始化分页实体
        /// <summary>
        /// 初始化分页实体
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static PageModel InitPageModel(DataLink link,SoureTable tab)
        {
            var item = new PageModel();

            //每页条数
            item.pageSize = tab.tabCondtion.count;

            //页数
            if ((tab.tabCount % item.pageSize) == 0)
                item.pageCount = long.Parse((tab.tabCount / item.pageSize).ToString());
            else
                item.pageCount = long.Parse(((tab.tabCount / item.pageSize) + 1).ToString());

            //当前页
            item.pageId = 1;

            //表名
            item.tableName = tab.tarGetTab;

            var list = DataSchema.ColumnList(link, tab.tarGetTab);

            foreach(var temp in list)
            {
                item.field = string.Format("{0}{1},", item.field, temp.colName);
            }

            item.field = item.field.Substring(0, item.field.Length - 1);

            item.where = tab.tabCondtion.where;

            return item;
        }
        #endregion

        #region 获取分页
        /// <summary>
        /// 获取分页
        /// </summary>
        /// <param name="link"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static DataTable GetPageList(DataLink source,ref PageModel model)
        {
            var dt = new DataTable();
            var sb = new StringBuilder();

            var starId = (model.pageId - 1) * model.pageSize + 1;
            var endId = model.pageId * model.pageSize;

            if (source.dbType == DataDbType.Oracle)
            {
                #region oracle
                using (var conn = new OracleConnection(source.connStr))
                {
                    log.SaveLog("开始读取oracle分页","GetPage");
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    sb.AppendFormat(@"select {2} from {3} where rowid in(select rid from 
                                        (select rownum rn,rid from (select rowid rid from {3}) where rownum<={0}) where rn>{1}) and {4}"
                                       , endId.ToString(), (starId - 1).ToString(), model.field, model.tableName
                                       , String.IsNullOrEmpty(model.where) ? "1=1" : model.where);
                    cmd.CommandText = sb.ToString();
                    var rd = cmd.ExecuteReader();
                    dt.Load(rd);
                    rd.Close();
                    conn.Close();
                    log.SaveLog("结束读取oracle分页", "GetPage");
                }
                #endregion
            }
            else if (source.dbType == DataDbType.SqlServer)
            {
                #region sqlserver
                using (var conn = new SqlConnection(source.connStr))
                {
                    log.SaveLog("开始读取sqlserver分页", "GetPage");
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    sb.AppendFormat(@"select top {0} {3} from (select row_number()over(order by tempcolumn)temprownumber,* 
                                        from (select tempcolumn=0,* from {1} where )t)tt where temprownumber>={2}"
                                            , model.pageSize.ToString(), model.tableName
                                            , starId.ToString(), model.field
                                            , String.IsNullOrEmpty(model.where) ? "1=1" : model.where);
                    cmd.CommandText = sb.ToString();                    
                    var rd = cmd.ExecuteReader();
                    dt.Load(rd);
                    rd.Close();
                    conn.Close();
                    log.SaveLog("结束读取sqlserver分页", "GetPage");
                }
                #endregion 
            }

            model.total = dt.Rows.Count;
            return dt;
        }
        #endregion
    }
}
