using System.Windows;

namespace Data
{
    /// <summary>
    /// DeEnFrom.xaml 的交互逻辑
    /// </summary>
    public partial class DeEnFrom : Window
    {
        public DeEnFrom()
        {
            InitializeComponent();
            Common.InitWindows(this);
        }

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            DeText.Text = Common.EncodeGB2312(NullText.Text);
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            DeText.Text = Common.DecodeGB2312(NullText.Text);
        }
        #endregion
    }
}
