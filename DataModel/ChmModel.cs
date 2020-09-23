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
    
            list.ForEach(a => {
                if (a.isKey)
                    a.colComments = string.Format("(主键){0}", a.colComments);

                if (a.isIndex)
                    a.colComments = string.Format("(索引){0}", a.colComments);

                if (a.colType == "decimal" || a.colType == "byte" || a.colType == "short" || a.colType == "int"
                                || a.colType == "TimeSpan" || a.colType == "long" || a.colType == "float" || a.colType == "double"
                                || a.colType == "bool" || a.colType == "DateTime" || a.colType == "Guid")
                    a.showNull = "?";
                else
                    a.showNull = "";

                rList.Add(a);
            });
            return rList;
        }
    }
}
