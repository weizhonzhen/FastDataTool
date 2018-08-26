using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataModel;
using System.IO;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TextTemplating;

namespace FastDataTool
{
    /// <summary>
    /// 公用类
    /// </summary>
    public class Common
    {
        //变量
        private static SymmetricAlgorithm mobjCryptoService = new RijndaelManaged();
        private static string path = AppDomain.CurrentDomain.BaseDirectory + "link.config";

        public Common() { }
        
        #region checkbox全选
        /// <summary>
        /// checkbox全选
        /// </summary>
        /// <param name="box"></param>
        /// <param name="grid"></param>
        public static void CheckAllBox(CheckBox box,DataGrid grid,string colName)
        {
            var tempCol = grid.Columns[0] as DataGridTemplateColumn;

            foreach (var item in grid.Items)
            {
                var element = grid.Columns[0].GetCellContent(item);
                if (element != null)
                {
                    var iBox = tempCol.CellTemplate.FindName(colName, element) as CheckBox;
                    if (iBox != null)
                        iBox.IsChecked = box.IsChecked;
                }
            }
        }
        #endregion

        #region 获取连接字符串
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string GetConnStr(string DbType, TextBox UserName, TextBox UserPwd, TextBox HostName, TextBox Port, TextBox ServerName)
        {
            var connStr = "";

            if (DbType == DataDbType.Oracle)
            {
                connStr = string.Format("User Id={0};Password={1};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3})))(CONNECT_DATA=(SERVICE_NAME={4})));pooling=true;Min Pool Size=1;Max Pool Size=5;"
                                    , UserName.Text.Trim(), UserPwd.Text.Trim(), HostName.Text.Trim(), Port.Text.Trim(), ServerName.Text.Trim());
            }
            else if (DataDbType.SqlServer == DbType)
            {
                connStr = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4};pooling=true;Min Pool Size=1;Max Pool Size=5;"
                                    , HostName.Text.Trim(), Port.Text.Trim(), ServerName.Text.Trim(), UserName.Text.Trim(), UserPwd.Text.Trim());
            }
            else if (DataDbType.MySql == DbType)
            {
                connStr = string.Format("server={0};port={1};Database={2};user id={3};password={4};pooling=true;Min Pool Size=1;Max Pool Size=5;CharSet=utf8;"
                                   , HostName.Text.Trim(), Port.Text.Trim(), ServerName.Text.Trim(), UserName.Text.Trim(), UserPwd.Text.Trim());
            }

