using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace FastDataTool
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    public partial class MainWindow : Window
    {
        //生成路径
        public string txtFile = "";

        //成功、失败数量
        public int success = 0, total = 0, colNext = 0, tabNext = 0;

        #region 加载
        /// <summary>
        /// 加载
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Common.InitWindows(this, ResizeMode.CanMinimize, WindowStartupLocation.CenterScreen);

            #region 托盘初始化
            var notifyIcon = new NotifyIcon();
            AppCache.SetTitle(this.Title);
            notifyIcon.BalloonTipText = AppCache.GetTitle();
            notifyIcon.Text = AppCache.GetTitle();

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.Visible = true;

            //打开菜单项
            var open = new System.Windows.Forms.MenuItem(string.Format("打开{0}", AppCache.GetTitle()));
            open.Click += new EventHandler(Show);

            //退出菜单项
            var exit = new System.Windows.Forms.MenuItem(string.Format("退出{0}", AppCache.GetTitle()));
            exit.Click += new EventHandler(Close);

            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            //双击
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) => { if (e.Button == MouseButtons.Left) this.Show(o, e); });
            #endregion
        }
        #endregion

        #region 打开数据工具
        /// <summary>
        /// 打开数据工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }
        #endregion

        #region 隐藏数据工具
        /// <summary>
        /// 隐藏数据工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        #region 退出数据工具
        /// <summary>
        /// 退出数据工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region 显示表结构
        /// <summary>
        /// 显示表结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_Table(object sender, RoutedEventArgs e)
        {
            if (!AppCache.ExistsTable(AppCache.GetBuildLink()))
                AppCache.SetTableList(DataSchema.TableList(AppCache.GetBuildLink(), "loadColumnList") ?? new List<BaseTable>(), AppCache.GetBuildLink());
            Dtable.DataContext = AppCache.GetTableList(AppCache.GetBuildLink());
        }
        #endregion

        #region 更新表结构
        /// <summary>
        /// 更新表结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReLoad_Table(object sender, RoutedEventArgs e)
        {
            AppCache.SetTableList(DataSchema.TableList(AppCache.GetBuildLink(), "loadColumnList", true) ?? new List<BaseTable>(), AppCache.GetBuildLink());
            Dtable.DataContext = AppCache.GetTableList(AppCache.GetBuildLink());
        }
        #endregion

        #region 显示视图结构
        /// <summary>
        /// 显示视图结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_View(object sender, RoutedEventArgs e)
        {
            if (!AppCache.ExistsView(AppCache.GetBuildLink()))
                AppCache.SetViewList(DataSchema.ViewList(AppCache.GetBuildLink(), "loadColumnList", true) ?? new List<BaseTable>(), AppCache.GetBuildLink());
            Dtable.DataContext = AppCache.GetViewList(AppCache.GetBuildLink());
        }
        #endregion

        #region 更新视图结构
        /// <summary>
        /// 更新视图结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReLoad_View(object sender, RoutedEventArgs e)
        {
            AppCache.SetViewList(DataSchema.ViewList(AppCache.GetBuildLink(), "loadColumnList") ?? new List<BaseTable>(), AppCache.GetBuildLink());
            Dtable.DataContext = AppCache.GetViewList(AppCache.GetBuildLink());
        }
        #endregion

        #region 表CheckBox 全选
        /// <summary>
        /// 表CheckBox 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAll_tabClick(object sender, RoutedEventArgs e)
        {
            Common.CheckAllBox((sender as System.Windows.Controls.CheckBox), Dtable, "tabBox");
        }
        #endregion

        #region 列CheckBox 全选
        /// <summary>
        /// 列CheckBox 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAll_colClick(object sender, RoutedEventArgs e)
        {
            Common.CheckAllBox((sender as System.Windows.Controls.CheckBox), Dcolumn, "colBox");
        }
        #endregion

        #region 显示列结构
        /// <summary>
        /// 显示列结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dtable_Selected(object sender, RoutedEventArgs e)
        {
            var item = (sender as System.Windows.Controls.DataGrid).SelectedItem as BaseTable;

            if (item != null)
            {
                //列checkbox全选为不选择
                var tempCol = Dcolumn.Columns[0] as DataGridTemplateColumn;
                var box = tempCol.Header as System.Windows.Controls.CheckBox;
                box.IsChecked = false;

                //绑定新列的数据源
                Dcolumn.ItemsSource = DataSchema.ColumnList(AppCache.GetBuildLink(), item.tabName) ?? new List<BaseColumn>();
            }
        }
        #endregion

        #region 生成实体
        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build_Model(object sender, RoutedEventArgs e)
        {
            var list = new List<EntityInfo>();

            //生成list
            foreach (var item in Dtable.Items)
            {
                var box = Common.GetTemplateColumn<System.Windows.Controls.CheckBox>(Dtable, 0, "tabBox", item);

                if (box != null && box.IsChecked == true)
                {
                    var entiy = new EntityInfo();
                    entiy.table = item as BaseTable;
                    entiy.language = language.SelectionBoxItem.ToString();
                    entiy.columns = DataSchema.ColumnList(AppCache.GetBuildLink(), entiy.table.tabName) ?? new List<BaseColumn>();
                    entiy.columns = entiy.disColType(entiy.columns, entiy.language);
                    entiy.isSerialize = (bool)isSerialize.IsChecked;
                    entiy.isCheck = (bool)isCheck.IsChecked;
                    entiy.isMap = (bool)isMap.IsChecked;
                    entiy.isModel = (bool)isModel.IsChecked;
                    entiy.isOldModel = (bool)isOldModel.IsChecked;

                    if (DataDbType.Oracle == AppCache.GetBuildLink().dbType)
                        entiy.param = ":";

                    if (DataDbType.MySql == AppCache.GetBuildLink().dbType)
                        entiy.param = "?";

                    if (DataDbType.SqlServer == AppCache.GetBuildLink().dbType)
                        entiy.param = "@";

                    if (!entiy.isCheck && !entiy.isSerialize && !entiy.isMap && !entiy.isModel && !entiy.isOldModel)
                    {
                        CodeBox.Show("请选择模板", this);
                        return;
                    }

                    list.Add(entiy);
                }
            }

            if (list.Count == 0)
            {
                CodeBox.Show("请选择要生成表", this);
                return;
            }

            //执行生成实体           
            foreach (var item in list)
            {
                //命名空间
                item.nameSpace = txtNameSpace.Text.Trim();

                //生成路径
                if (txtFile != "")
                    item.modelFile = txtFile;

                //语言
                item.language = language.SelectionBoxItem.ToString();

                if (item.isMap)
                    Common.BuildCodeModel(item, "xml");

                else
                    Common.BuildCodeModel(item);
            }

            CodeBox.Show(string.Format("生成完成"), this);
        }
        #endregion

        #region  选择目录
        /// <summary>
        /// 选择目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chang_Directory(object sender, RoutedEventArgs e)
        {
            txtFile = Common.FolderBrowserDialog();
        }
        #endregion

        #region 选择数据库
        /// <summary>
        /// 选择数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Change_Database(object sender, RoutedEventArgs e)
        {
            Common.OpenWin(new SetLink(), this);
        }
        #endregion 

        #region 查询表
        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Table(object sender, RoutedEventArgs e)
        {
            var i = 0;
            tabNext = 0;

            if (Dtable.SelectedIndex != -1)
                Common.GetDataGridRow(Dtable, Dtable.SelectedIndex, false);

            foreach (var temp in Dtable.Items)
            {
                var item = temp as BaseTable;
                if (!String.IsNullOrEmpty(txtTable.Text.Trim()))
                {
                    Dtable.Items.MoveCurrentToNext();
                    if (item.tabName.ToLower().Contains(txtTable.Text.ToLower().Trim()))
                    {
                        tabNext = 1;
                        Common.GetDataGridRow(Dtable, i);
                        Dtable.ScrollIntoView(temp);
                        Dtable.Focus();
                        break;
                    }
                    else
                        Common.GetDataGridRow(Dtable, i, false);
                }

                if (!String.IsNullOrEmpty(txtComments.Text.Trim()))
                {
                    Dtable.Items.MoveCurrentToNext();
                    if (item.tabComments.ToLower().Contains(txtComments.Text.ToLower().Trim()))
                    {
                        Common.GetDataGridRow(Dtable, i);
                        Dtable.ScrollIntoView(temp);
                        Dtable.Focus();
                        break;
                    }
                    else
                        Common.GetDataGridRow(Dtable, i, false);
                }
                i++;
            }
        }
        #endregion

        #region 查看自定义实体
        /// <summary>
        /// 查看自定义实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Define(object sender, RoutedEventArgs e)
        {
            if (AppCache.GetDefineColumnList().Count == 0)
                CodeBox.Show("自定义实体数据为空", this);
            else
            {
                //弹出查询自定义列
                var definForm = new QueryDefine();
                definForm.DefineTable.ItemsSource = AppCache.GetDefineSoureTable();

                Common.DataGridSort(definForm.DefineTable, "tabName", ListSortDirection.Ascending);

                Common.OpenWin(definForm, this);
            }
        }
        #endregion

        #region 增加自定义实体
        /// <summary>
        /// 增加自定义实体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Define(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = 0;
                var tabItem = Dtable.SelectedItem as BaseTable;
                var tableList = AppCache.GetDefineSoureTable();
                var columnList = AppCache.GetDefineColumnList();

                foreach (var item in Dcolumn.Items)
                {
                    var box = Common.GetTemplateColumn<System.Windows.Controls.CheckBox>(Dcolumn, 0, "colBox", item);
                    var colItem = item as BaseColumn;
                    var DSTable = new DefineSoureTable();
                    if (box != null && box.IsChecked == true)
                    {
                        count++;
                        if (!columnList.Exists(a => a.colId == colItem.colId) && !tableList.Exists(a => a.colId == colItem.colId))
                        {
                            columnList.Add(colItem);
                            DSTable.colId = colItem.colId;
                            DSTable.colComments = colItem.colComments;
                            DSTable.colName = colItem.colName;
                            DSTable.tabComments = tabItem.tabComments;
                            DSTable.tabName = tabItem.tabName;
                            tableList.Add(DSTable);
                        }

                        box.IsChecked = false;
                    }
                }

                AppCache.SetDefineSoureTable(tableList);
                AppCache.SetDefineColumnList(columnList);

                if (count == 0)
                    CodeBox.Show("请选择要增加的列", this);
                else
                    CodeBox.Show("增加成功", this);
            }
            catch
            {
                CodeBox.Show("增加失败", this);
            }
        }
        #endregion

        #region 增加表单元素
        /// <summary>
        /// 增加表单元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_FromItem(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = 0;
                var fromList = AppCache.GetFromList();

                foreach (var item in Dcolumn.Items)
                {
                    var box = Common.GetTemplateColumn<System.Windows.Controls.CheckBox>(Dcolumn, 0, "colBox", item);
                    var colItem = item as BaseColumn;
                    if (box != null && box.IsChecked == true)
                    {
                        count++;
                        if (!fromList.Exists(a => a.colId == colItem.colId))
                        {
                            var from = new FromItems();
                            from.colId = colItem.colId;
                            from.fromName = colItem.colComments;
                            from.fromParam = colItem.colName;
                            from.maxLength = colItem.colLength;
                            from.isNull = colItem.isNull;
                            from.colComments = colItem.colComments;
                            from.colType = colItem.colType;
                            from.colName = colItem.colName;
                            fromList.Add(from);
                        }

                        box.IsChecked = false;
                    }
                }

                AppCache.SetFromList(fromList);

                if (count == 0)
                    CodeBox.Show("请选择要增加的表单元素", this);
                else
                    CodeBox.Show("增加成功", this);
            }
            catch
            {
                CodeBox.Show("增加失败", this);
            }
        }
        #endregion

        #region 查看表单
        /// <summary>
        /// 查看表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_From(object sender, RoutedEventArgs e)
        {
            if (AppCache.GetFromList().Count == 0)
                CodeBox.Show("表单元素为空", this);
            else
            {
                //弹出查询表单元素
                var from = new From();
                from.fromTable.ItemsSource = AppCache.GetFromList();

                Common.DataGridSort(from.fromTable, "tabName", ListSortDirection.Ascending);

                Common.OpenWin(from, this);
            }
        }
        #endregion

        #region 数据迁移
        /// <summary>
        /// 数据迁移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Common.OpenWin(new DataMove_1(), this);
        }
        #endregion

        #region 编辑表备注
        /// <summary>
        /// 编辑表备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dtable_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var list = AppCache.GetTableList(AppCache.GetBuildLink());
            var item = e.Row.Item as BaseTable;
            var temp = list.Find(a => a.tabName == item.tabName);
            list.Remove(temp);
            list.Add(item);
            AppCache.SetTableList(list, AppCache.GetBuildLink());

            DataSchema.UpdateTabComments(e.Row.Item as BaseTable, AppCache.GetBuildLink());
        }
        #endregion

        #region 查询列
        /// <summary>
        /// 查询列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Column(object sender, RoutedEventArgs e)
        {
            var i = 0;
            colNext = 0;

            if (Dcolumn.SelectedIndex != -1)
                Common.GetDataGridRow(Dcolumn, Dcolumn.SelectedIndex, false);

            foreach (var temp in Dcolumn.Items)
            {
                var item = temp as BaseColumn;
                Dcolumn.Items.MoveCurrentToNext();
                if (item.colName.ToLower().Contains(findColName.Text.ToLower().Trim()) && !string.IsNullOrEmpty(findColName.Text))
                {
                    colNext = 1;
                    Common.GetDataGridRow(Dcolumn, i);
                    Dcolumn.ScrollIntoView(temp);
                    Dcolumn.Focus();
                    break;
                }
                else if (item.colComments.ToLower().Contains(findColRemark.Text.ToLower().Trim()) && !string.IsNullOrEmpty(findColRemark.Text))
                {
                    Common.GetDataGridRow(Dcolumn, i);
                    Dcolumn.ScrollIntoView(temp);
                    Dcolumn.Focus();
                    break;
                }
                else
                    Common.GetDataGridRow(Dcolumn, i, false);
                i++;
            }
        }
        #endregion

        #region 下一个列
        /// <summary>
        /// 下一个列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Column(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var i = 0;

            if (Dcolumn.SelectedIndex != -1)
                Common.GetDataGridRow(Dcolumn, Dcolumn.SelectedIndex, false);

            foreach (var temp in Dcolumn.Items)
            {
                var item = temp as BaseColumn;
                Dcolumn.Items.MoveCurrentToNext();

                if (item.colName.ToLower().Contains(findColName.Text.ToLower().Trim()) && !string.IsNullOrEmpty(findColName.Text))
                {
                    count++;
                    if (count > colNext)
                    {
                        colNext++;
                        Common.GetDataGridRow(Dcolumn, i);
                        Dcolumn.ScrollIntoView(temp);
                        Dcolumn.Focus();
                        break;
                    }
                    else
                        Common.GetDataGridRow(Dcolumn, i, false);
                }
                else if (item.colComments.ToLower().Contains(findColRemark.Text.ToLower().Trim()) && !string.IsNullOrEmpty(findColRemark.Text))
                {
                    count++;
                    if (count > colNext)
                    {
                        colNext++;
                        Common.GetDataGridRow(Dcolumn, i);
                        Dcolumn.ScrollIntoView(temp);
                        Dcolumn.Focus();
                        break;
                    }
                    else
                        Common.GetDataGridRow(Dcolumn, i, false);
                }
                else
                    Common.GetDataGridRow(Dcolumn, i, false);
                i++;
            }
        }
        #endregion

        #region 下一个表
        /// <summary>
        /// 下一个表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Table(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var i = 0;

            if (Dtable.SelectedIndex != -1)
                Common.GetDataGridRow(Dtable, Dtable.SelectedIndex, false);

            foreach (var temp in Dtable.Items)
            {
                var item = temp as BaseTable;
                if (!String.IsNullOrEmpty(txtTable.Text.Trim()))
                {
                    Dtable.Items.MoveCurrentToNext();
                    if (item.tabName.ToLower().Contains(txtTable.Text.ToLower().Trim()))
                    {
                        count++;
                        if (count > tabNext)
                        {
                            tabNext++;
                            Common.GetDataGridRow(Dtable, i);
                            Dtable.ScrollIntoView(temp);
                            Dtable.Focus();
                            break;
                        }
                        else
                            Common.GetDataGridRow(Dtable, i, false);
                    }
                    else
                        Common.GetDataGridRow(Dtable, i, false);
                }

                if (!String.IsNullOrEmpty(txtComments.Text.Trim()))
                {
                    Dtable.Items.MoveCurrentToNext();
                    if (item.tabComments.ToLower().Contains(txtComments.Text.ToLower().Trim()))
                    {
                        count++;
                        if (count > tabNext)
                        {
                            tabNext++;
                            Common.GetDataGridRow(Dtable, i);
                            Dtable.ScrollIntoView(temp);
                            Dtable.Focus();
                            break;
                        }
                        else
                            Common.GetDataGridRow(Dtable, i, false);
                    }
                    else
                        Common.GetDataGridRow(Dtable, i, false);
                }
                i++;
            }
        }
        #endregion

        #region 编辑列备注
        /// <summary>
        /// 编辑列备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dcolumn_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var table = Dtable.SelectedItem as BaseTable;

            if (table != null)
            {
                var list = DataSchema.ColumnList(AppCache.GetBuildLink(), table.tabName) ?? new List<BaseColumn>();
                var item = e.Row.Item as BaseColumn;

                var temp = list.Find(a => a.colId == item.colId);
                list.Remove(temp);
                list.Add(item);
                AppCache.SetTableColumn(list, DataSchema.GetColumnKey(AppCache.GetBuildLink(), table.tabName));
            }

            DataSchema.UpdateColComments(e.Row.Item as BaseColumn, Dtable.SelectedItem as BaseTable, AppCache.GetBuildLink());
        }
        #endregion

        #region 重写关闭
        /// <summary>
        /// 重写关闭
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        #endregion

        #region 生成CHM
        /// <summary>
        /// 生成CHM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bulid_Chm(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = string.Format("{0}chm", AppDomain.CurrentDomain.BaseDirectory);
                
                if (txtFile != "")
                    path = txtFile;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //生成list
                var list = new List<ChmModel>();
                foreach (var item in Dtable.Items)
                {
                    var box = Common.GetTemplateColumn<System.Windows.Controls.CheckBox>(Dtable, 0, "tabBox", item);

                    if (box != null && box.IsChecked == true)
                    {
                        var model = new ChmModel();
                        model.tabComments = (item as BaseTable).tabComments;
                        model.tabName = (item as BaseTable).tabName;
                        model.columns = DataSchema.ColumnList(AppCache.GetBuildLink(), model.tabName) ?? new List<BaseColumn>();
                        model.columns = model.disColType(model.columns);

                        list.Add(model);
                    }
                }
                
                if (list.Count == 0)
                {
                    CodeBox.Show("请选择要生成表", this);
                    return;
                }


                Chm.CreateHhp(path);
                Chm.CreateHhc(path,list);
                Chm.CreateHhk(path,list);
                Chm.Compile(path,list);

                CodeBox.Show("生成成功", this);
            }
            catch(Exception ex)
            {
                CodeBox.Show("生成失败", this);
            }
        }
        #endregion

       
       
        #region 生成建表语句
        /// <summary>
        /// 生成建表语句
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bulid_Table(object sender, RoutedEventArgs e)
        {
            var bat = new StringBuilder();
            var path = string.Format("{0}sql", AppDomain.CurrentDomain.BaseDirectory);

            if (txtFile != "")
                path = txtFile + "\\sql";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var item in Dtable.Items)
            {
                var box = Common.GetTemplateColumn<System.Windows.Controls.CheckBox>(Dtable, 0, "tabBox", item);

                if (box != null && box.IsChecked == true)
                {
                    var sql = new StringBuilder();
                    sql.AppendFormat("create table {0}(\r\n", (item as BaseTable).tabName);
                    var field = DataSchema.ColumnList(AppCache.GetBuildLink(), (item as BaseTable).tabName) ?? new List<BaseColumn>();
                    var i = 0;
                    foreach (var temp in field)
                    {
                        if (AppCache.GetBuildLink().dbType == DataDbType.MySql)
                        {
                            if (temp.isNull == "是")
                                sql.AppendFormat("\t{0} {1}{2} comment {3}\r\n", temp.colName, temp.showType, i == field.Count - 1 ? "" : ",", temp.colComments.Replace("'",""));
                            else
                                sql.AppendFormat("\t{0} {1} not null{2} comment {3}\r\n", temp.colName, temp.showType, i == field.Count - 1 ? "" : ",", temp.colComments.Replace("'", ""));
                        }
                        else
                        {
                            if (temp.isNull == "是")
                                sql.AppendFormat("\t{0} {1}{2}\r\n", temp.colName, temp.showType, i == field.Count - 1 ? "" : ",");
                            else
                                sql.AppendFormat("\t{0} {1} not null{2}\r\n", temp.colName, temp.showType, i == field.Count - 1 ? "" : ",");
                        }
                        i++;
                    }

                    if (AppCache.GetBuildLink().dbType == DataDbType.MySql)
                    {
                        sql.AppendFormat(")comment={0} ", (item as BaseTable).tabComments.Replace("'", ""));
                        bat.AppendFormat("mysql -h {0} -u {1} -p {2} < @{3}.sql\r\n", AppCache.GetBuildLink().hostName, AppCache.GetBuildLink().userName, AppCache.GetBuildLink().userPwd, (item as BaseTable).tabName);
                    }
                    else
                        sql.Append(") ");

                    if (AppCache.GetBuildLink().dbType == DataDbType.SqlServer)
                    {
                        foreach (var temp in field)
                        {
                            if (temp.colComments != "")
                                sql.AppendFormat("execute sp_addextendedproperty 'MS_Description','{0}','user','dbo','table','{1}','column','{2}';\r\n", temp.colComments.Replace("'", ""), (item as BaseTable).tabName, temp.colName);
                        }
                        sql.AppendFormat("execute sp_addextendedproperty 'MS_Description','{0}','user','dbo','table','{1}',null,null;\r\n", (item as BaseTable).tabComments.Replace("'", ""), (item as BaseTable).tabName);

                        bat.AppendFormat("sqlcmd -U {0} -P {1} -i @{2}.sql\r\n", AppCache.GetBuildLink().userName, AppCache.GetBuildLink().userPwd, (item as BaseTable).tabName);
                    }

                    if (AppCache.GetBuildLink().dbType == DataDbType.Oracle)
                    {
                        sql.Append("\r\n tablespace USERS pctfree 10 initrans 1 maxtrans 255 storage(initial 64K minextents 1 maxextents unlimited);\r\n");
                        foreach(var temp in field)
                        {
                            if (temp.colComments != "")
                                sql.AppendFormat("comment on column {0}.{1} is '{2}'; \r\n", (item as BaseTable).tabName, temp.colName, temp.colComments.Replace("'", ""));
                        }
                        sql.AppendFormat("comment on table {0} is '{1}';\r\n", (item as BaseTable).tabName, (item as BaseTable).tabComments.Replace("'", ""));

                        bat.AppendFormat("sqlplus {0}/{1}@{2} @{3}.sql> CreateTable.log\r\n", AppCache.GetBuildLink().userName, AppCache.GetBuildLink().userPwd, AppCache.GetBuildLink().serverValue, (item as BaseTable).tabName);
                    }

                    File.WriteAllText(string.Format("{0}/{1}.sql", path, (item as BaseTable).tabName), sql.ToString(), Encoding.UTF8);
                }
            }

            bat.Append("exit;");

            if (AppCache.GetBuildLink().dbType == DataDbType.Oracle)
                File.WriteAllText(string.Format("{0}/Oracle.bat", path), bat.ToString(), Encoding.UTF8);

            if (AppCache.GetBuildLink().dbType == DataDbType.MySql)
                File.WriteAllText(string.Format("{0}/MySql.bat", path), bat.ToString(), Encoding.UTF8);

            if (AppCache.GetBuildLink().dbType == DataDbType.SqlServer)
                File.WriteAllText(string.Format("{0}/SqlServer.bat", path), bat.ToString(), Encoding.UTF8);

            CodeBox.Show("生成成功", this);
        }
        #endregion
    }
}
