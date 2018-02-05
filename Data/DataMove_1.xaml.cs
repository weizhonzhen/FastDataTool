using System.Windows;
using System.Windows.Controls;
using DataModel;

namespace Data
{
    /// <summary>
    /// DataMove_1.xaml 的交互逻辑
    /// </summary>
    public partial class DataMove_1 : Window
    {
        #region 初始化
        public DataMove_1()
        {
            InitializeComponent();

            Common.InitWindows(this);

            if (Common.GetConfigLink().Count != 0)
            {
                targetLinkName.ItemsSource = Common.GetConfigLink();
                sourceLinkName.ItemsSource = Common.GetConfigLink();
            }

            if (AppCache.GetSoureLink() != null)
                InitLink();

            if (AppCache.GetTargetLink() != null)
                InitLink(true);
        }
        #endregion

        #region 目标数据库选择中
        /// <summary>
        /// 目标数据库选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetBox_Selected(object sender, RoutedEventArgs e)
        {
            Common.InitLink(ref txtTargetPort, ref labTargetServerName, TargetDbType);
        }
        #endregion

        #region 目标数据库测试
        /// <summary>
        /// 目标数据库测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Target_Click(object sender, RoutedEventArgs e)
        {
            //连接串
            var dbType = (TargetDbType.SelectedItem as ComboBoxItem).Content.ToString();
            var connStr = Common.GetConnStr(dbType, txtTargetUserName, txtTargetUserPwd, txtTargetHostName, txtTargetPort, txtTargetServerName);

            if (DataSchema.CheckLink(dbType, connStr))
            {
                var item = Common.ControlsToData(dbType, txtTargetHostName, txtTargetUserName, txtTargetUserPwd, txtTargetPort, txtTargetServerName, labTargetServerName,true,txtTargetLinkName);
                item.connStr = connStr;

                Common.SaveConfigLink(item);

                AppCache.SetTargetLink(item);

                CodeBox.Show("连接成功！", this);
            }
            else
                CodeBox.Show("连接失败！", this);
        }
        #endregion

        #region 源数据库选择中
        /// <summary>
        /// 源数据库选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceBox_Selected(object sender, RoutedEventArgs e)
        {
            Common.InitLink(ref txtSourcePort, ref labSourceServerName, SourceDbType);
        }
        #endregion
        
        #region 源连接名选择中
        /// <summary>
        /// 源连接名选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Source_Box_Selected(object sender, RoutedEventArgs e)
        {
            var boxItem = sourceLinkName.SelectedItem as DataLink;

            if (boxItem != null)
            {
                AppCache.SetSoureLink(boxItem);
                InitLink();
            }
        }
        #endregion

        #region 目标连接名选择中
        /// <summary>
        /// 目标连接名选择中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Target_Box_Selected(object sender, RoutedEventArgs e)
        {
            var boxItem = targetLinkName.SelectedItem as DataLink;

            if (boxItem != null)
            {
                AppCache.SetTargetLink(boxItem);
                InitLink(true);
            }
        }
        #endregion

        #region 源数据库测试
        /// <summary>
        /// 源数据库测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Source_Click(object sender, RoutedEventArgs e)
        {
            //连接串
            var dbType = (SourceDbType.SelectedItem as ComboBoxItem).Content.ToString();
            var connStr = Common.GetConnStr(dbType, txtSourceUserName, txtSourceUserPwd, txtSourceHostName, txtSourcePort, txtSourceServerName);
            
            if (DataSchema.CheckLink(dbType, connStr))
            {
                var item = Common.ControlsToData(dbType, txtSourceHostName, txtSourceUserName, txtSourceUserPwd, txtSourcePort, txtSourceServerName, labSourceServerName, true, txtSourceLinkName);
                item.connStr = connStr;
                AppCache.SetSoureLink(item);

                CodeBox.Show("连接成功！", this);
            }
            else
                CodeBox.Show("连接失败！", this);
        }
        #endregion

        #region 数据迁移对应表
        /// <summary>
        /// 数据迁移对应表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (DataSchema.CheckLink(AppCache.GetSoureLink().dbType, AppCache.GetSoureLink().connStr)
                && DataSchema.CheckLink(AppCache.GetTargetLink().dbType, AppCache.GetTargetLink().connStr))
            {
                //数据迁移-对应表
                var form = new DataMove_2();

                //源表
                AppCache.SetSoureTable(DataSchema.GetSourceTable(AppCache.GetSoureLink()));
                form.TableGrid.ItemsSource = AppCache.GetSoureTable();

                //目标表
                AppCache.SetTargetTable (DataSchema.TableList(AppCache.GetTargetLink()));

                //说明提示
                form.labTable.Content = string.Format("目标库({0})：{1}，源库({2})：{3}", AppCache.GetTargetLink().dbType
                                                                , Common.GetDataInfo(AppCache.GetTargetLink())
                                                                , AppCache.GetSoureLink().dbType
                                                                , Common.GetDataInfo(AppCache.GetSoureLink()));                

                Common.OpenWin(form, this, true, true);     
            }
            else
                CodeBox.Show("请配置正确数据库", this);
        }
        #endregion

        #region 初始化连接信息
        /// <summary>
        /// 初始化连接信息
        /// </summary>
        /// <param name="isTarget"></param>
        private void InitLink(bool isTarget=false)
        {
            if(isTarget)
            {
                Common.DataToControls(AppCache.GetTargetLink(), ref txtTargetHostName, ref txtTargetUserName, ref txtTargetUserPwd, ref txtTargetPort, ref txtTargetServerName, ref labTargetServerName, ref txtTargetLinkName);
                Common.ComboBoxSelect(TargetDbType, AppCache.GetTargetLink().dbType);
                Common.ComboBoxSelect(targetLinkName, AppCache.GetTargetLink().linkName, true);
            }
            else
            {
                Common.DataToControls(AppCache.GetSoureLink(), ref txtSourceHostName, ref txtSourceUserName, ref txtSourceUserPwd, ref txtSourcePort, ref txtSourceServerName, ref labSourceServerName, ref txtSourceLinkName);
                Common.ComboBoxSelect(SourceDbType, AppCache.GetSoureLink().dbType);
                Common.ComboBoxSelect(sourceLinkName, AppCache.GetSoureLink().linkName, true);
            }
        }
        #endregion
    }
}
