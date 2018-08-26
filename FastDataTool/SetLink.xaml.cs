using System.Windows;
using DataModel;

namespace FastDataTool
{
    /// <summary>
    /// SetLink.xaml 的交互逻辑
    /// </summary>
    public partial class SetLink : Window
    {
        //连接串
        public string dbConn = "";

        //数据库
        public string dbType = "";

        #region 加载
        /// <summary>
        /// 加载
        /// </summary>
        public SetLink()
        {
            InitializeComponent();
            Common.InitWindows(this);

            if (Common.GetConfigLink().Count != 0)
                dbTypeLink.ItemsSource = Common.GetConfigLink();

            if (AppCache.GetBuildLink() != null)
                InitLinkInfo();
        }
        #endregion

        #region 测试数据库连接
        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Conn_Click(object sender, RoutedEventArgs e)
        {
            dbConn = Common.GetConnStr(dbType, txtUserName, txtPwd, txtHostName, txtPort, txtServerName);

            if (DataSchema.CheckLink(dbType, dbConn))
                CodeBox.Show("连接成功！",this);
            else
                CodeBox.Show("连接失败！",this);
        }
        #endregion        

        #region 数据库选择中
        /// <summary>
        /// 数据库选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_Selected(object sender, RoutedEventArgs e)
        {
            dbType = Common.InitLink(ref txtPort, ref labServerName, boxDbType);           
        }
        #endregion

        #region 连接名选择中
        /// <summary>
        /// 连接名选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_Selected(object sender, RoutedEventArgs e)
        {
            var boxItem = dbTypeLink.SelectedItem as DataLink;

            if (boxItem != null)
            {
                AppCache.SetBuildLink(boxItem);
                InitLinkInfo();
            }
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            dbConn = Common.GetConnStr(dbType, txtUserName, txtPwd, txtHostName, txtPort, txtServerName);

            if (!DataSchema.CheckLink(dbType, dbConn))
                CodeBox.Show("连接数据库失败！",this);
            else
            {
                var buildLink = Common.ControlsToData(dbType, txtHostName, txtUserName, txtPwd, txtPort, txtServerName, labServerName, true, txtLinkName);
                buildLink.connStr = dbConn;
                Common.SaveConfigLink(buildLink);
                AppCache.SetBuildLink(buildLink);
                this.Owner.Title = string.Format("数据工具-{0}", txtLinkName.Text);
                AppCache.SetTitle(this.Owner.Title);
                
                this.Close();
            }
        }
        #endregion         

        #region 初始化连接信息
        /// <summary>
        /// 初始化连接信息
        /// </summary>
        private void InitLinkInfo()
        {
            Common.DataToControls(AppCache.GetBuildLink(), ref txtHostName, ref txtUserName, ref txtPwd, ref txtPort, ref txtServerName, ref labServerName, ref txtLinkName);
            Common.ComboBoxSelect(boxDbType, AppCache.GetBuildLink().dbType);
            Common.ComboBoxSelect(dbTypeLink, AppCache.GetBuildLink().linkName, true);
        }
        #endregion
        
        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            var item = dbTypeLink.SelectedItem as DataLink;
            if (item != null)
            {
                var list = Common.GetConfigLink();
                list.RemoveAll(a => a.linkName == item.linkName);
                Common.SaveConfigLinkAll(list);

                dbTypeLink.ItemsSource = list;
                DataCache.Remove("buildLink");
                InitLinkInfo();
            }
        }
        #endregion         
    }
}
