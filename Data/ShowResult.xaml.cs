using System.Windows;

namespace Data
{
    /// <summary>
    /// ShowResult.xaml 的交互逻辑
    /// </summary>
    public partial class ShowResult : Window
    {
        public ShowResult()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }

        /// <summary>
        /// 内容消息
        /// </summary>
        public string Message
        {
            get { return this.labMessage.Content.ToString(); }
            set { this.labMessage.Content = value; }
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static void Show(string msg, Window owner)
        {
            var msgBox = new ShowResult();
            msgBox.Message = msg;
            Common.OpenWin(msgBox, owner.Owner);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
