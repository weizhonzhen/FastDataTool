using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.Threading;

namespace Data
{
    /// <summary>
    /// DataMove_2.xaml 的交互逻辑
    /// </summary>
    public partial class DataMove_2 : Window
    {
        public DataMove_2()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }

        #region CheckBox 全选
        /// <summary>
        /// CheckBox 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            Common.CheckAllBox((sender as CheckBox), TableGrid, "tabCheckBox");

            foreach (var item in TableGrid.Items)
            {
                BindComboBox(item);
            }
        }
        #endregion

        #region 选中checkbox
        /// <summary>
        /// 选中checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabBox_Click(object sender, RoutedEventArgs e)
        {
            BindComboBox(TableGrid.SelectedItem);
        }
        #endregion

        #region 绑定datagrid中combobox
        /// <summary>
        /// 绑定datagrid中combobox
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="selectedItem"></param>
        private void BindComboBox(object selectedItem)
        {
            var sourceItem = selectedItem as SoureTable;
            var box = Common.GetTemplateColumn<CheckBox>(TableGrid, 0, "tabCheckBox", selectedItem);
            if (box != null && box.IsChecked == true)
            {
                var boBox = Common.GetTemplateColumn<ComboBox>(TableGrid, 4, "tabComBox", selectedItem);
                if (boBox != null && boBox.ItemsSource == null)
                {
                    var list = AppCache.GetSoureTable();
                    if (list.Count != 0)
                    {
                        boBox.ItemsSource = list;
                        list.Find(a => a.tabName == sourceItem.tabName).tabCondtion.IsCreateTable = true;
                        list.Find(a => a.tabName == sourceItem.tabName).tabCondtion.IsKey = true;
                        list.Find(a => a.tabName == sourceItem.tabName).tabCondtion.IsIndex = true;
                        list.Find(a => a.tabName == sourceItem.tabName).tabCondtion.IsTableSpace = true;
                        AppCache.SetSoureTable(list);
                    }
                }
            }           
            else
            {
                var boBox = Common.GetTemplateColumn<ComboBox>(TableGrid, 4, "tabComBox", selectedItem);
                boBox.ItemsSource = null;
                var list = AppCache.GetSoureTable();
                list.Find(a => a.tabName == sourceItem.tabName).tabCondtion = new TableCondtion();
                AppCache.SetSoureTable(list);
            }
        }
        #endregion

        #region 批量设定条件
        /// <summary>
        /// 批量设定条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CondtionMore_Click(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var table = new SoureTable();
            var form = new DataMove_Condtion();

            foreach(var item in TableGrid.Items)
            {
                if (GetCondtionTable(item, ref table))
                {
                    count++;
                    form.sourceTabList.Add(table);
                }
            }

            if (count == 0)
                CodeBox.Show("请选择表", this);
            else
            {
                form.colGrid.IsEnabled = false;
                form.tabName.IsEnabled = false;
                form.SqlWhere.IsEnabled = false;
                form.tabCountBtn.IsEnabled = false;
                form.IsCreateTable.IsEnabled = false;
                Common.OpenWin(form, this);
            }
        }
        #endregion

        #region 单个设定条件
        /// <summary>
        /// 单个设定条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Condtion_Click(object sender, RoutedEventArgs e)
        {
            var form = new DataMove_Condtion();
            var table = new SoureTable();
            var coList = new List<ColTargetSource>();
            
            if (GetCondtionTable(TableGrid.SelectedItem, ref table))
            {
                form.IsCreateTable.IsEnabled = false;
                form.tabName.IsEnabled = false;
                form.sourceTabList.Add(table);             
                   
                form.tabName.Text = table.tarGetTab;
                form.isKey.IsChecked = table.tabCondtion.IsKey;
                form.IsIndex.IsChecked = table.tabCondtion.IsIndex;
                form.IsCreateTable.IsChecked = table.tabCondtion.IsCreateTable;
                form.IsTableSpace.IsChecked = table.tabCondtion.IsTableSpace;
                form.IsMoveData.IsChecked = table.tabCondtion.IsMoveData;

                //不创建表
                if (!table.tabCondtion.IsCreateTable)
                {
                    form.isKey.IsEnabled = false;
                    form.IsIndex.IsEnabled = false;
                    form.IsTableSpace.IsEnabled = false;
                }
                else
                    form.colGrid.IsEnabled = false;

                BindCol(ref form, table);

                Common.OpenWin(form, this);                
            }
            else
                CodeBox.Show("请选择表", this);           
        }
        #endregion

        #region 获取设置条件表
        /// <summary>
        /// 获取设置条件表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool GetCondtionTable(object item,ref SoureTable table)
        {
            var refValue = true;
            var box = Common.GetTemplateColumn<CheckBox>(TableGrid, 0, "tabCheckBox", item);
            if (box != null && box.IsChecked == true)
            {
                table = AppCache.GetSoureTable().Find(a => a.tabName == (item as SoureTable).tabName);
                if (!table.tabCondtion.IsCreateTable)
                {
                    table.tabCondtion.IsIndex = false;
                    table.tabCondtion.IsKey = false;
                    table.tabCondtion.IsTableSpace = false;
                }
                else
                    table.tarGetTab = table.tabName;
            }
            else
                refValue = false;

            return refValue;
        }
        #endregion
        
        #region 目标表选择中
        /// <summary>
        /// 目标表选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_Selected(object sender, RoutedEventArgs e)
        {
            var boxItem = sender as ComboBox;

            if (boxItem.SelectedItem != null)
            {
                var list = AppCache.GetSoureTable();
                list.Find(a => a.tabName == (TableGrid.SelectedItem as SoureTable).tabName).tabCondtion.IsCreateTable = false;
                list.Find(a => a.tabName == (TableGrid.SelectedItem as SoureTable).tabName).tarGetTab = (boxItem.SelectedItem as SoureTable).tabName;
                AppCache.SetSoureTable(list);
            }
        }
        #endregion

        #region 执行迁移数据
        /// <summary>
        /// 执行迁移数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //线程池
            ThreadPool.SetMaxThreads(1000, 1000);

            //要迁移的表
            AppCache.SetResult(null);
            var count = 0;
            foreach (var item in TableGrid.Items)
            {
                var box = Common.GetTemplateColumn<CheckBox>(TableGrid, 0, "tabCheckBox", item);

                if (box != null && box.IsChecked == true)
                {
                    count++;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(run), item);
                }
            }
            
            while (true)
            {
                if (AppCache.GetResult().Count == count)
                {
                    var sb = new StringBuilder();

                    sb.AppendFormat("表成功：{0} 个，失败：{1} 个\n\r", AppCache.GetResult().Sum(a => a.createTable.success), AppCache.GetResult().Sum(a => a.createTable.fail));
                    sb.AppendFormat("索引成功：{0} 个，失败：{1} 个\n\r", AppCache.GetResult().Sum(a => a.index.success), AppCache.GetResult().Sum(a => a.index.fail));
                    sb.AppendFormat("主键成功：{0} 个，失败：{1} 个\n\r", AppCache.GetResult().Sum(a => a.key.success), AppCache.GetResult().Sum(a => a.key.fail));
                    sb.AppendFormat("表空间成功：{0} 个，失败：{1} 个\n\r", AppCache.GetResult().Sum(a => a.tableSpace.success), AppCache.GetResult().Sum(a => a.tableSpace.fail));
                    sb.AppendFormat("数据成功：{0} 条，失败：{1} 条\n\r", AppCache.GetResult().Sum(a => a.data.success), AppCache.GetResult().Sum(a => a.data.fail));

                    ShowResult.Show(sb.ToString(), this);
                    break;
                }
                else
                    Thread.Sleep(1000 * 5);
            }
        }
        #endregion
        
        #region 迁移数据线程
        /// <summary>
        /// 迁移数据线程
        /// </summary>
        private void run(object temp)
        {
            var item = temp as SoureTable;
            var result = new DataResult();
            var refValue = false;
            var list = AppCache.GetResult();

            //表名
            result.tableName = String.IsNullOrEmpty(item.tarGetTab) ? item.tabName : item.tarGetTab;
            item.tarGetTab = result.tableName;

            //创建表
            if (item.tabCondtion.IsCreateTable)
            {
                refValue = DataCreate.CopyTable(AppCache.GetTargetLink(), AppCache.GetSoureLink(), item);
                result.createTable = Common.Calculation(refValue);
            }
            else
                result.createTable = new State();

            //创建索引
            if (item.tabCondtion.IsIndex && refValue)
                result.index = DataCreate.CreateIndex(AppCache.GetTargetLink(), AppCache.GetSoureLink(), item);
            else
                result.index = new State();

            //创建主键
            if (item.tabCondtion.IsKey && refValue)
                result.key = DataCreate.CreateKey(AppCache.GetTargetLink(), AppCache.GetSoureLink(), item);
            else
                result.key = new State();

            //表空间
            if (item.tabCondtion.IsTableSpace && refValue)
                result.tableSpace = DataCreate.CreateTableSpace(AppCache.GetTargetLink(), AppCache.GetSoureLink(), item);
            else
                result.tableSpace = new State();
            
            //数据
            if(item.tabCondtion.IsMoveData)
                result.data = DataCreate.AddList(AppCache.GetTargetLink(), AppCache.GetSoureLink(), item);

            list.Add(result);
            AppCache.SetResult(list);
            Thread.CurrentThread.Abort();
        }
        #endregion

        #region 绑定对应字段数据
        /// <summary>
        /// 绑定对应字段数据
        /// </summary>
        private void BindCol(ref DataMove_Condtion form, SoureTable table)
        {
            var boolFlag = true;
            if (!table.tabCondtion.IsCreateTable)
            {
                if (!AppCache.GetColList().Exists(a => a.targetTableName == table.tarGetTab))
                {
                    boolFlag = false;
                    var targetList = DataSchema.ColumnList(AppCache.GetTargetLink(), table.tarGetTab);
                    var list = AppCache.GetColList();

                    foreach (var temp in targetList)
                    {
                        var item = new ColTargetSource();
                        item.sourceTableName = table.tabName;
                        item.targetTableName = table.tarGetTab;
                        item.targetColName = temp.colName;
                        item.targetShowColName = string.Format("{0} [ {1} ]", temp.colName, temp.showType); 
                        list.Add(item);
                    }

                    AppCache.SetColList(list);
                }

                form.colGrid.ItemsSource = AppCache.GetColList().FindAll(a => a.targetTableName == table.tarGetTab);
                
                if(boolFlag)
                    form.colGrid.Loaded += new RoutedEventHandler(form.Box_Selected_Test);
            }
        }
        #endregion 
    }
}