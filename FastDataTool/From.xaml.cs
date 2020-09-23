using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.ComponentModel;

namespace FastDataTool
{
    /// <summary>
    /// From.xaml 的交互逻辑
    /// </summary>
    public partial class From : Window
    {
        //生成路径
        public string txtFile = "";

        public From()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }
        
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
            Common.CheckAllBox((sender as CheckBox), fromTable, "fromBox");
        }
        #endregion
        
        #region 删除多个
        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelMore_Item(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var listFrom = AppCache.GetFromList();
            foreach (var item in fromTable.Items)
            {
                var box = Common.GetTemplateColumn<CheckBox>(fromTable, 0, "fromBox", item);
                if (box != null && box.IsChecked == true)
                {
                    count++;
                    var colItem = item as FromItems;                           
                    listFrom.RemoveAll(a => a.colId == colItem.colId); 
                }
            }

            AppCache.SetFromList(listFrom);

            if (count == 0)
                CodeBox.Show("请选择要删除的列", this);
            else
            {
                fromTable.ItemsSource = AppCache.GetFromList();
                Common.DataGridSort(fromTable, "tabName", ListSortDirection.Ascending);
            }
        }
        #endregion
         
        #region 生成表单
        /// <summary>
        /// 生成表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build_From(object sender, RoutedEventArgs e)
        {
            var info = new EntityInfo();
            var list = new List<BaseColumn>();
            
            AppCache.GetFromList().ForEach(a => {
                var temp = new BaseColumn();
                temp.colId = a.colId;
                temp.fromName = a.fromName;
                temp.fromParam = a.fromParam;
                temp.fromType = a.fromType ?? "text";
                temp.colLength = a.maxLength;
                temp.isNull = a.isNull;
                temp.colComments = a.colComments;
                temp.colName = a.colName;
                temp.colType = a.colType;
                list.Add(temp);
            });

            info.columns = list;
            info.modelFile = txtFile;
            info.isFrom = true;
            info.fromName = fromName.Text;
            info.fromUrl = fromUrl.Text;
            info.language = "C#";

            var table = new BaseTable();
            table.tabName = tabName.Text;
            table.tabComments = "";
            info.table = table;
            info.nameSpace = nameSpace.Text;

            var count = Common.BuildCodeModel(info, "cshtml") == true ? 1 : 0;

            info.isFrom = false;
            info.isCheck = true;
            info.columns = info.disColType(info.columns, info.language);
            count += Common.BuildCodeModel(info) == true ? 1 : 0;

            if (count == 2)
            {
                CodeBox.Show("生成成功", this);
                AppCache.SetFromList(null);
            }
            else
                CodeBox.Show("生成失败", this);
        }
        #endregion
        
        #region 编辑表单元素
        /// <summary>
        /// 编辑表单元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromTable_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var item = e.Row.Item as FromItems;
            var fromList = AppCache.GetFromList();

            var colItem = fromList.Find(a => a.colId == item.colId);

            fromList.Remove(colItem);

            colItem.fromName = item.fromName;

            fromList.Add(colItem);

            AppCache.SetFromList(fromList);
        }
        #endregion

        #region 表单类型选择
        /// <summary>
        /// 表单类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fromTable.SelectedItem != null)
            {
                var fromList = AppCache.GetFromList();
                var item = fromTable.SelectedItem as FromItems;

                var colItem = fromList.Find(a => a.colId == item.colId);

                fromList.Remove(colItem);

                colItem.fromName = item.fromName;
                colItem.fromType = ((sender as ComboBox).SelectedValue as ComboBoxItem).Content.ToString();

                fromList.Add(colItem);

                AppCache.SetFromList(fromList);
            }
        }
        #endregion
    }
}
