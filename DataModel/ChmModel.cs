using System.Collections.Generic;

namespace DataModel
{
    public class ChmModel
    {
        /// <summary>
        /// 列 list
        /// </summary>
        public List<BaseColumn> columns { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string tabName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string tabComments { get; set; }

        /// <summary>
        /// 处理列类型
        /// </summary>
        /// <param name="col"></param>
        public List<BaseColumn> disColType(List<BaseColumn> list)
        {
            var rList = new List<BaseColumn>();
            foreach (var item in list)
            {
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

                rList.Add(item);
            }
            return rList;
        }
    }
}
