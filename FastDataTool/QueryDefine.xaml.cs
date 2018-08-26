using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.ComponentModel;

namespace FastDataTool
{
    /// <summary>
    /// QueryDefine.xaml 的交互逻辑
    /// </summary>
    public partial class QueryDefine : Window
    {
        //生成路径
        public string txtFile = "";

        public QueryDefine()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }

        #region 单个删除自定义表的列
        /// <summary>
        /// 单个删除自定义表的列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_Define(object sender, RoutedEventArgs e)
        {
            var item = DefineTable.SelectedItem as DefineSoureTable;
            var listTable = AppCache.GetDefineSoureTable();
            var listColumn = AppCache.GetDefineColumnList();

            listTable.RemoveAll(a => a.colId == item.colId);
            listColumn.RemoveAll(a => a.colId == item.colId);

            DefineTable.ItemsSource = listTable;

            AppCache.SetDefineColumnList(listColumn);
            AppCache.SetDefineSoureTable(listTable);

            Common.DataGridSort(DefineTable, "tabName", ListSortDirection.Ascending);  
        }
        #endregion

        #region 删除多个
        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelMore_Define(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var listTable = AppCache.GetDefineSoureTable();
            var listColumn = AppCache.GetDefineColumnList();

            foreach (var item in DefineTable.Items)
            {
                var box = Common.GetTemplateColumn<CheckBox>(DefineTable, 0, "defBox", item);
                if (box != null && box.IsChecked == true)
                {
                    count++;
                    var colItem = item as DefineSoureTable;

                    listTable.RemoveAll(a => a.colId == colItem.colId);
                    listColumn.RemoveAll(a => a.colId == colItem.colId);
                }
            }

            AppCache.SetDefineColumnList(listColumn);
            AppCache.SetDefineSoureTable(listTable);

            if (count == 0)
                CodeBox.Show("请选择要删除的列", this);
            else
            {
                DefineTable.ItemsSource = AppCache.GetDefineSoureTable();
                Common.DataGridSort(DefineTable, "tabName", ListSortDirection.Ascending);  
            }
        }
        #endregion

        #region 编辑自定义实体
        /// <summary>
        /// 编辑自定义实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefineTable_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //自定义实体
            var item = e.Row.Item as DefineSoureTable;
            var tableList = AppCache.GetDefineSoureTable();
            var columnList = AppCache.GetDefineColumnList();

            if (tableList.Remove(tableList.Find(a => a.colId == item.colId)))
                tableList.Add(item);

            //自定义列
            var colItem = columnList.Find(a => a.colId == item.colId);
            columnList.Remove(colItem);
            colItem.colName = item.colName;
            colItem.colComments = item.colComments;
            columnList.Add(colItem);

            AppCache.SetDefineColumnList(columnList);
            AppCache.SetDefineSoureTable(tableList);
        }
        #endregion

        #region  选择目录
        /// <summary>
        /// 选择目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtFile = Common.FolderBrowserDialog();
        }
        #endregion
        
        #region CheckBox 全选
        /// <summary>
        /// CheckBox 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAll_defClick(object sender, RoutedEventArgs e)
        {
            Common.CheckAllBox((sender as CheckBox), DefineTable, "defBox");
        }
        #endregion

        #region 生成实体
        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build_Define(object sender, RoutedEventArgs e)
        {
            var table = new BaseTable();
            var info = new EntityInfo();

            table.tabComments = txtComments.Text.Trim();
            table.tabName = txtName.Text.Trim();

            info.columns = AppCache.GetDefineColumnList();
            info.modelFile = txtFile;
            info.nameSpace = txtNameSpace.Text.Trim();
            info.table = table;
            info.language = language.SelectionBoxItem.ToString();
            info.isSerialize = (bool)isSerialize.IsChecked;
            info.isCheck = (bool)isCheck.IsChecked;
            info.isModel = (bool)isModel.IsChecked;

            if (!info.isCheck && !info.isSerialize && !info.isMap && !info.isModel)
                CodeBox.Show("请选择模板", this);

            info.columns = info.disColType(info.columns, info.language);

            if (Common.BuildCodeModel(info))
            {
                CodeBox.Show("生成成功", this);
                AppCache.SetDefineSoureTable(null);
                AppCache.SetDefineColumnList(null);
            }
            else
                CodeBox.Show("生成失败", this);
        }
        #endregion
    }
}
