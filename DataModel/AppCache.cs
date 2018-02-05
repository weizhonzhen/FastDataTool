using System.Collections.Generic;

namespace DataModel
{
    public class AppCache
    {
        private static int hours = 24;
        private static string title = "数据工具";

        //源数据库连接
        public static string GetTitle() { return title; }
        public static void SetTitle(string item) { title=item; }

        //源数据库连接
        public static DataLink GetSoureLink() { return DataCache.Get<DataLink>("soureLink"); }
        public static void SetSoureLink(DataLink item) { DataCache.Set<DataLink>("soureLink", item, hours); }

        //目标数据库连接
        public static DataLink GetTargetLink() { return DataCache.Get<DataLink>("targetLink"); }
        public static void SetTargetLink(DataLink item) { DataCache.Set<DataLink>("targetLink", item, hours); }

        //生成实体数据库连接
        public static DataLink GetBuildLink() { return DataCache.Get<DataLink>("buildLink"); }
        public static void SetBuildLink(DataLink item) { DataCache.Set<DataLink>("buildLink", item, hours); }

        //表list
        public static List<BaseTable> GetTableList() { return DataCache.Get<List<BaseTable>>("tableList")??new List<BaseTable>(); }
        public static void SetTableList(List<BaseTable> item) { DataCache.Set<List<BaseTable>>("tableList", item, hours); }

        //列list
        public static List<BaseColumn> GetColumnList() { return DataCache.Get<List<BaseColumn>>("columnList") ?? new List<BaseColumn>(); }
        public static void SetColumnList(List<BaseColumn> item) { DataCache.Set<List<BaseColumn>>("columnList", item, hours); }

        //自定义表
        public static BaseTable GetDefineTable() { return DataCache.Get<BaseTable>("defineTable"); }
        public static void SetDefineTable(BaseTable item) { DataCache.Set<BaseTable>("defineTable", item, hours); }

        //表单元素
        public static List<FromItems> GetFromList() { return DataCache.Get<List<FromItems>>("fromItem"); }
        public static void SetFromList(List<FromItems> item) { DataCache.Set<List<FromItems>>("fromItem", item, hours); }

        //自定义列
        public static List<BaseColumn> GetDefineColumnList() { return DataCache.Get<List<BaseColumn>>("defineColumnList") ?? new List<BaseColumn>(); }
        public static void SetDefineColumnList(List<BaseColumn> item) { DataCache.Set<List<BaseColumn>>("defineColumnList", item, hours); }

        //自定义表来源表
        public static List<DefineSoureTable> GetDefineSoureTable() { return DataCache.Get<List<DefineSoureTable>>("defineSoureTable") ?? new List<DefineSoureTable>(); }
        public static void SetDefineSoureTable(List<DefineSoureTable> item) { DataCache.Set<List<DefineSoureTable>>("defineSoureTable", item, hours); }

        //目标表
        public static List<BaseTable> GetTargetTable() { return DataCache.Get<List<BaseTable>>("targetTable") ?? new List<BaseTable>(); }
        public static void SetTargetTable(List<BaseTable> item) { DataCache.Set<List<BaseTable>>("targetTable", item, hours); }

        //源表
        public static List<SoureTable> GetSoureTable() { return DataCache.Get<List<SoureTable>>("soureTable") ?? new List<SoureTable>(); }
        public static void SetSoureTable(List<SoureTable> item) { DataCache.Set<List<SoureTable>>("soureTable", item, hours); }

        //迁移结果
        public static List<DataResult> GetResult() { return DataCache.Get<List<DataResult>>("result") ?? new List<DataResult>(); }
        public static void SetResult(List<DataResult> item) { DataCache.Set<List<DataResult>>("result", item, hours); }
        
        //列对应
        public static List<ColTargetSource> GetColList() { return DataCache.Get<List<ColTargetSource>>("colList")??new List<ColTargetSource>();  }
        public static void SetColList(List<ColTargetSource> item) { DataCache.Set<List<ColTargetSource>>("colList", item, hours); }
        
        //表结构的列
        public static List<BaseColumn> GetTableColumn(string key) { return DataCache.Get<List<BaseColumn>>(key) ?? new List<BaseColumn>(); }
        public static void SetTableColumn(List<BaseColumn> item, string key) { DataCache.Set<List<BaseColumn>>(key, item, hours); }
        public static bool ExistsTableColumn(string key) { return DataCache.Exists(key); }
        
    }
}