            return connStr;
        }
        #endregion
        
        #region 生成实体调用模板
        /// <summary>
        /// 生成实体调用模板
        /// </summary>
        /// <returns></returns>
        public static bool BuildCodeModel(EntityInfo entity,string extensionName="")
        {
            var refValue = true;
            var sb = new StringBuilder();

            //T4模板路径
            var templateFileName = "";

            if (entity.language.ToUpper() == "C#")
            {
                if (entity.isModel)
                    templateFileName = string.Format("{0}Template\\Model_cs.tt", AppDomain.CurrentDomain.BaseDirectory);

                if (entity.isOldModel)
                    templateFileName = string.Format("{0}Template\\ModelOld_cs.tt", AppDomain.CurrentDomain.BaseDirectory);

                if (entity.isMap)
                    templateFileName = string.Format("{0}Template\\Map.tt", AppDomain.CurrentDomain.BaseDirectory);

                if (entity.isCheck)
                    templateFileName = string.Format("{0}Template\\Check_cs.tt", AppDomain.CurrentDomain.BaseDirectory);

                if (entity.isSerialize)
                    templateFileName = string.Format("{0}Template\\Serialize_cs.tt", AppDomain.CurrentDomain.BaseDirectory);

                if(entity.isMap)
                    templateFileName = string.Format("{0}Template\\Map.tt", AppDomain.CurrentDomain.BaseDirectory);

                if (entity.isFrom)
                    templateFileName = string.Format("{0}Template\\From.tt", AppDomain.CurrentDomain.BaseDirectory);
            }
            else
                templateFileName = string.Format("{0}Template\\Model_java.tt", AppDomain.CurrentDomain.BaseDirectory);
                        
            //生成实体路径
            var modelFile = AppDomain.CurrentDomain.BaseDirectory + "model";

            //生成实体路径
            if (!string.IsNullOrEmpty(entity.modelFile))
                modelFile = entity.modelFile;

            //T4模板服务
            var host = new CustomTextTemplatingEngineHost();
            var engine = new Engine();
            
            //传入模板内容
            host.TemplateFileValue = templateFileName;
            string input = File.ReadAllText(templateFileName);

            //传参给模板
            host.Session = new TextTemplatingSession();
            host.Session.Add("entity", entity);
            engine.ProcessTemplate(input, host);

            //生成内容
            string output = engine.ProcessTemplate(input, host);

            //错误内容
            foreach (CompilerError error in host.Errors)
            {
                sb.Append(error.Line).Append(":").AppendLine(error.ErrorText);
            }

            log.SaveLog(sb.ToString(), "error");

            //生成文件
            if (!System.IO.Directory.Exists(modelFile))
            {
                System.IO.Directory.CreateDirectory(modelFile);
            }

            //写入文件
            if (extensionName == "")
            {
                if (entity.language.ToUpper() == "C#")
                    File.WriteAllText(string.Format("{0}/{1}.cs", modelFile, entity.table.tabName), output, Encoding.UTF8);
                else
                    File.WriteAllText(string.Format("{0}/{1}.java", modelFile, entity.table.tabName), output, Encoding.UTF8);
            }
            else
                File.WriteAllText(string.Format("{0}/{1}.{2}", modelFile, entity.table.tabName, extensionName), output, Encoding.UTF8);

            return refValue;
        }
        #endregion
        
        #region 选择一行
        /// <summary>
        /// 选择一行
        /// </summary>
        /// <param name="datagrid"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static void GetDataGridRow(System.Windows.Controls.DataGrid datagrid, int rowIndex,bool isCheck=true)
        {
            var row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row != null)
            {
                datagrid.UpdateLayout();
                row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                row.IsSelected = isCheck;
            }
        }
        #endregion
    
        #region ComboBoxItem选中
        /// <summary>
        /// ComboBoxItem选中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="selectValue"></param>
        public static void ComboBoxSelect(ComboBox box, string value, bool isDataLink = false)
        {
            foreach (var item in box.Items)
            {
                if (isDataLink)
                {
                    var boxItem = item as DataLink;
                    if (boxItem.linkName == value)
                        box.SelectedItem = item;
                }
                else
                {
                    var boxItem = item as ComboBoxItem;
                    if (boxItem.Content.ToString() == value)
                        box.SelectedItem = item;
                }
            }
        }
        #endregion

        #region 保存连接名单个
        /// <summary>
        /// 保存连接名单个
        /// </summary>
        public static void SaveConfigLink(DataLink item)
        {
            if (!File.Exists(path))
            {
                using (var fs = File.Create(path)) { }
            }

            if (item.linkName == "")
                item.linkName = GetLinkName(item);

            var list = GetConfigLink();

            list.RemoveAll(a => a.linkName == item.linkName);
            list.Add(item);

            DataCache.Set<List<DataLink>>("AllLink", list);
        }
        #endregion

        #region 保存连接名所有
        /// <summary>
        /// 保存连接名所有
        /// </summary>
        public static void SaveConfigLinkAll(List<DataLink> list)
        {
            DataCache.Set<List<DataLink>>("AllLink", list);
        }
        #endregion

        #region 获取连接名列表
        /// <summary>
        /// 获取连接名列表
        /// </summary>
        /// <returns></returns>
        public static List<DataLink> GetConfigLink()
        {
            return DataCache.Get<List<DataLink>>("AllLink");
        }
        #endregion

        #region 获取datagrid中的自定义对象单元格
        /// <summary>
        /// 获取datagrid中的自定义对象单元格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid">DataGrid</param>
        /// <param name="columnId">列数</param>
        /// <param name="templateName">自定义列名</param>
        /// <param name="item">datagrid 行</param>
        /// <returns></returns>
        public static T GetTemplateColumn<T>(DataGrid grid, int columnId, string templateName, object item) where T : class,new()
        {
            try
            {
                if (item == null)
                    return null;

                var tempCol = grid.Columns[columnId] as DataGridTemplateColumn;
                var element = grid.Columns[columnId].GetCellContent(item);

                if (element != null)
                    return tempCol.CellTemplate.FindName(templateName, element) as T;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region datagrid 排序
        /// <summary>
        /// datagrid 排序
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sortFiled"></param>
        /// <param name="sort"></param>
        public static void DataGridSort(DataGrid grid,string sortFiled,ListSortDirection sort)
        {
            var view = CollectionViewSource.GetDefaultView(grid.ItemsSource);
            view.SortDescriptions.Clear();
            var sd = new SortDescription(sortFiled, sort);
            view.SortDescriptions.Add(sd);
        }
        #endregion

        #region 弹出窗口任务栏不显示并指定父窗口
        /// <summary>
        /// 弹出窗口任务栏不显示并指定父窗口
        /// </summary>
        /// <param name="win">新窗体</param>
        /// <param name="owner">拥有者</param>
        /// <param name="isCloseOwner">是否关闭父窗休</param>
        /// <param name="isOwnerOwer">是否父窗体的拥有者</param>
        public static void OpenWin(Window win, Window owner, bool isCloseOwner = false, bool isOwnerOwer = false)
        {
            try
            {
                if (isOwnerOwer)
                    win.Owner = owner.Owner;
                else
                    win.Owner = owner;

                if (isCloseOwner)
                    owner.Close();

                win.ShowInTaskbar = false;
                win.ShowDialog();
            }
            catch(Exception ex)
            {
                log.SaveLog(ex.ToString(), "openWin_exp");
            }
        }
        #endregion

        #region 弹出选择目录对话框
        /// <summary>
        /// 弹出选择目录对话框
        /// </summary>
        /// <returns></returns>
        public static string FolderBrowserDialog()
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
                return fbd.SelectedPath;
            else
                return "";
        }
        #endregion

        #region 连接初始化
        /// <summary>
        /// 连接初始化
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="serveName">服务名</param>
        /// <param name="dbType">数据库类型</param>
        public static string InitLink(ref TextBox txtPort, ref Label labServerName, ComboBox box)
        {
            var boxItem = box.SelectedItem as ComboBoxItem;
            var dbType = boxItem.Content.ToString();

            if (txtPort != null && labServerName != null)
            {
                if (dbType == DataDbType.Oracle)
                {
                    txtPort.Text = "1521";
                    labServerName.Content = "服务名：";
                }
                else if (dbType == DataDbType.SqlServer)
                {
                    txtPort.Text = "1433";
                    labServerName.Content = "库名称：";
                }
                else if (dbType == DataDbType.MySql)
                {
                    txtPort.Text = "3306";
                    labServerName.Content = "库名称：";
                }
            }

            return dbType;
        }
        #endregion

        #region 连接名格式
        /// <summary>
        /// 连接名格式
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="userName">用户名</param>
        /// <param name="serverValue">数据库名</param>
        /// <returns></returns>
        public static string GetLinkName(DataLink item)
        {
            return string.Format("{0}_{1}_{2}", item.dbType, item.userName, item.serverValue);            
        }
        #endregion

        #region 控件值给DataLink
        /// <summary>
        ///  控件值给DataLink
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="dbConn"></param>
        /// <param name="txtHostName"></param>
        /// <param name="txtUserName"></param>
        /// <param name="txtPwd"></param>
        /// <param name="txtPort"></param>
        /// <param name="txtServerName"></param>
        /// <param name="labServerName"></param>
        /// <returns></returns>
        public static DataLink ControlsToData(string dbType, TextBox txtHostName, TextBox txtUserName, TextBox txtPwd,
            TextBox txtPort, TextBox txtServerName, Label labServerName, bool isLink = false, TextBox txtLinkName = null)
        {
            var item = new DataLink();
            item.dbType = dbType;
            item.hostName = txtHostName.Text.Trim();
            item.userName = txtUserName.Text.Trim();
            item.userPwd = txtPwd.Text.Trim();
            item.port = txtPort.Text.Trim();
            item.serverValue = txtServerName.Text.Trim();
            item.serverName = labServerName.Content.ToString();

            if (isLink)
            {
                if (txtLinkName.Text.Trim() == "")
                    item.linkName = GetLinkName(AppCache.GetBuildLink());
                else
                    item.linkName = txtLinkName.Text.Trim();
            }

            return item;
        }
        #endregion

        #region DataLink给控件值
        /// <summary>
        /// DataLink给控件值
        /// </summary>
        /// <param name="link"></param>
        /// <param name="txtHostName"></param>
        /// <param name="txtUserName"></param>
        /// <param name="txtPwd"></param>
        /// <param name="txtPort"></param>
        /// <param name="txtServerName"></param>
        /// <param name="labServerName"></param>
        /// <param name="txtLinkName"></param>
        public static void DataToControls(DataLink link,ref TextBox txtHostName, ref TextBox txtUserName,ref TextBox txtPwd,
                                           ref TextBox txtPort,ref TextBox txtServerName,ref Label labServerName,ref TextBox txtLinkName)
        {
            txtHostName.Text = link.hostName;
            txtUserName.Text = link.userName;
            txtPwd.Text = link.userPwd;
            txtPort.Text = link.port;
            txtServerName.Text = link.serverValue;
            labServerName.Content = link.serverName;
            txtLinkName.Text = link.linkName;
        }        
        #endregion

        #region 定义窗口类型
        /// <summary>
        /// 定义窗口类型
        /// </summary>
        /// <param name="win"></param>
        public static void InitWindows(Window win, ResizeMode rm = ResizeMode.NoResize, WindowStartupLocation wl = WindowStartupLocation.CenterOwner)
        {
            win.WindowStartupLocation = wl;
            win.ResizeMode = rm;
            var uri = new Uri(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "") + "img/code.ico", UriKind.RelativeOrAbsolute);
            win.Icon = BitmapFrame.Create(uri);
            win.Background = (Brush)TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#efefef");
        }
        #endregion
    }
}
