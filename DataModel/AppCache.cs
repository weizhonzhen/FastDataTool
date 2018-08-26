using System.Collections.Generic;

namespace DataModel
{
    public class AppCache
    {
        private static string title = "Fast数据工具";

        private static string GetTableKey(DataLink link) { return string.Format("tableList_{0}_{1}_{2}", link.hostName, link.serverName, link.serverValue); }
        private static string GetViewKey(DataLink link) { return string.Format("viewList_{0}_{1}_{2}", link.hostName, link.serverName, link.serverValue); }
        
        //源数据库连接
        public static string GetTitle() { return title; }
        public static void SetTitle(string item) { title=item; }

        //生成实体数据库连接
        public static DataLink GetBuildLink() { return DataCache.Get<DataLink>("buildLink"); }
        public static void SetBuildLink(DataLink item) { DataCache.Set<DataLink>("buildLink", item); }

        //表list
        public static List<BaseTable> GetTableList(DataLink link) { return DataCache.Get<List<BaseTable>>(GetTableKey(link)) ??new List<BaseTable>(); }
        public static void SetTableList(List<BaseTable> item, DataLink link) { DataCache.Set<List<BaseTable>>(GetTableKey(link), item); }
        public static bool ExistsTable(DataLink link) { return DataCache.Exists(GetTableKey(link)); }

        //视图list
        public static List<BaseTable> GetViewList(DataLink link) { return DataCache.Get<List<BaseTable>>(GetViewKey(link)) ?? new List<BaseTable>(); }
        public static void SetViewList(List<BaseTable> item, DataLink link) { DataCache.Set<List<BaseTable>>("viewList", item); }
        public static bool ExistsView(DataLink link) { return DataCache.Exists(GetViewKey(link)); }

        //表单元素
        public static List<FromItems> GetFromList() { return DataCache.Get<List<FromItems>>("fromItem"); }
        public static void SetFromList(List<FromItems> item) { DataCache.Set<List<FromItems>>("fromItem", item); }

        //自定义列
        public static List<BaseColumn> GetDefineColumnList() { return DataCache.Get<List<BaseColumn>>("defineColumnList") ?? new List<BaseColumn>(); }
        public static void SetDefineColumnList(List<BaseColumn> item) { DataCache.Set<List<BaseColumn>>("defineColumnList", item); }

        //自定义表来源表
        public static List<DefineSoureTable> GetDefineSoureTable() { return DataCache.Get<List<DefineSoureTable>>("defineSoureTable") ?? new List<DefineSoureTable>(); }
        public static void SetDefineSoureTable(List<DefineSoureTable> item) { DataCache.Set<List<DefineSoureTable>>("defineSoureTable", item); }        
        
        //表结构的列
        public static List<BaseColumn> GetTableColumn(string key) { return DataCache.Get<List<BaseColumn>>(key) ?? new List<BaseColumn>(); }
        public static void SetTableColumn(List<BaseColumn> item, string key) { DataCache.Set<List<BaseColumn>>(key, item); }
        public static bool ExistsTableColumn(string key) { return DataCache.Exists(key); }
        
    }
}
