using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;

namespace Data
{
    /// <summary>
    /// Condtion.xaml 的交互逻辑
    /// </summary>
    public partial class DataMove_Condtion : Window
    {
        public List<SoureTable> sourceTabList = new List<SoureTable>();

        #region 加载
        /// <summary>
        /// 加载
        /// </summary>
        public DataMove_Condtion()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }
        #endregion
        
        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
             var reg = new Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
             bool ismatch = reg.IsMatch(txtCount.Text.Trim());
             
            if(!ismatch)
            {
                CodeBox.Show("请输入正确的数据！", this);
                return;
            }

            var list = AppCache.GetSoureTable();
            foreach (var item in sourceTabList)
            {
                if (tabName.IsEnabled)
                    item.tarGetTab = tabName.Text.Trim();

                list.Find(a => a.tabName == item.tabName).tabCondtion.IsKey = (bool)isKey.IsChecked;
                list.Find(a => a.tabName == item.tabName).tabCondtion.IsTableSpace = (bool)IsTableSpace.IsChecked;
                list.Find(a => a.tabName == item.tabName).tabCondtion.IsIndex = (bool)IsIndex.IsChecked;
                list.Find(a => a.tabName == item.tabName).tabCondtion.IsCreateTable = (bool)IsCreateTable.IsChecked;
                list.Find(a => a.tabName == item.tabName).tabCondtion.where = SqlWhere.Text.Trim();
                list.Find(a => a.tabName == item.tabName).tabCondtion.count = long.Parse(txtCount.Text.Trim());
                list.Find(a => a.tabName == item.tabName).tabCondtion.IsMoveData = (bool)IsMoveData.IsChecked;
            }

            AppCache.SetSoureTable(list);
            
            this.Close();
        }
        #endregion

        #region 过虑后数据条数
        /// <summary>
        /// 过虑后数据条数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dt = new DataTable();
            var sql = string.Format("select count(0) from {0} where {1}"
                                            , AppCache.GetSoureTable().Find(a => a.tarGetTab == tabName.Text.Trim()).tabName
                                            , String.IsNullOrEmpty(SqlWhere.Text.Trim()) ? "1=1" : SqlWhere.Text.Trim());
            try
            {
                if (AppCache.GetSoureLink().dbType == DataDbType.Oracle)
                {
                    using (var conn = new OracleConnection(AppCache.GetSoureLink().connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = sql;
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                    }
                }
                else if (AppCache.GetSoureLink().dbType == DataDbType.SqlServer)
                {
                    using (var conn = new SqlConnection(AppCache.GetSoureLink().connStr))
                    {
                        conn.Open();
                        var cmd = conn.CreateCommand();
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = sql;
                        var rd = cmd.ExecuteReader();
                        dt.Load(rd);
                        rd.Close();
                    }
                }

                CodeBox.Show(string.Format("共 {0} 条", dt.Rows[0][0].ToString()), this); 
            }
            catch
            {
                CodeBox.Show("条件 sql 语句不正确", this);
            }
        }
        #endregion        

        #region 绑定源列
        /// <summary>
        /// 绑定源列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Selected(object sender, RoutedEventArgs e)
        {
            var item = (sender as DataGrid).SelectedItem as ColTargetSource;

            if (item != null)
            {
                var box = Common.GetTemplateColumn<ComboBox>(colGrid, 1, "colComBox", (sender as DataGrid).SelectedItem);

                if (box.ItemsSource == null)
                {
                    var list = new List<BaseColumn>();
                    list.Add(new BaseColumn());
                    list.AddRange(DataSchema.ColumnList(AppCache.GetSoureLink(), item.sourceTableName));
                    box.ItemsSource = list;
                    box.SelectedItem = list.Find(a => a.colName == item.sourceColName);
                }
                else
                    box.SelectedItem = DataSchema.ColumnList(AppCache.GetSoureLink(), item.sourceTableName).Find(a => a.colName == item.sourceColName);
            }
        }
        #endregion
        
        #region 目标列选择中
        /// <summary>
        /// 目标列选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_Selected(object sender, RoutedEventArgs e)
        {
            var boxItem = sender as ComboBox;
            var item = colGrid.SelectedItem as ColTargetSource;

            if (boxItem.SelectedItem != null)
            {
                var baseItem = boxItem.SelectedItem as BaseColumn;
                var list = AppCache.GetColList();
                list.Find(a => a.targetTableName == item.targetTableName && a.targetColName == item.targetColName).sourceColName = baseItem.colName;
                list.Find(a => a.targetTableName == item.targetTableName && a.targetColName == item.targetColName).sourceShowColName = baseItem.showTypeColName;
                AppCache.SetColList(list);
            }
        }
        #endregion

        #region 目标列选择中
        /// <summary>
        /// 目标列选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Box_Selected_Test(object sender, RoutedEventArgs e)
        {
            foreach (var temp in colGrid.Items)
            {
                if ((temp as ColTargetSource).sourceColName != "")
                {
                    

                }
            }
        }
        #endregion
    }
}
