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

    #region 数据库连接
    /// <summary>
    /// 数据库连接
    /// </summary>
    [Serializable]
    public class DataLink
    {
        // 数据库类型
        private string _dbType = "Oracle";

        // 端口
        private string _port = "1521";

        // 服务名 数据库名 值
        private string _serverValue = "ORCL";

        // 服务名 数据库名 名
        private string _serverName = "服务名：";

        private string _userName = "";

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string dbType
        {
            set { _dbType = value; }
            get { return _dbType; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName
        {
            set { _userName = value; }
            get { return _userName; }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string userPwd { get; set; }
        
        /// <summary>
        /// 端口
        /// </summary>
        public string port 
        {
            set { _port = value; }
            get { return _port; }
        }
        
        /// <summary>
        /// 主机名
        /// </summary>
        public string hostName { get; set; }

        /// <summary>
        /// 服务名 数据库名 值
        /// </summary>
        public string serverValue
        {
            set { _serverValue = value; }
            get { return _serverValue; }
        }

        /// <summary>
        /// 服务名 数据库名 名
        /// </summary>
        public string serverName
        {
            set { _serverName = value; }
            get { return _serverName; }
        }

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
}
