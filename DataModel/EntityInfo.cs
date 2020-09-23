using System;
using System.Collections.Generic;

namespace DataModel
{
    /// <summary>
    /// 传给T4实体
    /// </summary>
    [Serializable]
    public class EntityInfo
    {
        #region 传给T4实体
        /// <summary>
        /// 列 list
        /// </summary>
        public List<BaseColumn> columns { get; set; }
        
        /// <summary>
        /// 是否序列化
        /// </summary>
        public bool isSerialize { get; set; }
        
        /// <summary>
        /// 是否map
        /// </summary>
        public bool isMap { get; set; }

        /// <summary>
        /// 是否验证
        /// </summary>
        public bool isCheck { get; set; }

        /// <summary>
        /// 是否model
        /// </summary>
        public bool isModel { get; set; }

        /// <summary>
        /// 是否old model
        /// </summary>
        public bool isOldModel { get; set; }

        /// <summary>
        /// 是否表单
        /// </summary>
        public bool isFrom { get; set; }
        
        /// <summary>
        /// 表
        /// </summary>
        public BaseTable table { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string nameSpace { get; set; }

        /// <summary>
        /// 生成实体路径
        /// </summary>
        public string modelFile { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 数据库类型参数标识符
        /// </summary>
        public string param { get; set; }
        
        /// <summary>
        /// 表单名称 
        /// </summary>
        public string fromName { get; set; }

        /// <summary>
        /// 表单URL 
        /// </summary>
        public string fromUrl { get; set; }

        /// <summary>
        /// 处理列类型
        /// </summary>
        /// <param name="col"></param>
        public List<BaseColumn> disColType(List<BaseColumn> list, string Language)
        {
            var rList = new List<BaseColumn>();
            list.ForEach(item =>
            {
                if (Language.ToUpper() == "C#")
                    item.colType = GetCsType(item.colType);
                else
                    item.colType = GetJavaType(item.colType);

                if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "decimal")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "byte")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "short")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "int")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "long")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "float")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData) && item.colType == "double")
                    item.defaultData = string.Format("={0};", item.defaultData);
                else if (!string.IsNullOrEmpty(item.defaultData))
                    item.defaultData = string.Format("=\"{0}\";", item.defaultData);
                else
                    item.defaultData = "";

                if (item.isKey)
                    item.colComments = string.Format("(主键){0}", item.colComments);

                if (item.isIndex)
                    item.colComments = string.Format("(索引){0}", item.colComments);

                if (item.colType == "decimal" || item.colType == "byte" || item.colType == "short" || item.colType == "int"
                                || item.colType == "TimeSpan" || item.colType == "long" || item.colType == "float" || item.colType == "double"
                                || item.colType == "bool" || item.colType == "DateTime" || item.colType == "Guid")
                    item.showNull = "?";
                else
                    item.showNull = "";

                item.maxMessage = "{0}最大长度" + item.colLength;

                if (item.isNull == "否")
                    item.requiredMessage = "[Required(ErrorMessage = \"{0}不能为空\")]";
                else
                    item.requiredMessage = "";

                rList.Add(item);
            });
            return rList;
        }
        #endregion

        #region 列类型Cs
        /// <summary>
        /// 列类型cs
        /// </summary>
        /// <param name="colType"></param>
        /// <returns></returns>
        private string GetCsType(string colType)
        {
            switch (colType.ToLower())
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "clob":
                case "long":
                case "nclob":
                case "nvarchar2":
                case "rowid":
                case "varchar2":
                case "meduimtext":
                case "tinytext":
                case "longtext":
                case "enum":
                case "set":  
                    return "string";

                case "smallmoney":
                case "numeric":
                case "integer":
                case "number":
                case "money":
                case "decimal":
                        return "decimal";

                case "tinyint":
                        return "byte";

                case "smallint":
                        return "short";

                case "int":
                case "mediumint":
                case "interval year to month":
                        return "int";

                case "interval day to second":
                        return "TimeSpan";

                case "bigint":
                        return "long";

                case "real":
                        return "float";

                case "float":
                case "double":
                        return "double";

                case "bfile":
                case "blob":
                case "long raw":
                case "raw":
                case "image":
                case "binary":
                case "nrowid":
                case "longblob":
                case "mediumblob":
                case "tityblob":
                    return "byte[]";

                case "bit":
                    return "bool";

                case "datetime":
                case "date":
                case "timestamp":
                case "time":
                case "year":
                    return "DateTime";

                case "uniqueidentifier":
                    return "Guid";
                default:
                    return colType;                    
            }
        }
        #endregion
        
        #region 列类型java
        /// <summary>
        /// 列类型java
        /// </summary>
        /// <param name="colType"></param>
        /// <returns></returns>
        private string GetJavaType(string colType)
        {
            switch (colType.ToLower())
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "clob":
                case "long":
                case "nclob":
                case "nvarchar2":
                case "rowid":
                case "varchar2":
                case "meduimtext":
                case "tinytext":
                case "longtext":
                case "enum":
                case "set":  
                    return "String";

                case "float":
                    return "float";

                case "bigint":
                case "number":
                    return "Long";

                case "tinyint":
                    return "byte";

                case "smallint":
                case "mediumint":
                    return "short";

                case "integer":
                case "int":
                    return "int";

                case "numeric":
                case "smallmoney":
                case "real":
                case "money":
                case "decimal":
                    return "double";

                case "bfile":
                case "blob":
                case "long raw":
                case "raw":
                case "image":
                case "binary":
                case "nrowid":
                case "longblob":
                case "mediumblob":
                case "tityblob":
                    return "byte[]";

                case "bit":
                    return "boolean";

                case "datetime":
                case "date":
                case "timestamp":
                case "time":
                case "year":
                    return "Date";

                case "uniqueidentifier":
                    return "Guid";
                default:
                    return colType;
            }
        }
        #endregion
    }
}
