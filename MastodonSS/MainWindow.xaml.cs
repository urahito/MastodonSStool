using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MastodonSS
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool blnCanCopy;
        private bool blnHashtag;
        private bool blnForTwitter;
        private string strArticle;
        private string strTitle;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region イベント
        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tblkCount.Text = "0";
            strArticle = "";
            strTitle = "";

            blnCanCopy = true;
            blnForTwitter = false;
            blnHashtag = true;
            ckbHashtag.IsChecked = true;

            cmbMaxChar.Items.Add(500);
            cmbMaxChar.Items.Add(140);
            cmbMaxChar.SelectedIndex = 0;

            getArticle();
        }

        /// <summary>
        /// 現在の本文を記録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbArticle_TextChanged(object sender, TextChangedEventArgs e)
        {
            getArticle();
        }

        /// <summary>
        /// 現在のタイトルを記録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            getTitle();
        }

        /// <summary>
        /// タイトルコピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyTitle_Click(object sender, RoutedEventArgs e)
        {
            getTitle();

            // タイトルが空ならコピーしない
            if (CommonUtility.IsNoArticle(txbTitle.Text)) return;
            Clipboard.SetText(strTitle);
        }

        /// <summary>
        /// 本文コピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyArticle_Click(object sender, RoutedEventArgs e)
        {
            getArticle();

            // 文字数オーバー・不足時はコピー不可
            if (blnCanCopy == false) return;
            // Twitter用は、ハッシュタグ付きでない、タイトル無しまたは本文無しではコピー不可
            if (blnForTwitter)
            {
                if (!blnHashtag || CanCopyArticle()) return;
            }
            // Mastodon用は、タイトルまたは本文無しではコピー不可
            else
            {
                if (CanCopyArticle()) return;
            }

            Clipboard.SetText(strArticle);
        }

        /// <summary>
        /// ハッシュタグの切り替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbHashtag_Click(object sender, RoutedEventArgs e)
        {
            getArticle();
            blnHashtag = (bool)ckbHashtag.IsChecked;
        }

        /// <summary>
        /// 最大文字数でモード切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbMaxChar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((int)cmbMaxChar.SelectedValue == 140)
            {
                blnForTwitter = true;
                blnHashtag = true;
                ckbHashtag.IsChecked = true;
            }
            else
            {
                blnForTwitter = false;
            }
            setTextCount(strArticle.Length);
        }
        #endregion

        #region 画面連携
        private bool CanCopyArticle()
        {
            return !(CommonUtility.IsNoArticle(txbTitle.Text) || CommonUtility.IsNoArticle(txbArticle.Text));
        }

        /// <summary>
        /// 本文を取得し、文字数を反映
        /// </summary>
        private void getArticle()
        { 
            // ハッシュタグの切り替えを反映
            if(ckbHashtag.IsChecked == true)
            {
                strArticle = "#" + txbTitle.Text + "\r\n";
            }
            else
            {
                strArticle = "";
            }

            // 本文を追記
            strArticle = strArticle + txbArticle.Text;

            // 文字数を反映
            setTextCount(strArticle.Length);
            return;
        }

        /// <summary>
        /// タイトルを取得し、文字数を反映
        /// </summary>
        private void getTitle()
        {
            // 本文を取得
            getArticle();
            
            // タイトルを取得（ナンバリングがあればそれを追記）
            strTitle = txbTitle.Text + " " + txbNo.Text;

            // ナンバリングが有効な数字でない場合、タイトルのみにする
            if (CommonUtility.IsInt(txbNo.Text) == false)
            {
                strTitle = txbTitle.Text;
            }

            // 文字数を反映
            setTextCount(strArticle.Length);
            return;
        }

        /// <summary>
        /// 文字数を反映
        /// </summary>
        /// <param name="txtLen"></param>
        private void setTextCount(int txtLen)
        {
            // 文字数を反映
            tblkCount.Text = txtLen.ToString();

            // 文字数オーバー時は赤で表示
            if (txtLen > (int)cmbMaxChar.SelectedValue)
            {
                tblkCount.Foreground = Brushes.Red;
                blnCanCopy = false;
            }
            // 本文がないときは灰色で表示
            else if (txbArticle.Text.Length == 0)
            {
                tblkCount.Foreground = Brushes.Gray;
                blnCanCopy = false;
            }
            // 問題が無いときは黒で表示
            else
            {
                tblkCount.Foreground = Brushes.Black;
                blnCanCopy = true;
            }
        }
        #endregion
    }
}
