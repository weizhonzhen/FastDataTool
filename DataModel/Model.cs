using System;

namespace DataModel
{
    #region 列实体
    /// <summary>
    /// 列实体
    /// </summary>
    [Serializable]
    public class BaseColumn
    { 
        // 唯一标识
        private string _colId = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string colId
        {
            set { _colId = value; }
            get { return _colId; }
        }
        
        /// <summary>
        /// 列名
        /// </summary>
        public string colName { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string colComments { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string colType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public decimal colLength { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool isKey { get; set; }

        /// <summary>
        /// 是否索引
        /// </summary>
        public bool isIndex { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool isIdentity { get; set; }
        
        /// <summary>
        /// 是否空
        /// </summary>
        public string isNull { get; set; }
        
        /// <summary>
        /// 空显示? 不为空不显示
        /// </summary>
        public string showNull { get; set; }
        
        /// <summary>
        /// 精度
        /// </summary>
        public int precision { get; set; }

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int scale { get; set; }

        /// <summary>
        /// 显示类型
        /// </summary>
        public string showType { get; set; }
        
        /// <summary>
        /// 显示列名(主键索引+列名)
        /// </summary>
        public string showColName { get; set; }

        /// <summary>
        /// 显示列名
        /// </summary>
        public string showTypeColName { get; set; }

        /// <summary>
        /// 验证出错信息
        /// </summary>
        public string maxMessage { get; set; }

        /// <summary>
        /// 验证必填信息
        /// </summary>
        public string requiredMessage { get; set; }

        /// <summary>
        /// 表单元素参数名称
        /// </summary>
        public string fromParam { get; set; }

        /// <summary>
        /// 表单元素显示名
        /// </summary>
        public string fromName { get; set; }

        /// <summary>
        /// 表单元素类型
        /// </summary>
        public string fromType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string defaultData { get; set; }
    }
    #endregion

    #region 表实体
    /// <summary>
    /// 表实体
    /// </summary>
   [Serializable]
    public class BaseTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tabName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string tabComments { get; set; }

        /// <summary>
        /// 表条件
        /// </summary>
        public TableCondtion tabCondtion { get; set; }
    }
    #endregion

    #region 自定义表的列来源表
    /// <summary>
    /// 自定义表的列来源表
    /// </summary>
    [Serializable]
    public class DefineSoureTable
    {
        // 唯一标识
        private string _colId = Guid.NewGuid().ToString();

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string colId
        {
            set { _colId = value; }
            get { return _colId; }
        }

        /// <summary>
        /// 来源列
        /// </summary>
        public string colName { get; set; }

        /// <summary>
        /// 列备注
        /// </summary>
        public string colComments { get; set; }

        /// <summary>
        /// 来源表
        /// </summary>
        public string tabName { get; set; }

        /// <summary>
        /// 表备注
        /// </summary>
        public string tabComments { get; set; }
    }
    #endregion

    #region 表单元素
    /// <summary>
    /// 表单元素
    /// </summary>
    [Serializable]
    public class FromItems
    {

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string colId { set; get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 显示名称
        /// </summary>
        public string fromName { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string fromParam { get; set; }
        
        /// <summary>
        /// 显示类型
        /// </summary>
        public string fromType { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public decimal maxLength { get; set; }

        /// <summary>
        /// 是否空
        /// </summary>
        public string isNull { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string colName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string colComments { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string colType { get; set; }

    }
    #endregion

    #region 表名、大小、条数实体
    /// <summary>
    /// 表名、大小、条数实体
    /// </summary>
    [Serializable]
    public class SoureTable
    {

        /// <summary>
        /// 表名
        /// </summary>
        public string tabName { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public decimal tabSize { get; set; }

        /// <summary>
        /// 条数
        /// </summary>
        public long tabCount { get; set; }

        /// <summary>
        /// 表条件
        /// </summary>
        public TableCondtion tabCondtion { set; get; } = new TableCondtion();

        /// <summary>
        /// 目标表
        /// </summary>
        public string tarGetTab { get; set; }
    }
    #endregion

    #region 数据库连接
    /// <summary>
    /// 数据库连接
    /// </summary>
    [Serializable]
    public class DataLink
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string dbType { set; get; } = "Oracle";

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { set; get; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string userPwd { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string port { set; get; } = "1521";

        /// <summary>
        /// 主机名
        /// </summary>
        public string hostName { get; set; }

        /// <summary>
        /// 服务名 数据库名 值
        /// </summary>
        public string serverValue { set; get; } = "ORCL";

        /// <summary>
        /// 服务名 数据库名 名
        /// </summary>
        public string serverName { set; get; } = "服务名：";

        /// <summary>
        /// 连接串
        /// </summary>
        public string connStr { get; set; }
        
        /// <summary>
        /// 连接名
        /// </summary>
        public string linkName { get; set; }
    }
    #endregion

    #region 迁移数据条件
   [Serializable]
    public class TableCondtion
    {

        /// <summary>
        /// 主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public bool IsIndex { get; set; }

        /// <summary>
        /// 表空间
        /// </summary>
        public bool IsTableSpace { get; set; }
                
        /// <summary>
        /// 建表
        /// </summary>
        public bool IsCreateTable { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string where { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public bool IsMoveData { set; get; } = true;

        /// <summary>
        /// 每次批量导入数据条数
        /// </summary>
        public long count { set; get; } = 100000;
    }
    #endregion

    #region 迁移数据结果
    [Serializable]
    public class DataResult
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public State index { get; set; }

        /// <summary>
        /// 表空间
        /// </summary>
        public State tableSpace { get; set; }

        /// <summary>
        /// 建表
        /// </summary>
        public State createTable { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public State key { get; set; }
        
        /// <summary>
        /// 数据条数
        /// </summary>
        public State data { get; set; }
    }   
    #endregion

    #region 成功与失败实体
    [Serializable]
    public class State
    {
        /// <summary>
        /// 成功
        /// </summary>
        public long success { get; set; }

        /// <summary>
        /// 失败
        /// </summary>
        public long fail { get; set; }
    }
    #endregion

    #region 主键实体
    [Serializable]
    public class TableKey
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string colName { get; set; }

        /// <summary>
        /// 主键名
        /// </summary>
        public string keyName { get; set; }
    }
    #endregion

    #region 索引实体
    [Serializable]
    public class TableIndex
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string colName { get; set; }

        /// <summary>
        /// 索引类型
        /// </summary>
        public string indexType { get; set; }

        /// <summary>
        /// 索引名
        /// </summary>
        public string indexName { get; set; }
    }
    #endregion

    #region 分页实体
    [Serializable]
    public class PageModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { get; set; }
        
        /// <summary>
        /// 页数
        /// </summary>
        public long pageCount { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public long pageId { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public long pageSize { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 当前条数
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string where { get; set; }
    }
    #endregion

    #region 源表与目标表对应列
    [Serializable]
    public class ColTargetSource
    {
        /// <summary>
        /// 目标表名
        /// </summary>
        public string targetTableName { get; set; }

        /// <summary>
        /// 目标列名
        /// </summary>
        public string targetColName { get; set; }

        /// <summary>
        /// 显示目标列名和类型
        /// </summary>
        public string targetShowColName { get; set; }

        /// <summary>
        /// 源表名
        /// </summary>
        public string sourceTableName { get; set; }

        /// <summary>
        /// 源列名
        /// </summary>
        public string sourceColName { get; set; }

        /// <summary>
        /// 显示源列名和类型
        /// </summary>
        public string sourceShowColName { get; set; }
    }
    #endregion

    #region 预先加载列实体
    [Serializable]
    public class LoadColumn
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        public DataLink link { get; set; }
    }
    #endregion
}
